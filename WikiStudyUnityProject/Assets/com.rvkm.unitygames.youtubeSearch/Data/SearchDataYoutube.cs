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
        [SerializeField] string searchName;
        [SerializeField] TextAsset[] inputHtmlFiles;
        [SerializeField] string[] inputUrls;
        public TextAsset[] InputHtmlFiles { get { return inputHtmlFiles; } }
        public string[] InputUrls { get { return inputUrls; } }
        public string[] allTags;
        public YoutubeVideo[] allVideos;
        public bool fetchedFromLocal = false;
    }
}