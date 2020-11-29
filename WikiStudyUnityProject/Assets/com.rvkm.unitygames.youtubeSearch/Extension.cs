using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class Extension
    {
        public static bool HasAny(this List<string> strList, string str)
        {
            return strList.Exists((s) => { return string.Equals(s, str, StringComparison.CurrentCultureIgnoreCase); });
        }

        public static bool HasAny(this string[] strList, string str)
        {
            bool hasAny = false;
            foreach (var s in strList)
            {
                if (string.IsNullOrEmpty(s)) { continue; }
                if (string.Equals(s, str, StringComparison.CurrentCultureIgnoreCase))
                {
                    hasAny = true;
                    break;
                }
            }
            return hasAny;
        }

        public static bool Contains_IgnoreCase(this string st, string str)
        {
            return st?.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool Contains_OnAnyElement_IgnoreCase(this List<string> strList, string str)
        {
            bool contains = false;
            foreach (var s in strList)
            {
                if (string.IsNullOrEmpty(s)) { continue; }
                if (s.Contains_IgnoreCase(str))
                {
                    contains = true;
                    break;
                }
            }
            return contains;
        }

        public static bool Contains_IgnoreCase(this string[] strList, string str)
        {
            bool contains = false;
            foreach (var s in strList)
            {
                if (string.IsNullOrEmpty(s)) { continue; }
                if (s.Contains_IgnoreCase(str))
                {
                    contains = true;
                    break;
                }
            }
            return contains;
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

        public static void RemoveIfContains(this List<string> thisList, List<string> toRemove)
        {
            if (toRemove != null && toRemove.Count > 0)
            {
                foreach (var d in toRemove)
                {
                    if (d == null) { continue; }
                    if (thisList.Contains_OnAnyElement_IgnoreCase(d))
                    {
                        thisList.Remove(d);
                    }
                }
            }
        }

        public static void GetAllTags(this List<string> vList, TagDesc[] vListToCopy)
        {
            if (vListToCopy != null && vListToCopy.Length > 0)
            {
                foreach (var v in vListToCopy)
                {
                    if (v == null) { continue; }
                    if (!vList.Contains_OnAnyElement_IgnoreCase(v.mainTag))
                    {
                        vList.Add(v.mainTag);
                    }
                    if (v.relatedWords != null && v.relatedWords.Length > 0)
                    {
                        foreach (var r in v.relatedWords)
                        {
                            if (string.IsNullOrEmpty(r)) { continue; }
                            if (!vList.Contains_OnAnyElement_IgnoreCase(r))
                            {
                                vList.Add(r);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If 'str' is not null or empty, this string will be updated with 'str' and 'valueUpdated' will be true
        /// </summary>
        /// <param name="host"></param>
        /// <param name="str"></param>
        /// <param name="valueUpdated"></param>
        public static void IsIfNullOrEmpty(this string host, string str, ref bool valueUpdated)
        {
            if (string.IsNullOrEmpty(str) == false)
            {
                host = str;
                valueUpdated = true;
            }
        }

        /// <summary>
        /// Will get value from the string if conversion is successful and 'valueUpdated' will be true in such case.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="str"></param>
        /// <param name="valueUpdated"></param>
        public static void TryGetFromString(this int host, string str, ref bool valueUpdated)
        {
            if (string.IsNullOrEmpty(str) == false)
            {;
                int count = 0;
                var success = int.TryParse(str, out count);
                if (success)
                {
                    host = count;
                    valueUpdated = true;
                }
            }
        }

        public static void GetAllTags(this List<string> vList, YoutubeVideo[] vListToCopy)
        {
            if (vListToCopy != null && vListToCopy.Length > 0)
            {
                foreach (var v in vListToCopy)
                {
                    if (v == null || v.tags == null || v.tags.Length == 0) { continue; }
                    foreach (var tag in v.tags)
                    {
                        if (string.IsNullOrEmpty(tag)) { continue; }
                        if (!vList.Contains_OnAnyElement_IgnoreCase(tag))
                        {
                            vList.Add(tag);
                        }
                    }
                }
            }
        }
    }
}