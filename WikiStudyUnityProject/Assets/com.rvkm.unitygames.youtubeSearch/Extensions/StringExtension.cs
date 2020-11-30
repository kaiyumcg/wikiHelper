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

           //  return strList.Exists((s) => { return string.Equals(s, str, StringComparison.CurrentCultureIgnoreCase); });
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

        public static void RemoveSimilar(this List<string> thisList, List<string> similarStr)
        {
            if (similarStr != null && similarStr.Count > 0)
            {
                foreach (var sm in similarStr)
                {
                    if (string.IsNullOrEmpty(sm)) { continue; }
                    bool matchFound = false;
                    string toRemove = "";
                    foreach (var t in thisList)
                    {
                        if (string.IsNullOrEmpty(t)) { continue; }
                        if ((t.ToLower()).Contains(sm.ToLower()))
                        {
                            matchFound = true;
                            toRemove = t;
                            break;
                        }
                    }

                    if (matchFound && string.IsNullOrEmpty(toRemove) == false)
                    {
                        thisList.Remove(sm);
                    }
                }
            }
        }
    }
}