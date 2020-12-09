using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility
{
    public static class IMGUIStatics
    {
        public static GUIContent useTitle, useDescription, useTags, useBlacklist, useWhitelist, categoryName, stringComparisonMode, isCaseSensitive,
            willShowUI, useViewCount, useLikeCount, useDislikeCount, useCommentCount, useDuration, useDateOperation, videoData, totalMinutes, 
            sortMode, IsAscendingOrder, totalVideoCount, averageDuration, mediationDuration, frequentDuration,
            printPublishedYearInHtml, printDurationInHtml, printViewCountInHtml, printLikeCountInHtml, printDislikeCountInHtml, printCommentCountInHtml,
            printThumbnailInHtml, categoryListType, useTextArea, useAssetFile;
        public static void CreateGUIContents()
        {
            useTextArea = new GUIContent("Use TextArea? ");
            useAssetFile = new GUIContent("Use TextAsset? ");
            useTitle = new GUIContent("Use title? ");
            useDescription = new GUIContent("Use Description? ");
            useTags = new GUIContent("Use YouTube Video tags? ");
            useBlacklist = new GUIContent("Use blacklist? ");
            useWhitelist = new GUIContent("Use whitelist? ");
            useViewCount = new GUIContent("Use view count? ");
            useLikeCount = new GUIContent("Use like count? ");
            useDislikeCount = new GUIContent("Use dislike count? ");
            useCommentCount = new GUIContent("Use comment count? ");
            useDuration = new GUIContent("Use duration? ");
            useDateOperation = new GUIContent("Use Date Operation? ");
            categoryName = new GUIContent("Category Name: ");
            videoData = new GUIContent("Video data: ");
            totalMinutes = new GUIContent("Total Minutes: ");
            sortMode = new GUIContent("Sort Mode: ");
            totalVideoCount = new GUIContent("Total Video Count: ");
            IsAscendingOrder = new GUIContent("Is Ascending Order? ");
            stringComparisonMode = new GUIContent("String Comparison Mode ");
            isCaseSensitive = new GUIContent("Case Sensitive Operation? ");
            willShowUI = new GUIContent("Show UI? ");
            averageDuration = new GUIContent("Average Duration: ");
            mediationDuration = new GUIContent("Median Duration: ");
            frequentDuration = new GUIContent("Frequent Duration: ");
            printPublishedYearInHtml = new GUIContent("Print Published Year? ");
            printDurationInHtml = new GUIContent("Print Duration? ");
            printViewCountInHtml = new GUIContent("Print View Count? ");
            printLikeCountInHtml = new GUIContent("Print Like Count? ");
            printDislikeCountInHtml = new GUIContent("Print Dislike Count? ");
            printCommentCountInHtml = new GUIContent("Print Comment Count? ");
            printThumbnailInHtml = new GUIContent("Show Thumbnail? ");
            categoryListType = new GUIContent("Category list type: ");
        }
    }
}