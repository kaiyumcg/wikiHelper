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
        static SearchDataYoutube data;
        static string errorMsgIfAny;
        public static void InitControl()
        {
            categoryOperationHasCompleted = true;
            categoryOperationProgress = 0f;
            
        }

        public static void Categorize(ref SearchDataYoutube data, SearchDataEditor editor, Action OnComplete, Action<string> OnErrorIfAny)
        {
            categoryOperationHasCompleted = false;
            categoryOperationProgress = 0f;
            errorMsgIfAny = "";
            CategoryControl.data = data;

            if (data == null || data.videoData == null || data.videoData.allVideos == null || data.videoData.allVideos.Length == 0
                || data.categories == null || data.categories.Length == 0)
            {
                categoryOperationHasCompleted = true;
                categoryOperationProgress = 0f;
                OnErrorIfAny?.Invoke("Check the category data and/or video data. Those are invalid!");
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
                List<YoutubeVideo> catVlist = new List<YoutubeVideo>();
                foreach (var video in vList)
                {
                    if (data.categories[i].IsSatisfiedByVideo(video))
                    {
                        catVlist.Add(video);
                        usedFlags[video] = true;
                    }
                }


                if (data.catListType == CategoryListType.Exclusive)
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

                CreateVideoDataWithVideoList(ref data.categories[i], catVlist, data, OnErrorIfAny);
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
                    //create uncat 
                    uncat = new YoutubeCategory() { categoryName = "Uncategorized" };
                }
                CreateVideoDataWithVideoList(ref uncat, notUsedVideos, data, OnErrorIfAny);
                uncat.UpdateStat(OnErrorIfAny);
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


        public static void SortVideos(ref YoutubeCategory category, Action<string> OnErrorIfAny)
        {
            var success = category.Sort(OnErrorIfAny);
        }

        static void CreateVideoDataWithVideoList(ref YoutubeCategory cat, List<YoutubeVideo> catVlist, SearchDataYoutube originalData, 
            Action<string> OnErrorIfAny)
        {
            string dirRel = UrlUtility.GetDataDirRelative(originalData.videoData);
            string assetPathRel = Path.Combine(dirRel, cat.categoryName + "_vData_.asset");
            assetPathRel = assetPathRel.Replace('\\', '/');
            string assetPathAbs = UrlUtility.GetAbsolutePath(assetPathRel);
            if (File.Exists(assetPathAbs) == false)
            {
                //create
                cat.videoData = ScriptableObject.CreateInstance<YoutubeVideoData>();
                AssetDatabase.CreateAsset(data, assetPathRel);
            }
            else
            {
                //load
                cat.videoData = AssetDatabase.LoadAssetAtPath<YoutubeVideoData>(assetPathRel);

            }

            cat.videoData.searchName = originalData.SearchName;
            cat.videoData.belongedCategory = cat.categoryName;
            cat.videoData.allVideos = catVlist.ToArray();
            cat.UpdateStat(OnErrorIfAny);
            cat.Sort(OnErrorIfAny);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}