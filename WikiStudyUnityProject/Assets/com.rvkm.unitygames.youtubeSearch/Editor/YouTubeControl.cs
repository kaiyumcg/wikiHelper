using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class YouTubeControl
    {
        public static bool YoutubeAPIOperationHasCompleted { get; private set; }
        public static float YoutubeAPIOperationProgress { get; private set; }
        static string APIKEY;
        static bool forceUpdate;
        public static void InitControl()
        {
            YoutubeAPIOperationHasCompleted = true;
            YoutubeAPIOperationProgress = 0f;
        }

        public static IEnumerator LoopAllYoutubeAPI(List<YoutubeVideo> videos, string APIKEY, SearchDataEditor editor, SearchDataYoutube data, Action<List<YoutubeVideo>> OnFinish)
        {
            YouTubeControl.forceUpdate = data.forceUpdateForGeneration;
            YouTubeControl.APIKEY = APIKEY;
            YoutubeAPIOperationHasCompleted = false;
            if (videos != null)
            {
                for (int i = 0; i < videos.Count; i++)
                {
                    string videoID = Utility.GetVideoId(videos[i].url);
                    if (videoID != "yqCHiZrgKzs") { continue; }

                    if (data.dataGenerationTestMode)
                    {
                        if (i > data.dataGenerationCountForTestMode) { continue; }//for test
                    }
                    
                    if (videos[i] == null) { continue; }
                    YoutubeAPIOperationProgress = (float)(i + 1) / (float)videos.Count;
                    var cor = EditorCoroutineUtility.StartCoroutine(ProcessVideo(videos[i], i, (vd, idx) =>
                    {
                        videos[idx] = vd;
                    }),
                    editor);
                    editor.AllCoroutines.Add(cor);
                    yield return cor;
                }
            }
            OnFinish?.Invoke(videos);
            YoutubeAPIOperationHasCompleted = true;
        }

        static IEnumerator ProcessVideo(YoutubeVideo video, int index, Action<YoutubeVideo, int> OnFinish)
        {
            string status = "";
            var success = YoutubeDataAPI.UpdateFromYoutubeDataAPI(ref video, ref status, APIKEY, forceUpdate);
            if (!success)
            {
                EditorUtility.DisplayDialog("Error", status, "Got It");
                Debug.Log("<color='red'>error: " + status + "</color>");
            }
            OnFinish?.Invoke(video, index);
            yield return null;
        }
    }
}