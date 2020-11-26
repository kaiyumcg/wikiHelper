using com.rvkm.unitygames.YouTube.YTAPI_Internal_Video;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public class VideoInfoCon
    {
        public static YoutubeVideo FetchVideoInfo(string videoID, string API_KEY)
        {
            string url = "https://youtube.googleapis.com/youtube/v3/videos?part=snippet&id=" + videoID + "&key=" + API_KEY;
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                var op = www.SendWebRequest();
                yield return op;
                if (www.isNetworkError)
                {
                    //error
                }
                else
                {
                    Debug.Log("<color='green'>got response from api to get video info, lets perse the json.</color>");
                    string json = www.downloadHandler.text;
                    //VideoInfoFromYTDataAPI data = null;
                    //try
                    //{
                    //    data = JsonUtility.FromJson<VideoInfoFromYTDataAPI>(json);
                    //}
                    //catch (Exception ex)
                    //{
                    //    //error
                    //}

                    //var snippet = GetSnippet(data);
                    //if (snippet == null)
                    //{
                    //    //error
                    //}
                    //else
                    //{
                    //    /*
                    //        string title = thisData.snippet.title;
                    //           string description = thisData.snippet.description;
                    //           string channelTitle = thisData.snippet.channelTitle;
                    //           string pubDate = thisData.snippet.publishedAt;
                    //           DateTime publishedDatetime = new DateTime(1980, 1, 1, 1, 1, 1, 1);
                    //           DateTime.TryParse(pubDate, out publishedDatetime);
                    //           int publishedYear = publishedDatetime.Year;
                    //           int publishedMonth = publishedDatetime.Month;

                    //        */

                    //}
                }
            }
        }

        Snippet GetSnippet(VideoInfoFromYtDataApi data)
        {
            Snippet result = null;
            //if (data != null && data.Items != null && data.Items.Length > 0)
            //{
            //    foreach (var i in data.Items)
            //    {
            //        if (i == null) { continue; }
            //        if (i.Snippet != null)
            //        {
            //            result = i.Snippet;
            //            break;
            //        }
            //    }
            //}
            return result;
        }
    }
}