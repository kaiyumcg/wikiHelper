using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [CreateAssetMenu(fileName = "New Youtube Search Data", menuName = "Kaiyum/Youtube Search Data(V3 API powered)", order = 1)]
    public class SearchDataYoutube : ScriptableObject
    {
        [Multiline]
        public string APIKEY;
        public string SearchName;
        public TextAsset[] InputHtmlFiles;
        public string[] InputUrls;
        public YoutubeVideoData videoData;
        public YoutubeVideoTags tagData;
        public TagSearchDescription blacklist, mustUseList;
        public YoutubeCategory[] categories;
        public bool showTagSetting, showCategorySetting, showAllCategoryOutputUI;
        public float textAreaSizeUI;
        public bool publishedYearInHtml, durationInHtml, showThumbnailInHtml, viewCountInHtml, likeCountInHtml, dislikeCountInHtml, commentCountInHtml;
        public CategoryListType categoryProcessType;
        public bool dataGenerationTestMode;
        public int dataGenerationCountForTestMode;
        public bool forceUpdateForGeneration;
    }
    public enum CategoryListType { Associative, Exclusive }
}