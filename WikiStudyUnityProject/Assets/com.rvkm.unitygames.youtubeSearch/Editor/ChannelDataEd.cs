using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using System.Security.AccessControl;
using System.Linq;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [CustomEditor(typeof(SearchDataYoutube))]
    public class ChannelDataEd : Editor
    {
        SearchDataYoutube data;
        static float progress, progressTag;
        static bool isProcessing, isProcessingTag;
        static List<EditorCoroutine> allCORs;
        static List<string> procList = new List<string>();
        static List<TagDesc> tagDescList = new List<TagDesc>();
        string nl;
        string genBtnStr, jsonBtnStr, tagBtnStr, cancelTagStr, cancelYTStr;
        public TextAsset htmlFileAssetTest;
        public string strTest;

        SerializedObject editorObj;
        public void OnEnable()
        {
            data = (SearchDataYoutube)target;
            data.SearchName = data.name;
            isProcessing = isProcessingTag = false;
            procList = new List<string>();
            tagDescList = new List<TagDesc>();
            nl = Environment.NewLine;
            genBtnStr = nl + "Generate youtube data" + nl;
            jsonBtnStr = nl + "Write to device(JSON)" + nl;
            tagBtnStr = nl + "Update tags" + nl;
            cancelTagStr = nl + "Cancel Tag Update" + nl;
            cancelYTStr = nl + "Cancel" + nl;
            allCORs = new List<EditorCoroutine>();
            StopAllEditorCoroutines();
            editorObj = new SerializedObject(this);
        }

        public override void OnInspectorGUI()
        {
            //GUILayout.Label("Search Name: " + data.SearchName);
            //GUILayout.Space(10);
            //serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("APIKEY"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InputHtmlFiles"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InputUrls"), true);
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("allTags"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ignoreTags"), true);
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("allVideos"), true);

            //editorObj.Update();
            //EditorGUILayout.PropertyField(editorObj.FindProperty("htmlFileAssetTest"));
            //EditorGUILayout.PropertyField(editorObj.FindProperty("strTest"), true);
            //editorObj.ApplyModifiedProperties();

            if (isProcessingTag)
            {
                GUILayout.BeginHorizontal("box");
                progressTag = (float)Math.Round((double)progressTag, 4);
                GUILayout.Label("Tag Progress: ");
                GUILayout.Button("" + (100 * progressTag) + "% ");
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                if (GUILayout.Button(cancelTagStr))
                {
                    StopAllEditorCoroutines();
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Error!", "Tag Operation aborted by user!", "Ok");
                    isProcessingTag = false;
                }
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (isProcessing)
            {
                GUILayout.BeginHorizontal("box");
                progress = (float)Math.Round((double)progress, 4);
                GUILayout.Label("Progress: ");
                GUILayout.Button("" + (100 * progress) + "% ");
                progress = EditorGUILayout.Slider(progress, 0f, 1f);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                if (GUILayout.Button(cancelYTStr))
                {
                    StopAllEditorCoroutines();
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Error!", "Operation aborted by user!", "Ok");
                    isProcessing = false;
                }
            }
            else
            {
                if (GUILayout.Button(genBtnStr))
                {
                    isProcessing = true;
                    EditorUtility.DisplayProgressBar("Processing", "Reading html files and/or textasset containing html files", 0);
                    List<YoutubeVideo> vList = new List<YoutubeVideo>();
                    vList.CopyUniqueFrom(data.allVideos);
                    if (data.InputHtmlFiles != null)
                    {
                        foreach (var txtAsset in data.InputHtmlFiles)
                        {
                            vList.CopyUniqueFrom(EdUtility.GetAllVideoInfo(txtAsset.text));
                        }
                    }

                    if (data.InputUrls != null)
                    {
                        foreach (var url in data.InputUrls)
                        {
                            if (!EdUtility.IsUrl(url)) { continue; }
                            vList.CopyUniqueFrom(EdUtility.GetAllVideoInfo(EdUtility.GetWWWResponse(url)));
                        }
                    }

                    EditorUtility.ClearProgressBar();
                    var cor = EditorCoroutineUtility.StartCoroutine(LoopAllYoutubeAPI(vList, serializedObject, (procList, ser) =>
                    {
                        data.allVideos = procList.ToArray();
                        UpdateTags(serializedObject);

                        string fPath = "";
                        bool success = false;
                        WriteDataToDevice(serializedObject, ref fPath, ref success);

                        EditorUtility.ClearProgressBar();
                        if (success)
                        {
                            EditorUtility.DisplayDialog("Success!", "Completed the operation successfully. " +
                            "Json data has been written into path: " + fPath, "Ok");
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error!", "Failure writing to device. check log. " +
                                "You might need to create or set the path first! " +
                            "at path: " + fPath, "Ok");
                        }
                        ser.Update();
                        ser.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                    }), this);
                    allCORs.Add(cor);
                }
                
                if (GUILayout.Button(tagBtnStr))
                {
                    if (data.IsDataOk() == false)
                    {
                        EditorUtility.DisplayDialog("Error!", "You are trying to update tags but youtube data is invalid! "
                            + Environment.NewLine + "Please generate the data first!", "Ok");
                    }
                    else
                    {
                        isProcessingTag = true;
                        UpdateTags(serializedObject);
                    }
                }
                if (GUILayout.Button(jsonBtnStr))
                {
                    string fPath = "";
                    bool success = false;
                    WriteDataToDevice(serializedObject, ref fPath, ref success);
                    if (success == false)
                    {
                        EditorUtility.DisplayDialog("Error!", "Failure writing to device. " +
                            "at path: " + fPath, "Ok");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Success!", "Completed the operation successfully. " +
                           "Json data has been written into path: " + fPath, "Ok");
                    }
                }
            }


            serializedObject.ApplyModifiedProperties();
        }

        void WriteDataToDevice(SerializedObject obj, ref string savePath, ref bool success)
        {
            success = false;
            //write to disk
            var output = EditorJsonUtility.ToJson(data);
            savePath = EditorUtility.SaveFilePanel("Save json files[DO NOT SAVE INSIDE PROJECT FOLDER]", "", "data.json", ".json");
            if (savePath.Contains(Application.dataPath))
            {
                EditorUtility.DisplayDialog("Error!", "Please set a directory outside project folder " +
                    "because this folder has write permission issues!", "Ok");
                return;
            }
            FileStream stream = null;
            if (!File.Exists(savePath))
            {
                stream = File.Create(savePath);
            }
            File.WriteAllText(savePath, output);
            if (stream != null)
            {
                stream.Dispose();
            }
            obj.Update();
            obj.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            success = true;
        }

        void StopAllEditorCoroutines()
        {
            if (allCORs != null && allCORs.Count > 0)
            {
                foreach (var c in allCORs)
                {
                    if (c == null) { continue; }
                    EditorCoroutineUtility.StopCoroutine(c);
                }
                allCORs.Clear();
                allCORs = new List<EditorCoroutine>();
            }
        }

        private void OnValidate()
        {

        }

        void UpdateHtmlfile()
        {
            var htmlSavepath = EditorUtility.SaveFilePanel("Save html files", "", "tag.html", "html");
            FileStream stream = null;
            if (!File.Exists(htmlSavepath))
            {
                try
                {
                    stream = File.Create(htmlSavepath);
                }
                catch (Exception ex)
                {
                    StopAllEditorCoroutines();
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Error!", "Tag Operation Error!", "Ok");
                    isProcessingTag = false;
                    serializedObject.ApplyModifiedProperties();
                    return;
                }

            }
            var upperTxt = Utility.upperHtml;
            var lowerTxt = Utility.lowerHtml;
            string htmlCode = upperTxt;
            string nl = Environment.NewLine;
            if (data.allTags != null)
            {
                for (int i = 0; i < data.allTags.Length; i++)
                {
                    var t = data.allTags[i];
                    if (t == null) { continue; }
                    if (t.relatedWords == null
                        || t.relatedWords.Length == 0)
                    {
                        //no child
                        htmlCode += "{" + nl +
                            " name: \"" + t.mainTag + "[S]" + "\"" + nl +
                            "}";
                        if (i != data.allTags.Length - 1)
                        {
                            htmlCode += "," + nl;
                        }
                        else
                        {
                            htmlCode += nl;
                        }
                    }
                    else
                    {
                        htmlCode += "{" + nl +
                            " name: \"" + t.mainTag + "\"," + nl +
                            " menu:" + nl +
                            " [";
                        for (int j = 0; j < t.relatedWords.Length; j++)
                        {
                            if (string.IsNullOrEmpty(t.relatedWords[j])) { continue; }
                            htmlCode += "{" + nl +
                           "  name: \"" + t.relatedWords[j] + "\"" + nl +
                           "}";
                            if (j != t.relatedWords.Length - 1)
                            {
                                htmlCode += "," + nl;
                            }
                            else
                            {
                                htmlCode += nl;
                            }
                        }
                        htmlCode += "]" + nl +
                            "}";
                        if (i != data.allTags.Length - 1)
                        {
                            htmlCode += "," + nl;
                        }
                        else
                        {
                            htmlCode += nl;
                        }
                    }
                }
            }

            htmlCode += lowerTxt;

            try
            {
                File.WriteAllText(htmlSavepath, htmlCode);
            }
            catch (Exception)
            {
                StopAllEditorCoroutines();
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Error!", "Tag Operation Error!", "Ok");
                isProcessingTag = false;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (stream != null)
            {
                stream.Dispose();
            }

            EditorUtility.OpenWithDefaultApp(htmlSavepath);
        }

        void UpdateTags(SerializedObject obj)
        {
            //UpdateHtmlfile();
            //obj.Update();
            //obj.ApplyModifiedProperties();
            //isProcessingTag = false;
            //return;
            if (tagDescList != null) { tagDescList.Clear(); }
            tagDescList = new List<TagDesc>();
            List<string> blacklistedTags = new List<string>();
            blacklistedTags.GetAllTags(data.ignoreTags);
            List<string> allTags = new List<string>();
            allTags.GetAllTags(data.allVideos);
            if (allTags.Count > 0)
            {
                allTags.RemoveIfContains(blacklistedTags);
            }
            allTags = allTags.OrderBy(x => x).ToList();

            EditorUtility.ClearProgressBar();
            var cor = EditorCoroutineUtility.StartCoroutine(UpdateTag(obj, allTags, (ser, tagDescList) =>
            {
                if (tagDescList.Count > 0)
                {
                    data.allTags = tagDescList.ToArray();
                }
                else
                {
                    data.allTags = null;
                }
                ser.Update();
                ser.ApplyModifiedProperties();
                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
                Debug.Log("tag done!");
                EditorUtility.DisplayDialog("Success!", "Tags updated", "Ok");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }), this);
            allCORs.Add(cor);
        }

        IEnumerator UpdateTag(SerializedObject ser, List<string> allTags, Action<SerializedObject, List<TagDesc>> OnFinish)
        {
            if (allTags.Count > 0)
            {
                for (int i = 0; i < allTags.Count; i++)
                {
                    progressTag = (float)(i + 1) / (float)allTags.Count;
                    if (string.IsNullOrEmpty(allTags[i]) || procList.HasAny(allTags[i])
                        || allTags[i].IsItNeumeric_YT()) { continue; }
                    procList.Add(allTags[i]);
                    var cor = EditorCoroutineUtility.StartCoroutine(SingleTagProcCOR(allTags, allTags[i],
                     (thisDesc) =>
                     {
                         tagDescList.Add(thisDesc);
                     }), this);
                    allCORs.Add(cor);
                    yield return cor;
                }
            }
            isProcessingTag = false;
            OnFinish?.Invoke(ser, tagDescList);
        }

        IEnumerator SingleTagProcCOR(List<string> allTags, string thisTag, Action<TagDesc> OnFinish)
        {
            SingleTagProc(allTags, thisTag, OnFinish);
            yield return null;
        }

        void SingleTagProc(List<string> allTags, string thisTag, Action<TagDesc> OnFinish)
        {
            var tagDesc = new TagDesc();
            tagDesc.mainTag = thisTag;
            List<string> relList = new List<string>();
            allTags.ForEach((pred) =>
            {
                if (string.IsNullOrEmpty(pred) == false
                && string.Equals(pred, thisTag, StringComparison.OrdinalIgnoreCase) == false
                && procList.HasAny(pred) == false)
                {
                    if (pred.Contains_IgnoreCase(thisTag))
                    {
                        relList.Add(pred);
                        procList.Add(pred);
                    }
                }
            });

            if (relList.Count > 0)
            {
                tagDesc.relatedWords = relList.ToArray();
            }
            else
            {
                tagDesc.relatedWords = null;
            }

            var pList = new List<string>();
            pList.CopyUniqueFrom(procList);
            OnFinish?.Invoke(tagDesc);
        }


        IEnumerator LoopAllYoutubeAPI(List<YoutubeVideo> videos, 
            SerializedObject serObj, Action<List<YoutubeVideo>, SerializedObject> OnFinish)
        {
            if (videos != null)
            {
                for (int i = 0; i < videos.Count; i++)
                {
                    if (videos[i] == null) { continue; }
                    // if (i > 50) { break; }//Test purpose
                    progress = (float)(i + 1) / (float)videos.Count;
                    var cor = EditorCoroutineUtility.StartCoroutine(ProcessVideo(videos[i], i, serObj, (vd, idx) =>
                    {
                        videos[idx] = vd;
                    }),
                    this);
                    allCORs.Add(cor);
                    yield return cor;
                }
            }
            
            serObj.ApplyModifiedProperties();
            OnFinish?.Invoke(videos, serObj);
            isProcessing = false;
        }

        private void OnDisable()
        {
            isProcessing = isProcessingTag = false;
            EditorUtility.ClearProgressBar();
            procList = new List<string>();
            tagDescList = new List<TagDesc>();
            StopAllEditorCoroutines();
        }

        IEnumerator ProcessVideo(YoutubeVideo video, int index, SerializedObject serObj, Action<YoutubeVideo, int> OnFinish)
        {
            string status = "";
            var success = EdUtility.UpdateFromYoutubeDataAPI(ref video, ref status, data.APIKEY);
            if (!success)
            {
                //EditorUtility.DisplayDialog("Error", status, "Got It");
                Debug.Log("<color='red'>error: " + status+"</color>");
            }
            serObj.ApplyModifiedProperties();
            OnFinish?.Invoke(video, index);
            yield return null;
            
        }
    }
}