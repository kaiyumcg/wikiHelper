using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [System.Serializable]
    public class StringSearchOp
    {
        public bool use;
        public bool caseSensitive;
        public bool useWhitelist, useBlacklist;
        public StrSearchComp compMode;
        public string blacklistedWords, whitelistedWords;
        [HideInInspector]
        public Vector2 scrollBlacklistUI, scrollWhitelistUI;
    }

    public enum StrSearchComp { Match = 0, Contains = 1 }
    public enum IntSearchComp { Equal = 0, LessthanOrEqual = 1, GreaterthanOrEqual = 2, Greaterthan = 3, Lessthan = 4, NotEqual = 5 }
    [System.Serializable]
    public class IntSearchOp
    {
        public bool use;
        public IntSearchComp comparison;
        public int target;
    }

    public enum DateSearchScaleComp { Days = 0, Weeks = 1, Months = 2, Years = 3 }
    public enum DateSearchMode { TheLast = 0, OlderThan = 1, Between = 2 }
    [System.Serializable]
    public class DateSearchOp
    {
        public bool use;
        public float target;
        public OurDateTime targetMin, targetMax;
        public DateSearchScaleComp scale;
        public DateSearchMode mode;
    }

    [System.Serializable]
    public class OurDateTime
    {
        public int year, month, day;
    }

    public enum CategorySortMode { SortAlphabetically = 0, SortByDuration = 1, SortByPublishedDate = 2, SortByViewCount = 3, SortByLikeCount = 4, SortByDislikeCount = 5, SortByCommentCount = 6 }
    [System.Serializable]
    public class YoutubeCategory
    {
        public string categoryName;
        public bool showUI;
        public StringSearchOp titleOp = null, descriptionOp = null, tagOp = null;
        public IntSearchOp viewCountOp = null, likeCountOp = null, dislikeCountOp = null, commentCountOp = null, durationOp = null;
        public DateSearchOp pubDateOp = null;
        [ReadOnly]
        public YoutubeVideoData videoData = null;
        [ReadOnly]
        public float totalMinutes;
        [ReadOnly]
        public int totalVideoCount;
        [ReadOnly]
        public int averageVideoDuration, medianVideoDuration, frequentVideoDuration;
        public CategorySortMode sortMode;
        public bool AscendingOrder;
    }
}