using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch.Extensions
{
    public static class VideoExtension
    {
        public static bool HasAnyHaveLink(this List<YoutubeVideo> vList, string link)
        {
            bool hasAny = false;
            foreach (var video in vList)
            {
                if (video == null || string.IsNullOrEmpty(video.url)) { continue; }
                if (string.Equals(video.url, link)) { hasAny = true; break; }
            }

            return hasAny;
        }

        public static void CopyUniqueFrom(this List<YoutubeVideo> vList, List<YoutubeVideo> vListToCopy)
        {
            if (vListToCopy != null && vListToCopy.Count > 0)
            {
                foreach (var v in vListToCopy)
                {
                    if (v == null) { continue; }
                    if (!vList.HasAnyHaveLink(v.url))
                    {
                        vList.Add(v);
                    }
                }
            }
        }

        public static void CopyUniqueFrom(this List<YoutubeVideo> vList, YoutubeVideo[] vListToCopy)
        {
            if (vListToCopy != null && vListToCopy.Length > 0)
            {
                foreach (var v in vListToCopy)
                {
                    if (v == null) { continue; }
                    if (!vList.HasAnyHaveLink(v.url))
                    {
                        vList.Add(v);
                    }
                }
            }
        }

        public static bool IsItNeumeric_YT(this string st)
        {
            if (st.Contains(":"))
            {
                st = st.Replace(":", "");
            }
            if (st.Contains("_"))
            {
                st = st.Replace("_", "");
            }
            if (st.Contains("-"))
            {
                st = st.Replace("-", "");
            }
            if (st.Contains("."))
            {
                st = st.Replace(".", "");
            }
            var isIt = int.TryParse(st, out _);
            return isIt;
        }
    }
}