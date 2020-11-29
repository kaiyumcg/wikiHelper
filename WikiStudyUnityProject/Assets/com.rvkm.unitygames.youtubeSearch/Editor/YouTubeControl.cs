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
        public static bool YoutubeAPIOperationHasCompleted { get { return completed; } }
        public static float YoutubeAPIOperationProgress { get { return progress; } }
        static bool completed;
        static float progress;
        static string APIKEY;
        public static void InitControl()
        {
            completed = true;
            progress = 0f;
        }

        public static IEnumerator LoopAllYoutubeAPI(List<YoutubeVideo> videos, string APIKEY, SearchDataEditor editor, Action<List<YoutubeVideo>> OnFinish)
        {
            YouTubeControl.APIKEY = APIKEY;
            completed = false;
            if (videos != null)
            {
                for (int i = 0; i < videos.Count; i++)
                {
                    if (videos[i] == null) { continue; }
                    // if (i > 50) { break; }//Test purpose
                    progress = (float)(i + 1) / (float)videos.Count;
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
            completed = true;
        }

        static IEnumerator ProcessVideo(YoutubeVideo video, int index, Action<YoutubeVideo, int> OnFinish)
        {
            string status = "";
            var success = YoutubeDataAPI.UpdateFromYoutubeDataAPI(ref video, ref status, APIKEY);
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