using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using com.rvkm.unitygames.YouTubeSearch.Extensions;
using System.Linq;

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

        bool MatchesInTheList(string[] strList, string str, bool caseSensitive = false)
        {
            bool hasAny = false;
            if (strList != null)
            {
                for (int i = 0; i < strList.Length; i++)
                {
                    if (string.IsNullOrEmpty(strList[i])) { continue; }
                    if (caseSensitive)
                    {
                        if (strList[i] == str) { hasAny = true; break; }
                    }
                    else
                    {
                        if (string.Equals(strList[i], str, StringComparison.OrdinalIgnoreCase)) { hasAny = true; break; }
                    }
                }
            }
            
            return hasAny;
        }

        bool ContainsInTheList(string[] strList, string str, bool caseSensitive = false)
        {
            bool contains = false;
            if (strList != null)
            {
                for (int i = 0; i < strList.Length; i++)
                {
                    if (string.IsNullOrEmpty(strList[i])) { continue; }
                    if (caseSensitive)
                    {
                        if (str.Contains(strList[i])) { contains = true; break; }
                    }
                    else
                    {
                        if (str.Contains_IgnoreCase(strList[i])) { contains = true; break; }
                    }
                }
            }
            return contains;
        }

        public bool IsPassed(string str)
        {
            if (!use || (!useBlacklist && !useWhitelist)) { return true; }
            bool passed = false;
            if (string.IsNullOrEmpty(str) == false && (useBlacklist || useWhitelist))
            {
                if (useBlacklist)
                {
                    var blacklist = Utility.SplitByComaOrNewline(blacklistedWords);
                    passed = compMode == StrSearchComp.ContainsOnAnyParts ? ContainsInTheList(blacklist, str, caseSensitive) : MatchesInTheList(blacklist, str, caseSensitive);
                }

                if (useWhitelist)
                {
                    var whitelist = Utility.SplitByComaOrNewline(whitelistedWords);
                    passed = compMode == StrSearchComp.ContainsOnAnyParts ? ContainsInTheList(whitelist, str, caseSensitive) : MatchesInTheList(whitelist, str, caseSensitive);
                }
            }

            return passed;
        }


        public bool IsPassed(string[] strs)
        {
            if (!use || (!useBlacklist && !useWhitelist)) { return true; }
            bool isPassed = false;
            if (strs != null && strs.Length > 0)
            {
                foreach (var str in strs)
                {
                    if (string.IsNullOrEmpty(str)) { continue; }
                    if (IsPassed(str)) 
                    {
                        isPassed = true;
                        break;
                    }
                }
            }

            return isPassed;
        }
    }

    public enum StrSearchComp { ContainsOnAnyParts = 0, ExactMatch = 1 }
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
            if (!use) { return true; }
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
            if (!use) { return true; }
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

        public bool IsSatisfiedByVideo(YoutubeVideo video)
        {
            bool titlePass = titleOp.IsPassed(video.title);
            bool descPass = descriptionOp.IsPassed(video.description);
            bool tagsPass = tagOp.IsPassed(video.tags);
            bool viewCountPass = viewCountOp.IsPassed(video.viewCount);
            bool likeCountPass = likeCountOp.IsPassed(video.likeCount);
            bool dislikeCountPass = dislikeCountOp.IsPassed(video.dislikeCount);
            bool commentCountPass = commentCountOp.IsPassed(video.commentCount);
            bool durationPass = durationOp.IsPassed(video.durationInMinutes);
            //if (video.durationInMinutes > 2)
            //{
            //    Debug.Log("bag valluk!");
            //}
            bool pubDatePass = pubDateOp.IsPassed(video.publishedAtDate);
            //Debug.Log("titlePass? " + titlePass + " and descPass? " + descPass + " and tagsPass? " + tagsPass + " and viewCountPass? " + viewCountPass +
            //    " and likeCountPass? " + likeCountPass + " and dislikeCountPass? " + dislikeCountPass + " and commentCountPass? " + commentCountPass +
            //    " and durationPass? " + durationPass + " and pubDatePass? " + pubDatePass);
            return titlePass && descPass && tagsPass && viewCountPass && likeCountPass && dislikeCountPass && commentCountPass && durationPass && pubDatePass;
        }

        public void UpdateStat()
        {
            totalMinutes = 0f;
            totalVideoCount = averageVideoDuration = medianVideoDuration = frequentVideoDuration = 0;
            List<int> allDuration = new List<int>();
            if (videoData != null && videoData.allVideos != null)
            {
                foreach (var video in videoData.allVideos)
                {
                    if (video == null) { continue; }
                    totalVideoCount++;
                    totalMinutes += video.durationInMinutes;
                    allDuration.Add(video.durationInMinutes);
                }
            }

            averageVideoDuration = (int)((float)totalMinutes / (float)totalVideoCount);
            medianVideoDuration = (int)allDuration.GetMedian();
            frequentVideoDuration = (int)allDuration.GetMode();
        }

        public void Sort()
        {
            List<YoutubeVideo> vdList = new List<YoutubeVideo>();
            vdList.AddRange(videoData.allVideos);

            if (sortMode == CategorySortMode.SortAlphabetically)
            {
                videoData.allVideos = AscendingOrder ? vdList.OrderBy(v => v.title).ToArray() : vdList.OrderByDescending(v => v.title).ToArray();
            }
            else if (sortMode == CategorySortMode.SortByCommentCount)
            {
                videoData.allVideos = AscendingOrder ? vdList.OrderBy(v => v.commentCount).ToArray() : vdList.OrderByDescending(v => v.commentCount).ToArray();
            }
            else if (sortMode == CategorySortMode.SortByDislikeCount)
            {
                videoData.allVideos = AscendingOrder ? vdList.OrderBy(v => v.dislikeCount).ToArray() : vdList.OrderByDescending(v => v.dislikeCount).ToArray();
            }
            else if (sortMode == CategorySortMode.SortByDuration)
            {
                videoData.allVideos = AscendingOrder ? vdList.OrderBy(v => v.durationInMinutes).ToArray() : vdList.OrderByDescending(v => v.durationInMinutes).ToArray();
            }
            else if (sortMode == CategorySortMode.SortByLikeCount)
            {
                videoData.allVideos = AscendingOrder ? vdList.OrderBy(v => v.likeCount).ToArray() : vdList.OrderByDescending(v => v.likeCount).ToArray();
            }
            else if (sortMode == CategorySortMode.SortByPublishedDate)
            {
                videoData.allVideos = AscendingOrder ? vdList.OrderBy(v => v.publishedAtDate).ToArray() : vdList.OrderByDescending(v => v.publishedAtDate).ToArray();
            }
            else if (sortMode == CategorySortMode.SortByViewCount)
            {
                videoData.allVideos = AscendingOrder ? vdList.OrderBy(v => v.viewCount).ToArray() : vdList.OrderByDescending(v => v.viewCount).ToArray();
            }
        }
    }
}