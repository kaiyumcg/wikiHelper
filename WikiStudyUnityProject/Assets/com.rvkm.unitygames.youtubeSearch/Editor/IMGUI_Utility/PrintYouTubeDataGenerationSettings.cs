using com.rvkm.unitygames.YouTubeSearch.Extensions;
using com.rvkm.unitygames.YouTubeSearch.HtmlPrinter;
using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class PrintYouTubeDataGenerationSettings
    {
        static string nl;
        public static void Print(ref SearchDataYoutube mainData, SearchDataEditor editor, SerializedObject serializedObject)
        {
            SearchDataYoutube data = mainData;
            if (string.IsNullOrEmpty(nl)) { nl = Environment.NewLine; }
            if (GUILayout.Button(nl + "Generate youtube data" + nl))
            {
                editor.anyButtonClicked = true;
                YoutubeVideoData vData = null;
                YoutubeVideoTags tData = null;
                var dataObtained = DependencyDataUtility.GetDependencyDataIfAny(data, ref vData, ref tData);
                if (!dataObtained)
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
                    }
                }
                if (vData == null || tData == null) { return; }
                data.videoData = vData;
                data.tagData = tData;
                data.videoData.searchName = data.SearchName;
                data.tagData.searchName = data.SearchName;
                var vSer = new SerializedObject(data.videoData);
                vSer.Update();
                var tSer = new SerializedObject(data.tagData);
                tSer.Update();

                EditorUtility.DisplayProgressBar("Processing", "Reading html files and/or textasset containing html files", 0);
                List<YoutubeVideo> vList = new List<YoutubeVideo>();
                vList.CopyUniqueFrom(data.videoData.allVideos);
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
                        vList.CopyUniqueFrom(HtmlNodeUtility.GetAllVideoInfo(Utility.GetWWWResponse(url)));
                    }
                }

                EditorUtility.ClearProgressBar();
                var cor = EditorCoroutineUtility.StartCoroutine(YouTubeControl.LoopAllYoutubeAPI(vList, data.APIKEY, editor, data, (procList) =>
                {
                    data.videoData.allVideos = procList.ToArray();
                }), editor);
                editor.AllCoroutines.Add(cor);
                vSer.ApplyModifiedProperties();
                tSer.ApplyModifiedProperties();
                ChannelDataEditorUtility.SaveYoutubeDataToDisk(serializedObject, data);
            }
        }
    }
}