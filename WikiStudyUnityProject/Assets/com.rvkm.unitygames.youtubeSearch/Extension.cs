using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class Extension
    {
        public static bool ContainsCaseInsensitive(this List<string> st, string str)
        {
            return st.Exists((s) => { return string.Equals(s, str, StringComparison.CurrentCultureIgnoreCase); });
        }

        public static bool ContainsCaseInsensitive(this string st, string str)
        {

            return st?.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool ContainsAnyOf(this string st, List<string> strList)
        {
            bool contains = false;
            if (strList != null && strList.Count > 0)
            {
                for (int i = 0; i < strList.Count; i++)
                {
                    if (string.IsNullOrEmpty(strList[i])) { continue; }
                    if (st.ContainsCaseInsensitive(strList[i])) { contains = true; break; }
                }
            }
            return contains;
        }

        public static bool IsItNeumeric(this string st)
        {
            if (st.Contains(":"))
            {
                st = st.Replace(":", "");
            }
            var isIt = int.TryParse(st, out _);
            return isIt;
        }

        public static void CopyUniqueFrom(this List<YoutubeVideo> vList, List<YoutubeVideo> vListToCopy)
        {
            if (vListToCopy != null && vListToCopy.Count > 0)
            {
                foreach (var v in vListToCopy)
                {
                    if (v == null) { continue; }
                    bool exists = vList.Exists((pred) => { return string.Equals(pred.url, v.url); });
                    if (!exists)
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
                    bool exists = vList.Exists((pred) => { return string.Equals(pred.url, v.url); });
                    if (!exists)
                    {
                        vList.Add(v);
                    }
                }
            }
        }

        public static void CopyUniqueFrom<T>(this List<T> vList, List<T> vListToCopy)
        {
            if (vListToCopy != null && vListToCopy.Count > 0)
            {
                foreach (var v in vListToCopy)
                {
                    if (v == null) { continue; }
                    bool exists = vList.Exists((pred) => { return pred.Equals(v); });
                    if (!exists)
                    {
                        vList.Add(v);
                    }
                }
            }
        }

        public static void CopyUniqueFrom<T>(this List<T> vList, T[] vListToCopy)
        {
            if (vListToCopy != null && vListToCopy.Length > 0)
            {
                foreach (var v in vListToCopy)
                {
                    if (v == null) { continue; }
                    bool exists = vList.Exists((pred) => { return pred.Equals(v); });
                    if (!exists)
                    {
                        vList.Add(v);
                    }
                }
            }
        }
    }
}