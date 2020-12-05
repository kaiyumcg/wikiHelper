using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch.Extensions
{
    public static class StringExtension
    {
        public static bool HasAny_IgnoreCase(this List<string> strList, string str)
        {
            bool hasAny = false;
            for (int i = 0; i < strList.Count; i++)
            {
                if (string.IsNullOrEmpty(strList[i])) { continue; }
                if (string.Equals(strList[i], str, StringComparison.OrdinalIgnoreCase)) { hasAny = true; break; }
            }
            return hasAny;
        }

        public static bool Contains_IgnoreCase(this string st, string str)
        {
            var st_lower = st.ToLower();
            var str_lower = str.ToLower();
            return st_lower.Contains(str_lower);
        }

        public static void CopyUniqueTagsFrom(this List<string> vList, List<string> vListToCopy)
        {
            if (vListToCopy != null && vListToCopy.Count > 0)
            {
                foreach (var v in vListToCopy)
                {
                    if (v == null) { continue; }
                    if (!vList.HasAny_IgnoreCase(v))
                    {
                        vList.Add(v);
                    }
                }
            }
        }

        public static List<string> RemoveBlacklistedTags(this List<string> thisList, List<string> blackList)
        {
            List<string> tags = new List<string>();
            if (thisList != null && thisList.Count > 0)
            {
                foreach (var tag in thisList)
                {
                    if (string.IsNullOrEmpty(tag)) { continue; }
                    bool hasBlacklistedWord = false;
                    foreach (var bl in blackList)
                    {
                        if (string.IsNullOrEmpty(bl)) { continue; }
                        if (tag.Contains_IgnoreCase(bl))
                        {
                            hasBlacklistedWord = true;
                            break;
                        }
                    }

                    if (hasBlacklistedWord == false)
                    {
                        tags.Add(tag);
                    }
                }
            }
            return tags;
        }
    }
}