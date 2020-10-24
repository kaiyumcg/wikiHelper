using com.rvkm.unitygames.debug;
using com.rvkm.unitygames.extensions;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace com.rvkm.unitygames.wiki
{
    public static class UrlUtility
    {
        public static string FormatWikiUrlCommon(string url)
        {
            string r_url = Regex.Replace(url, "http://", "");
            r_url = Regex.Replace(r_url, "https://", "");
            r_url = Regex.Replace(r_url, "en.wikipedia.org", "");
            r_url = Regex.Replace(r_url, "en.m.wikipedia.org", "");
            r_url = r_url.TrimUrlSlashesFromStart();
            r_url = r_url.TrimUrlSlashesFromEnd();
            return r_url;
        }

        public static string GetFullWikiUrlForOpenWWW(string url)
        {
            bool isMobile = false;
#if UNITY_IOS || UNITY_ANDROID
        isMobile = true;
#endif
            string mainDomainFull = isMobile ? "https://en.m.wikipedia.org" : "https://en.wikipedia.org";
            string formattedUrl = url;

            mainDomainFull = mainDomainFull.TrimUrlSlashesFromEnd();
            formattedUrl = formattedUrl.TrimUrlSlashesFromStart();
            formattedUrl = formattedUrl.TrimUrlSlashesFromEnd();


            //Utility.LogYellow("main domain url: " + mainDomainFull + " and formatted url: " + formattedUrl);
            string newUrl = mainDomainFull + "/" + formattedUrl; //Path.Combine(mainDomainFull, formattedUrl);
            DebugRVKM.LogYellow("the full wiki url is: " + newUrl);
            return newUrl;
        }

        public static bool IsUrlWiki(string url)
        {
            return url.Contains("wiki/");
        }
    }
}