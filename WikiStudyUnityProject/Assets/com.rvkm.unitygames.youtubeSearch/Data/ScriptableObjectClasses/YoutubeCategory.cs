using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [CreateAssetMenu(fileName = "New Youtube Category Data", menuName = "Kaiyum/Youtube Caregory Data(V3 API powered)", order = 2)]
    public class YoutubeCategory : ScriptableObject
    {
        public string categoryName;
        public bool showUI, StrOpShow, IntOpShow, DurationAndDateOpShow, OutputOptionShow;
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
        public CategoryHtmlPrintDesc htmlPrintOptions;
        public bool IsSatisfiedByVideo(YoutubeVideo video)
        {
            bool titleWhitelistFail, titleBlacklistFail, descriptionWhitelistFail,
                descriptionBlacklistFail, tagWhitelistFail, tagBlacklistFail;
            titleWhitelistFail = titleBlacklistFail = descriptionWhitelistFail = descriptionBlacklistFail
                = tagWhitelistFail = tagBlacklistFail = false;
            bool titlePass = titleOp.IsPassed(video.title, ref titleBlacklistFail, ref titleWhitelistFail);
            bool descPass = descriptionOp.IsPassed(video.description, ref descriptionBlacklistFail, ref descriptionWhitelistFail);
            bool tagsPass = tagOp.IsPassed(video.tags, ref tagBlacklistFail, ref tagWhitelistFail);
            bool viewCountPass = viewCountOp.IsPassed(video.viewCount);
            bool likeCountPass = likeCountOp.IsPassed(video.likeCount);
            bool dislikeCountPass = dislikeCountOp.IsPassed(video.dislikeCount);
            bool commentCountPass = commentCountOp.IsPassed(video.commentCount);
            bool durationPass = durationOp.IsPassed(video.durationInMinutes);
            bool pubDatePass = pubDateOp.IsPassed(video.publishedAtDate);
            bool isAnyPass = false;
            if (titleOp.use && titlePass) { isAnyPass = true; }
            if (descriptionOp.use && descPass) { isAnyPass = true; }
            if (tagOp.use && tagsPass) { isAnyPass = true; }
            if (viewCountOp.use && viewCountPass) { isAnyPass = true; }
            if (likeCountOp.use && likeCountPass) { isAnyPass = true; }
            if (dislikeCountOp.use && dislikeCountPass) { isAnyPass = true; }
            if (commentCountOp.use && commentCountPass) { isAnyPass = true; }
            if (durationOp.use && durationPass) { isAnyPass = true; }
            if (pubDateOp.use && pubDatePass) { isAnyPass = true; }
            if (titleWhitelistFail || titleBlacklistFail || descriptionWhitelistFail || descriptionBlacklistFail
                || tagWhitelistFail || tagBlacklistFail)
            {
                isAnyPass = false;
            }
            return isAnyPass;
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

            if (allDuration != null && allDuration.Count > 0)
            {
                averageVideoDuration = (int)((float)totalMinutes / (float)totalVideoCount);
                medianVideoDuration = (int)allDuration.GetMedian();
                frequentVideoDuration = (int)allDuration.GetMode();
            }
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
            else if (sortMode == CategorySortMode.SoryByDurationDateAlphabatically)
            {
                videoData.allVideos = AscendingOrder ? vdList.OrderBy(v => v.durationInMinutes).ThenBy(v => v.publishedAtDate).ThenBy(v => v.title).ToArray()
                    : vdList.OrderByDescending(v => v.durationInMinutes).ThenByDescending(v => v.publishedAtDate).ThenByDescending(v => v.title).ToArray();
            }
        }
    }
}