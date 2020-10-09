using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Returns with suffix removed, if present
        /// </summary>
        public static string TrimUrlSlashesFromEnd(
            this string value)
        {
            while (value.EndsWith(@"\") || value.EndsWith("/"))
            {
                value = value.Substring(0, value.Length - 1);
            }
            return value;
        }

        /// <summary>
        /// Returns with suffix removed, if present
        /// </summary>
        public static string TrimUrlSlashesFromStart(
            this string value)
        {
            while (value.StartsWith(@"\") || value.StartsWith("/"))
            {
                value = value.Substring(1, value.Length - 1);
            }
            return value;
        }
    }
}