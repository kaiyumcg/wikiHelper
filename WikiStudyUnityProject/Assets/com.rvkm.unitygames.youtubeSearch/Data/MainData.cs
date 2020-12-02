using com.rvkm.unitygames.YouTubeSearch.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [System.Serializable]
    public class YoutubeVideo
    {
        public string title, url;
        [HideInInspector]
        public string channelName, description;
        [HideInInspector]
        public string[] thumbUrls, tags;
        [HideInInspector]
        public int viewCount, likeCount, dislikeCount, commentCount;
        [HideInInspector]
        public int durationInMinutes;
        [HideInInspector]
        public long publishedAtDate;
        public bool YouTubeDataAPI_Cooked;

        public YoutubeVideo(string url)
        {
            this.url = url;
            this.YouTubeDataAPI_Cooked = false;
        }

        public YoutubeVideo(YoutubeVideo data)
        {
            this.title = data.title;
            this.url = data.url;
        }
    }
}