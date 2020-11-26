using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeV2
{
    [System.Serializable]
    public class UrlData
    {
        public string linkName, url;

        public UrlData(string linkName, string url)
        {
            this.linkName = linkName;
            this.url = url;
        }

        public UrlData(UrlData data)
        {
            this.linkName = data.linkName;
            this.url = data.url;
        }
    }

    [System.Serializable]
    public class Category
    {
        public string categoryName;
        public List<string> blacklistedWords, whitelistedWords;
        public List<UrlData> videos;
    }
}