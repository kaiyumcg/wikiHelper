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
        public string APIKEY;
        public string SearchName;
        public TextAsset[] InputHtmlFiles;
        public string[] InputUrls;
        public string[] allTags;
        public YoutubeVideo[] allVideos;
        public string[] ignoreTags;

        public bool IsDataOk()
        {
            bool isOk = true;
            if (allVideos != null && allVideos.Length > 0)
            {
                foreach (var v in allVideos)
                {
                    if (v == null || v.YouTubeDataAPI_Cooked == false)
                    {
                        isOk = false;
                        break;
                    }
                }
            }
            else
            {
                isOk = false;
            }
            return isOk;
        }
    }
}