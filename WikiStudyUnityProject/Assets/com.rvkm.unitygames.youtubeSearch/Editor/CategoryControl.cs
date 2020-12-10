using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class CategoryControl
    {
        public static bool categoryOperationHasCompleted { get; private set; }
        public static float categoryOperationProgress { get; private set; }
        public static void InitControl()
        {
            categoryOperationHasCompleted = true;
            categoryOperationProgress = 0f;
            
        }

        public static void Categorize(ref SearchDataYoutube data, SearchDataEditor editor)
        {
            categoryOperationHasCompleted = false;
            categoryOperationProgress = 0f;

            if (data == null || data.videoData == null || data.videoData.allVideos == null || data.videoData.allVideos.Length == 0
                || data.categories == null || data.categories.Length == 0)
            {
                categoryOperationHasCompleted = true;
                categoryOperationProgress = 0f;
            }

            List<YoutubeVideo> vList = new List<YoutubeVideo>();
            foreach (var video in data.videoData.allVideos)
            {
                if (video == null) { continue; }
                var nVideo = new YoutubeVideo(video);
                vList.Add(nVideo);
            }



            Dictionary<YoutubeVideo, bool> usedFlags = new Dictionary<YoutubeVideo, bool>();
            foreach (var video in vList)
            {
                usedFlags.Add(video, false);
            }

            for (int i = 0; i < data.categories.Length; i++)
            {
                if (data.categories[i] == null || data.categories[i].categoryName == "Uncategorized") { continue; }
                if (data.categories[i].videoData != null && data.categories[i].videoData.allVideos != null && data.categories[i].videoData.allVideos.Length > 0)
                {
                    data.categories[i].videoData.allVideos = null;
                }

                List<YoutubeVideo> catVlist = new List<YoutubeVideo>();
                foreach (var video in vList)
                {
                    if (data.categories[i].IsSatisfiedByVideo(video))
                    {
                        catVlist.Add(video);
                        usedFlags[video] = true;
                    }
                }

                if (data.categoryProcessType == CategoryListType.Exclusive)
                {
                    Dictionary<YoutubeVideo, bool> delList = new Dictionary<YoutubeVideo, bool>();
                    foreach (var p in usedFlags)
                    {
                        if (p.Value == true)
                        {
                            delList.Add(p.Key, p.Value);
                        }
                    }

                    foreach (var p in delList)
                    {
                        vList.Remove(p.Key);
                        usedFlags.Remove(p.Key);
                    }
                }

                CreateOrLoadVideoDataWithVideoList(ref data.categories[i], catVlist, data);
            }

            int notUsedCount = 0;
            List<YoutubeVideo> notUsedVideos = new List<YoutubeVideo>();
            if (usedFlags != null && usedFlags.Count > 0)
            {
                foreach (var d in usedFlags)
                {
                    if (d.Value == false)
                    {
                        notUsedCount++;
                        if (notUsedVideos.Contains(d.Key) == false)
                        {
                            notUsedVideos.Add(d.Key);
                        }
                    }
                }
            }

            YoutubeCategory uncat = null;
            foreach (var cat in data.categories)
            {
                if (cat == null) { continue; }
                if (cat.categoryName == "Uncategorized")
                {
                    uncat = cat;
                    break;
                }
            }

            if (notUsedCount > 0 && notUsedVideos != null && notUsedVideos.Count > 0)
            {
                
                if (uncat == null)
                {
                    Debug.Log("we need to create it!");
                    //uncat = new YoutubeCategory() { categoryName = "Uncategorized" };

                    string mainDataDirRel = UrlUtility.GetDataDirRelative(data);
                    string dependencyDataDirRel = Path.Combine(mainDataDirRel, data.name + "_data_generated_");
                    string dependencyDataDirRelAbs = UrlUtility.GetAbsolutePath(dependencyDataDirRel);
                    
                    if (Directory.Exists(dependencyDataDirRelAbs) == false)
                    {
                        AssetDatabase.CreateFolder(mainDataDirRel, data.name + "_data_generated_");
                        Debug.Log("folder created at: " + dependencyDataDirRel);
                    }

                    string uncatDataPathRel = Path.Combine(dependencyDataDirRel, "_uncat_generated.asset");
                    uncatDataPathRel = uncatDataPathRel.Replace('\\', '/');
                    uncat = ScriptableObject.CreateInstance<YoutubeCategory>();
                    uncat.categoryName = "Uncategorized";
                    AssetDatabase.CreateAsset(uncat, uncatDataPathRel);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                CreateOrLoadVideoDataWithVideoList(ref uncat, notUsedVideos, data);
                uncat.UpdateStat();
                List<YoutubeCategory> cList = new List<YoutubeCategory>();
                foreach (var c in data.categories) 
                {
                    if (c.categoryName == "Uncategorized") { continue; }
                    cList.Add(c);
                }
                cList.Add(uncat);
                data.categories = cList.ToArray();
                
            }
            else
            {
                for (int i = 0; i < data.categories.Length; i++)
                {
                    if (data.categories[i] == null) { continue; }
                    if (data.categories[i].categoryName == "Uncategorized")
                    {
                        data.categories[i] = null;
                        break;
                    }
                }
            }
            categoryOperationHasCompleted = true;
        }
        static void CreateOrLoadVideoDataWithVideoList(ref YoutubeCategory cat, List<YoutubeVideo> catVlist, SearchDataYoutube originalData)
        {
            string dirRel = UrlUtility.GetDataDirRelative(originalData.videoData);
            string catName = cat.categoryName;
            catName = catName.Replace(" ", "_");
            string assetPathRel = Path.Combine(dirRel, catName + "_vData_.asset");
            assetPathRel = assetPathRel.Replace('\\', '/');
            string assetPathAbs = UrlUtility.GetAbsolutePath(assetPathRel);
            if (File.Exists(assetPathAbs) == false)
            {
                //create
                cat.videoData = ScriptableObject.CreateInstance<YoutubeVideoData>();
                AssetDatabase.CreateAsset(cat.videoData, assetPathRel);
            }
            else
            {
                //load
                cat.videoData = AssetDatabase.LoadAssetAtPath<YoutubeVideoData>(assetPathRel);

            }

            cat.videoData.searchName = originalData.SearchName;
            cat.videoData.belongedCategory = cat.categoryName;
            cat.videoData.allVideos = catVlist.ToArray();
            cat.UpdateStat();
            cat.Sort();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}