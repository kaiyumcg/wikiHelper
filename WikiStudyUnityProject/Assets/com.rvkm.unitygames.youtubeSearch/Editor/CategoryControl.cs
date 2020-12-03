using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class CategoryControl
    {
        public static bool categoryOperationHasCompleted { get; private set; }
        public static float categoryOperationProgress { get; private set; }
        static YoutubeVideoData videoData;
        static YoutubeCategory[] cats;
        public static void InitControl()
        {
            categoryOperationHasCompleted = true;
            categoryOperationProgress = 0f;
            
        }

        public static void Categorize(ref YoutubeCategory[] catList, YoutubeVideoData videoData, SearchDataEditor editor, Action OnComplete)
        {
            categoryOperationHasCompleted = false;
            CategoryControl.videoData = videoData;
            CategoryControl.cats = catList;

            throw new System.NotImplementedException(); //TODO
        }

        public static void SortVideos(ref YoutubeVideo[] videos, ref string errorMsgIfAny, CategorySortMode sortMode, Action OnErrorIfAny)
        {
            throw new System.NotImplementedException(); //TODO
        }
    }
}