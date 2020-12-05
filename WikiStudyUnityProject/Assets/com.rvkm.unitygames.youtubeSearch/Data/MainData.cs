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
        //[HideInInspector]
        public int durationInMinutes;
        [HideInInspector]
        public long publishedAtDate;
        public OurDateTime publisdedDate;
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
            this.channelName = data.channelName;
            this.description = data.description;
            this.thumbUrls = data.thumbUrls;
            this.tags = data.tags;
            this.viewCount = data.viewCount;
            this.likeCount = data.likeCount;
            this.dislikeCount = data.dislikeCount;
            this.commentCount = data.commentCount;
            this.durationInMinutes = data.durationInMinutes;
            this.publishedAtDate = data.publishedAtDate;

            DateTime dt = new DateTime(data.publishedAtDate);
            this.publisdedDate = new OurDateTime() { year = dt.Year, month = dt.Month, day = dt.Day };
            this.YouTubeDataAPI_Cooked = data.YouTubeDataAPI_Cooked;
        }
    }
}