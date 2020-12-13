using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using com.rvkm.unitygames.YouTubeSearch.Extensions;
using System.Linq;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public class StringAndCaseDesc
    {
        public string str;
        public bool caseSensitive, matchExactPhraseOrSentence,useDeepSearch;
    }

    [System.Serializable]
    public class StringSearchOp
    {
        public bool use;
        public TextAsset[] defAssets;

        bool IsRelatableWithAnyInList(string str, StringAndCaseDesc[] strList)
        {
            bool relatable = false;
            if (strList != null)
            {
                for (int i = 0; i < strList.Length; i++)
                {
                    if (strList[i] == null || string.IsNullOrEmpty(strList[i].str)) { continue; }
                    bool isIt = str.IsRelatableWith(strList[i].str, strList[i].caseSensitive,
                        strList[i].matchExactPhraseOrSentence, strList[i].useDeepSearch);
                    if (isIt)
                    {
                        relatable = true;
                        break;
                    }
                }
            }
            return relatable;
        }

        public bool IsPassed(string str, ref bool blacklistFailure, ref bool whitelistFailure)
        {
            if (defAssets == null || defAssets.Length == 0) { return true; }
            bool passed = false;
            if (string.IsNullOrEmpty(str) == false)
            {
                string strSoup = "";
                foreach (var a in defAssets)
                {
                    if (a == null) { continue; }
                    strSoup += a.text + Environment.NewLine;
                }
                StringAndCaseDesc[] blacklist = null, whitelist = null, blacklist2 = null, whitelist2 = null;
                Utility.GetBlackWhitelist(strSoup, ref blacklist, ref whitelist, ref blacklist2, ref whitelist2);

                //order of execution is important!
                if (whitelist != null && whitelist.Length > 0)
                {
                    if (IsRelatableWithAnyInList(str, whitelist))
                    {
                        passed = true;   
                    }
                }

                if (blacklist != null && blacklist.Length > 0)
                {
                    if (IsRelatableWithAnyInList(str, blacklist))
                    {
                        passed = false;
                        blacklistFailure = true;
                    }
                }

                if (passed)
                {
                    if (whitelist2 != null && whitelist2.Length > 0)
                    {
                        if (!IsRelatableWithAnyInList(str, whitelist2))
                        {
                            passed = false;
                            whitelistFailure = true;
                        }
                    }
                }

                if (blacklist2 != null && blacklist2.Length > 0)
                {
                    if (IsRelatableWithAnyInList(str, blacklist2))
                    {
                        passed = false;
                        blacklistFailure = true;
                    }
                }
            }

            return passed;
        }


        public bool IsPassed(string[] strs, ref bool failedDueToFoundBlacklist, ref bool failedDueToNotFoundInWhitelist)
        {
            if (defAssets == null || defAssets.Length == 0) { return true; }
            bool isPassed = false;
            if (strs != null && strs.Length > 0)
            {
                foreach (var str in strs)
                {
                    if (string.IsNullOrEmpty(str)) { continue; }
                    if (IsPassed(str, ref failedDueToFoundBlacklist, ref failedDueToNotFoundInWhitelist)) 
                    {
                        isPassed = true;
                        break;
                    }
                }
            }

            return isPassed;
        }
    }

    public enum IntSearchComp { Equal = 0, LessthanOrEqual = 1, GreaterthanOrEqual = 2, Greaterthan = 3, Lessthan = 4, NotEqual = 5 }
    public enum IntSearchCompMode { SingleTarget = 0, InBetween = 1}
    public enum IntSearchBetweenMode { OutsideRange = 0, InsideRange = 1, EitherExtreme = 2, MinExtremeSide = 3, MaxExtremeSide = 4 }

    [System.Serializable]
    public class IntSearchOp
    {
        public bool use;
        public IntSearchComp comparison;
        public IntSearchCompMode mode;
        public IntSearchBetweenMode betweenMode;
        public int target, targetMin, targetMax;

        public bool IsPassed(int Value)
        {
            bool passed = false;

            if (mode == IntSearchCompMode.SingleTarget)
            {
                if (comparison == IntSearchComp.Equal) { passed = Value == target; }
                else if (comparison == IntSearchComp.LessthanOrEqual) { passed = Value <= target; }
                else if (comparison == IntSearchComp.GreaterthanOrEqual) { passed = Value >= target; }
                else if (comparison == IntSearchComp.Greaterthan) { passed = Value > target; }
                else if (comparison == IntSearchComp.Lessthan) { passed = Value < target; }
                else if (comparison == IntSearchComp.NotEqual) { passed = Value != target; }
            }
            else
            {
                if (betweenMode == IntSearchBetweenMode.EitherExtreme) { passed = Value == targetMin || Value == targetMax; }
                else if (betweenMode == IntSearchBetweenMode.InsideRange) { passed = Value > targetMin && Value < targetMax; }
                else if (betweenMode == IntSearchBetweenMode.MaxExtremeSide) { passed = Value == targetMax; }
                else if (betweenMode == IntSearchBetweenMode.MinExtremeSide) { passed = Value == targetMin; }
                else if (betweenMode == IntSearchBetweenMode.OutsideRange) { passed = !(Value > targetMin && Value < targetMax); }
            }
            
            return passed;
        }
    }

    public enum DateSearchScaleComp { Days = 0, Weeks = 1, Months = 2, Years = 3 }
    public enum DateSearchMode { TheLast = 0, OlderThan = 1, Between = 2, OutSideRange = 3 }
    [System.Serializable]
    public class DateSearchOp
    {
        public bool use;
        public float target;
        public OurDateTime startDate, endDate;
        public DateSearchScaleComp scale;
        public DateSearchMode mode;
        
        public bool IsPassed(long dateTick)
        {
            bool passed = false;
            if (dateTick > 0)
            {
                DateTime pubDate = new DateTime(dateTick);
                DateTime targetAgoTime = default;
                double tValue = (double)target;

                if (mode == DateSearchMode.OlderThan || mode == DateSearchMode.TheLast)
                {
                    if (scale == DateSearchScaleComp.Days)
                    {
                        targetAgoTime = DateTime.UtcNow.AddDays(-tValue);
                    }
                    else if (scale == DateSearchScaleComp.Weeks)
                    {
                        targetAgoTime = DateTime.UtcNow.AddDays(-tValue * 7d);
                    }
                    else if (scale == DateSearchScaleComp.Months)
                    {
                        targetAgoTime = DateTime.UtcNow.AddDays(-tValue * 30d);//by design
                    }
                    else
                    {
                        targetAgoTime = DateTime.UtcNow.AddDays(-tValue * 365d);//by design
                    }
                }
                if (mode == DateSearchMode.TheLast)
                {
                    if (pubDate > targetAgoTime)
                    {
                        passed = true;
                    }
                }
                else if (mode == DateSearchMode.OlderThan)
                {
                    if (pubDate < targetAgoTime)
                    {
                        passed = true;
                    }
                }
                else if (mode == DateSearchMode.Between)
                {
                    DateTime endDt = new DateTime(endDate.year, endDate.month, endDate.day);
                    DateTime startDt = new DateTime(startDate.year, startDate.month, startDate.day);
                    if (pubDate > startDt && pubDate < endDt)
                    {
                        passed = true;
                    }
                }
                else
                {
                    DateTime endDt = new DateTime(endDate.year, endDate.month, endDate.day);
                    DateTime startDt = new DateTime(startDate.year, startDate.month, startDate.day);
                    if (!(pubDate > startDt && pubDate < endDt))
                    {
                        passed = true;
                    }
                }
            }
            

            return passed;
        }
    }

    [System.Serializable]
    public class OurDateTime
    {
        public int year, month, day;
    }

    public enum CategorySortMode { SortAlphabetically = 0, 
        SortByDuration = 1, SortByPublishedDate = 2, SortByViewCount = 3,
        SortByLikeCount = 4, SortByDislikeCount = 5, SortByCommentCount = 6,
        SoryByDurationDateAlphabatically = 7}

    [System.Serializable]
    public class CategoryHtmlPrintDesc
    {
        public bool publishedYearInHtml, durationInHtml, showThumbnailInHtml, viewCountInHtml, likeCountInHtml, dislikeCountInHtml, commentCountInHtml;
        public CategoryHtmlPrintDesc()
        {
            publishedYearInHtml = durationInHtml = showThumbnailInHtml = viewCountInHtml = likeCountInHtml = dislikeCountInHtml = commentCountInHtml = true;
            showThumbnailInHtml = false;
        }
    }
}