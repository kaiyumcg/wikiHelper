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
    }
}