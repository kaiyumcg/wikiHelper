using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using com.rvkm.unitygames.YouTubeSearch.Extensions;

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

        public bool IsPassed(string str)
        {
            bool passed = false;
            if (use && (useBlacklist || useWhitelist))
            {
                if (useBlacklist)
                {
                    var blacklist = Utility.SplitByComaOrNewline(blacklistedWords);
                    passed = compMode == StrSearchComp.ContainsOnAnyParts ? blacklist.ContainsOnItems(str, caseSensitive) : blacklist.HasAnyOnItems(str, caseSensitive);
                }

                if (useWhitelist)
                {
                    var whitelist = Utility.SplitByComaOrNewline(whitelistedWords);
                    passed = compMode == StrSearchComp.ContainsOnAnyParts ? whitelist.ContainsOnItems(str, caseSensitive) : whitelist.HasAnyOnItems(str, caseSensitive);
                }
            }

            return passed;
        }
    }

    public enum StrSearchComp { ContainsOnAnyParts = 0, Match = 1 }
    public enum IntSearchComp { Equal = 0, LessthanOrEqual = 1, GreaterthanOrEqual = 2, Greaterthan = 3, Lessthan = 4, NotEqual = 5 }
    [System.Serializable]
    public class IntSearchOp
    {
        public bool use;
        public IntSearchComp comparison;
        public int target;

        public bool IsPassed(int Value)
        {
            bool passed = false;
            if (use)
            {
                if (comparison == IntSearchComp.Equal) { passed = Value == target; }
                else if (comparison == IntSearchComp.LessthanOrEqual) { passed = Value <= target; }
                else if (comparison == IntSearchComp.GreaterthanOrEqual) { passed = Value >= target; }
                else if (comparison == IntSearchComp.Greaterthan) { passed = Value > target; }
                else if (comparison == IntSearchComp.Lessthan) { passed = Value < target; }
                else if (comparison == IntSearchComp.NotEqual) { passed = Value != target; }
            }
            return passed;
        }
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

        public bool IsPassed(long dateTick)
        {
            bool passed = false;
            DateTime pubDate = new DateTime(dateTick);
            if (mode == DateSearchMode.TheLast)
            {
                DateTime agoDt = default;
                if (scale == DateSearchScaleComp.Days)
                {
                    agoDt = DateTime.UtcNow.AddDays(-(double)target);
                }
                else if (scale == DateSearchScaleComp.Weeks)
                {
                    var days = target * 7;
                    agoDt = DateTime.UtcNow.AddDays(-(double)days);
                }
                else if (scale == DateSearchScaleComp.Months)
                {
                    agoDt = DateTime.UtcNow.AddMonths(-(int)target);
                }
                else
                {
                    agoDt = DateTime.UtcNow.AddYears(-(int)target);
                }
                //2.5 years will give us 2 year! bug!
            }

            return passed;
        }
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
        public bool showUI, StrOpShow, IntOpShow, DurationAndDateOpShow, SortOpShow, OutputOptionShow;
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