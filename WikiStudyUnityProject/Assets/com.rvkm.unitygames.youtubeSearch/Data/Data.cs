using System.Collections;
using System.Collections.Generic;
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
        [HideInInspector]
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

    [System.Serializable]
    public class StringSearchOp
    {
        public bool useWhitelist, useBlacklist;
        public List<string> blacklistedWords, whitelistedWords;
    }

    public enum IntSearchComp { Equal, LessthanOrEqual, GreaterthanOrEqual, Greaterthan, Lessthan, NotEqual}
    [System.Serializable]
    public class IntSearchOp
    {
        public int sValue;
        public IntSearchComp comparison;
    }

    public enum DateSearchComp { Days, Months, Years}
    [System.Serializable]
    public class DateSearchOp
    {
        public float sValue;
        public DateSearchComp comparison;
    }

    [System.Serializable]
    public class YoutubeCategory
    {
        public string categoryName;
        public bool useTitle = true, useDescription = false, useViewCount = false, 
            useLikeCount = false, useDislikeCount = false, useCommentCount = false
            , useDuration = false, usePublishedDate = false, useTags = false;
        public StringSearchOp titleOp = null, descriptionOp = null, tagOp = null;
        public IntSearchOp viewCountOp = null, likeCountOp = null, dislikeCountOp = null, commentCountOp = null, durationOp = null;
        public DateSearchOp pubDateOp = null;
        public List<YoutubeVideo> videos = null;
    }
}