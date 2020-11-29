using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class UrlUtility
    {
        public static bool IsUrl(string str)
        {
            return (!string.IsNullOrEmpty(str)) && (str.Contains("http://") || str.Contains("https://"));
        }

        public static string GetRelativePath(string absolutePath)
        {
            absolutePath = absolutePath.Replace('\\', '/');
            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }
            else
            {
                return absolutePath;
            }
        }

        public static string GetAbsolutePath(string relativePath)
        {
            relativePath = GetRelativePath(relativePath);
            relativePath = relativePath.Replace('/', '\\');
            FileAttributes attr = File.GetAttributes(relativePath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryInfo nfo = new DirectoryInfo(relativePath);
                return nfo.FullName;
            }
            else
            {
                FileInfo nfo = new FileInfo(relativePath);
                return nfo.FullName;
            }
        }

        public static string GetDataDirRelative(ScriptableObject obj)
        {
            string mainAssetPath = AssetDatabase.GetAssetPath(obj);
            FileInfo nfo = new FileInfo(mainAssetPath);
            return GetRelativePath(nfo.Directory.FullName);
        }
    }
}