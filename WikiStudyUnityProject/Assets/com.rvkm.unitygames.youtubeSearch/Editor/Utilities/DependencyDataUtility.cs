using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class DependencyDataUtility
    {
        public static bool GetDependencyDataIfAny(SearchDataYoutube data, ref YoutubeVideoData ytVideoData, ref YoutubeVideoTags ytTagData)
        {
            if (data.videoData != null && data.tagData != null)
            {
                ytVideoData = data.videoData;
                ytTagData = data.tagData;
                return true;
            }
            else
            {
                string dataDirRelative = UrlUtility.GetDataDirRelative(data);
                var d_info = new DirectoryInfo(UrlUtility.GetAbsolutePath(dataDirRelative));
                DirectoryInfo depDataDir = null;
                SearchAndFindDependencyData(d_info, data.name, ref ytVideoData, ref ytTagData, ref depDataDir);
                if (ytVideoData != null && ytTagData != null)
                {
                    return true;
                }
                else
                {
                    Debug.Log("load failure!");
                    return false;
                }
            }
        }

        static void SearchAndFindDependencyData(DirectoryInfo searchDir, string mainAssetfileName,
            ref YoutubeVideoData ytVideoData, ref YoutubeVideoTags ytTagData, ref DirectoryInfo dataContainingDir)
        {
            if (searchDir == null) { return; }
            var allDirs = searchDir.GetDirectories();
            if (allDirs != null && allDirs.Length > 0)
            {
                foreach (var d in allDirs)
                {
                    if (d == null) { continue; }
                    if (d.Name == mainAssetfileName + "_data_generated_")
                    {
                        Debug.Log("found named dir, let us try to load");
                        var files = d.GetFiles();
                        if (files != null && files.Length > 0)
                        {
                            foreach (var f in files)
                            {
                                if (f == null || f.Extension != ".asset") { continue; }
                                string relPath = UrlUtility.GetRelativePath(f.FullName);
                                var vd = AssetDatabase.LoadAssetAtPath<YoutubeVideoData>(relPath);
                                if (vd != null) { ytVideoData = vd; }
                                var td = AssetDatabase.LoadAssetAtPath<YoutubeVideoTags>(relPath);
                                if (td != null) { ytTagData = td; }
                            }
                        }

                        dataContainingDir = d;
                        break;
                    }
                    SearchAndFindDependencyData(d, mainAssetfileName, ref ytVideoData, ref ytTagData, ref dataContainingDir);
                }
            }
        }

        public static bool CreateFreshDependencyData(SearchDataYoutube data, ref YoutubeVideoData ytVideoData, ref YoutubeVideoTags ytTagData)
        {
            string dataDir = UrlUtility.GetDataDirRelative(data);
            var searchFolder = new DirectoryInfo(UrlUtility.GetAbsolutePath(dataDir));
            DirectoryInfo depDataDirNfo = null;
            SearchAndFindDependencyData(searchFolder, data.name, ref ytVideoData, ref ytTagData, ref depDataDirNfo);

            if (ytVideoData != null && ytTagData != null)
            {
                Debug.Log("No need to create data since it is already there. " +
                    "Code execution should not be here. It should be considered as a bug or 'design flow'. Ignore for now. Process again");
                return false;
            }
            else
            {
                string depDataPath = Path.Combine(dataDir, data.name + "_data_generated_");
                if (depDataDirNfo == null || depDataDirNfo.Exists == false)
                {
                    AssetDatabase.CreateFolder(dataDir, data.name + "_data_generated_");
                    Debug.Log("folder created at: " + depDataPath);
                }

                string ytVideoDataPath = Path.Combine(depDataPath, "ytVideoData.asset");
                ytVideoDataPath = ytVideoDataPath.Replace('\\', '/');
                ytVideoData = ScriptableObject.CreateInstance<YoutubeVideoData>();
                AssetDatabase.CreateAsset(ytVideoData, ytVideoDataPath);

                string ytTagDataPath = Path.Combine(depDataPath, "ytTagData.asset");
                ytTagDataPath = ytTagDataPath.Replace('\\', '/');
                ytTagData = ScriptableObject.CreateInstance<YoutubeVideoTags>();
                AssetDatabase.CreateAsset(ytTagData, ytTagDataPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return true;
            }

        }
    }
}