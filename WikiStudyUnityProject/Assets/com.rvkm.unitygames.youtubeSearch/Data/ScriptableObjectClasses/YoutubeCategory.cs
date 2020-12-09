using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [CreateAssetMenu(fileName = "New Youtube Category Data", menuName = "Kaiyum/Youtube Caregory Data(V3 API powered)", order = 1)]
    public class YoutubeCategory : ScriptableObject
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
        public CategoryHtmlPrintDesc htmlPrintOptions;
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