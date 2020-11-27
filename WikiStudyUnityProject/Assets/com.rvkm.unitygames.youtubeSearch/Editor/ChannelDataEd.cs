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
        static float progress;
        static bool isProcessing;
        static EditorCoroutine currentRunningCOR;

        public void OnEnable()
        {
            data = (SearchDataYoutube)target;
            data.SearchName = data.name;
            isProcessing = false;
        }

        static void ShowArray(SerializedProperty list, bool showListSize = true)
        {
            EditorGUILayout.PropertyField(list, false);
            EditorGUI.indentLevel += 1;
            if (list.isExpanded)
            {
                if (showListSize)
                {
                    EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
                }
                for (int i = 0; i < list.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                }
            }
            EditorGUI.indentLevel -= 1;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label("Search Name: " + data.SearchName);
            GUILayout.Space(10);
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("APIKEY"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InputHtmlFiles"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InputUrls"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ignoreTags"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allTags"), true);
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("allVideos"), true);
            
            if (!isProcessing)
            {
                if (GUILayout.Button(Environment.NewLine + "Generate youtube data" + Environment.NewLine))
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
                    currentRunningCOR = EditorCoroutineUtility.StartCoroutine(LoopAllYoutubeAPI(vList, serializedObject, (procList, ser) =>
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
                        
                    }), this);
                }
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
                if (GUILayout.Button(Environment.NewLine+"Cancel"+Environment.NewLine))
                {
                    EditorCoroutineUtility.StopCoroutine(currentRunningCOR);
                    isProcessing = false;
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Error!", "Operation aborted by user!", "Ok");
                }
            }
            else
            {
                GUILayout.Space(10);
                if (GUILayout.Button(Environment.NewLine+"Update tags"+Environment.NewLine))
                {
                    if (data.IsDataOk() == false)
                    {
                        EditorUtility.DisplayDialog("Error!", "You are trying to update tags but youtube data is invalid! "
                            + Environment.NewLine + "Please generate the data first!", "Ok");
                    }
                    else
                    {
                        UpdateTags(serializedObject);
                    }
                }

                GUILayout.Space(10);
                if (GUILayout.Button(Environment.NewLine + "Write to device(JSON)" + Environment.NewLine))
                {
                    string fPath = "";
                    bool success = false;
                    WriteDataToDevice(serializedObject, ref fPath, ref success);
                    if (success == false)
                    {
                        EditorUtility.DisplayDialog("Error!", "Failure writing to device. check log. " +
                                "You might need to create or set the path first! " +
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
            string dir = Path.Combine(Application.dataPath, "com.rvkm.unitygames.youtubeSearch/Data/Output/");
            savePath = Path.Combine(dir, data.SearchName + "_json.text");

            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (!dirInfo.Exists)
            {
                Directory.CreateDirectory(dir);
            }

            FileStream stream = null;
            if (!File.Exists(savePath))
            {
                //stream = File.Create(fPath);
                stream = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }

            File.WriteAllText(savePath, output);
            if (stream != null) { stream.Dispose(); }
            obj.Update();
            obj.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            success = true;
        }

        private void OnValidate()
        {
            
        }

        void UpdateTags(SerializedObject obj)
        {
            List<string> blacklistedTags = new List<string>();
            List<string> allTags = new List<string>();

            if (data.allVideos != null && data.allVideos.Length > 0)
            {
                foreach (var video in data.allVideos)
                {
                    if (video == null || video.tags == null || video.tags.Length <= 0) { continue; }
                    foreach (var tag in video.tags)
                    {
                        if (string.IsNullOrEmpty(tag)) { continue; }
                        var exists = allTags.Exists((pred) => { return string.Equals(pred, tag, StringComparison.OrdinalIgnoreCase); });
                        if (exists == false)
                        {
                            allTags.Add(tag);
                        }
                    }
                }
            }

            if (allTags.Count > 0)
            {
                List<string> delList = new List<string>();
                blacklistedTags.CopyUniqueFrom<string>(data.ignoreTags);
                if (blacklistedTags != null && blacklistedTags.Count > 0)
                {
                    foreach (var b in blacklistedTags)
                    {
                        if (string.IsNullOrEmpty(b)) { continue; }
                        allTags.ForEach((pred) =>
                        {
                            if (string.Equals(b, pred, StringComparison.OrdinalIgnoreCase))
                            {
                                delList.Add(pred);
                            }
                        });
                    }
                }

                if (delList.Count > 0)
                {
                    foreach (var d in delList)
                    {
                        if (string.IsNullOrEmpty(d)) { continue; }
                        allTags.Remove(d);
                    }
                }
            }
            allTags = allTags.OrderBy(x => x).ToList();
            data.allTags = allTags.ToArray();
            obj.ApplyModifiedProperties();
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

                    yield return EditorCoroutineUtility.StartCoroutine(ProcessVideo(videos[i], i, serObj, (vd, idx) =>
                    {
                        videos[idx] = vd;
                    }),
                    this);
                }
            }
            
            serObj.ApplyModifiedProperties();
            OnFinish?.Invoke(videos, serObj);
            isProcessing = false;
        }

        private void OnDisable()
        {
            isProcessing = false;
            EditorUtility.ClearProgressBar();
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