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
    public class SearchDataEditor : KaiyumScriptableObjectEditor
    {
        SearchDataYoutube data;
        static float progress;
        static bool busy;
        string nl;
        string genBtnStr, jsonBtnStr, tagBtnStr, cancelTagStr, cancelYTStr, genTestStr;

        public override void OnEnableScriptableObject()
        {
            data = (SearchDataYoutube)target;
            nl = Environment.NewLine;
            genBtnStr = nl + "Generate youtube data" + nl;
            jsonBtnStr = nl + "Write to device(JSON)" + nl;
            tagBtnStr = nl + "Update tags" + nl;
            cancelTagStr = nl + "Cancel Tag Update" + nl;
            cancelYTStr = nl + "Cancel" + nl;
            genTestStr = nl + "Test Scr" + nl;
            TagControl.InitControl();
            YouTubeControl.InitControl();
        }

        public override void OnDisableScriptableObject()
        {
            busy = false;
            EditorUtility.ClearProgressBar();
        }

        public override void OnUpdateScriptableObject()
        {
            busy = false;
            if (TagControl.TagFetchOperationHasCompleted == false || YouTubeControl.YoutubeAPIOperationHasCompleted == false)
            {
                busy = true;
                if (TagControl.TagFetchOperationHasCompleted == false) { progress = TagControl.TagFetchOperationProgress; }
                else if (YouTubeControl.YoutubeAPIOperationHasCompleted== false) { progress = YouTubeControl.YoutubeAPIOperationProgress; }
            }

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SearchName"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("APIKEY"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InputHtmlFiles"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InputUrls"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("videoData"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tagData"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("printTagsInHtml"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("printCategoriesInHtml"), true);
            //dynamically see what are the fields search data has and show them. 

            if (TagControl.TagFetchOperationHasCompleted == false)
            {
                GUILayout.BeginHorizontal("box");
                progress = (float)Math.Round((double)progress, 4);
                GUILayout.Label("Tag Progress: ");
                GUILayout.Button("" + (100 * progress) + "% ");
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                if (GUILayout.Button(cancelTagStr))
                {
                    StopAllEditorCoroutines();
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Error!", "Tag Operation aborted by user!", "Ok");
                    busy = false;
                }
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (busy)
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
                    busy = false;
                }
            }
            else
            {
                if (GUILayout.Button(genTestStr))
                {
                    if (false)
                    {
                        EditorUtility.DisplayDialog("Error!", "You are trying to update tags but youtube data is invalid! "
                            + Environment.NewLine + "Please generate the data first!", "Ok");
                    }
                    else
                    {
                        YoutubeVideoData vData = null;
                        YoutubeVideoTags tData = null;
                        var dataObtained = DependencyDataUtility.GetDependencyDataIfAny(data, ref vData, ref tData);
                        if (dataObtained)
                        {
                            bool willDownload = vData.IsDataOk();

                        }
                        else
                        {
                            bool createNew = EditorUtility.DisplayDialog("----Choice----", "There is no dependency data for this '" + data.SearchName + "' search. "
                                 + nl + " You can manually assign them or you can create one!", "Create them and start fresh", "Abort now to assign them");
                            if (createNew)
                            {
                                var creationSuccess = DependencyDataUtility.CreateFreshDependencyData(data, ref vData, ref tData);
                                if (!creationSuccess)
                                {
                                    EditorUtility.DisplayDialog("Error", "Could not create dependency data. Check the log!", "Ok");
                                    return;
                                }
                                else
                                {

                                    Debug.Log("successfully created! vData null? " + (vData == null) + " and tData null? " + (tData == null));
                                }
                            }
                        }
                        if (vData == null || tData == null) { return; }
                        data.videoData = vData;
                        data.tagData = tData;
                        var vSer = new SerializedObject(vData);
                        vSer.Update();
                        var tSer = new SerializedObject(tData);
                        tSer.Update();

                        List<YoutubeVideo> vList = new List<YoutubeVideo>();
                        if (data.allVideos != null && data.allVideos.Length > 0)
                        {
                            foreach (var v in data.allVideos)
                            {
                                if (v == null) { continue; }
                                vList.Add(v);
                            }
                        }

                        List<TagDesc> allTagList = new List<TagDesc>();
                        if (data.allTags != null && data.allTags.Length > 0)
                        {
                            foreach (var t in data.allTags)
                            {
                                if (t == null) { continue; }
                                allTagList.Add(t);
                            }
                        }

                        List<TagDesc> allIgnoreTagList = new List<TagDesc>();
                        if (data.ignoreTags != null && data.ignoreTags.Length > 0)
                        {
                            foreach (var t in data.ignoreTags)
                            {
                                if (t == null) { continue; }
                                allIgnoreTagList.Add(t);
                            }
                        }

                        vData.searchName = data.SearchName;
                        vData.allVideos = vList.ToArray();
                        tData.searchName = data.SearchName;
                        tData.allTags = allTagList.ToArray();
                        tData.ignoreTags = allIgnoreTagList.ToArray();

                        data.videoData = vData;
                        data.tagData = tData;
                        vSer.ApplyModifiedProperties();
                        tSer.ApplyModifiedProperties();
                        ChannelDataEditorUtility.SaveYoutubeDataToDisk(serializedObject, data);
                    }
                }

                if (GUILayout.Button(genBtnStr))
                {
                    busy = true;
                    EditorUtility.DisplayProgressBar("Processing", "Reading html files and/or textasset containing html files", 0);
                    List<YoutubeVideo> vList = new List<YoutubeVideo>();
                    vList.CopyUniqueFrom(data.allVideos);
                    if (data.InputHtmlFiles != null)
                    {
                        foreach (var txtAsset in data.InputHtmlFiles)
                        {
                            vList.CopyUniqueFrom(HtmlNodeUtility.GetAllVideoInfo(txtAsset.text));
                        }
                    }

                    if (data.InputUrls != null)
                    {
                        foreach (var url in data.InputUrls)
                        {
                            if (!UrlUtility.IsUrl(url)) { continue; }
                            vList.CopyUniqueFrom(HtmlNodeUtility.GetAllVideoInfo(ChannelDataEditorUtility.GetWWWResponse(url)));
                        }
                    }

                    EditorUtility.ClearProgressBar();
                    var cor = EditorCoroutineUtility.StartCoroutine(YouTubeControl.LoopAllYoutubeAPI(vList, data.APIKEY, this, (procList) =>
                    {
                        data.allVideos = procList.ToArray();
                        TagControl.UpdateTags(serializedObject, data, this);
                        EditorUtility.ClearProgressBar();
                        serializedObject.Update();
                        ChannelDataEditorUtility.SaveYoutubeDataToDisk(serializedObject, data);

                    }), this);
                    AllCoroutines.Add(cor);
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
                        busy = true;
                        TagControl.UpdateTags(serializedObject, data, this);
                    }
                }
                if (GUILayout.Button(jsonBtnStr))
                {
                    ChannelDataEditorUtility.SaveYoutubeDataToDisk(serializedObject, data);
                }
            }

            EditorUtility.SetDirty(data);
            EditorUtility.SetDirty(data.videoData);
            EditorUtility.SetDirty(data.tagData);
            serializedObject.ApplyModifiedProperties();
        }
    }
}