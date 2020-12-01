using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using System.Linq;
using com.rvkm.unitygames.YouTubeSearch.Extensions;
using UnityEngine;
using System.Text.RegularExpressions;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class TagControl
    {
        static List<string> procList = new List<string>();
        static List<TagDesc> tagDescList = new List<TagDesc>();
        public static bool TagFetchOperationHasCompleted { get; private set; }
        public static float TagFetchOperationProgress { get; private set; }
        static int opCount;
        static double singleOpTimeMax;
        public static void InitControl()
        {
            TagFetchOperationHasCompleted = true;
            TagFetchOperationProgress = 0f;
            procList = new List<string>();
            tagDescList = new List<TagDesc>();
            opCount = 0;
            singleOpTimeMax = 0D;
        }

        static List<string> GetAllTags(YoutubeVideo[] videos)
        {
            List<string> tags = new List<string>();
            if (videos != null && videos.Length > 0)
            {
                foreach (var video in videos)
                {
                    if (video == null || video.tags == null || video.tags.Length == 0) { continue; }
                    foreach (var tag in video.tags)
                    {
                        if (string.IsNullOrEmpty(tag)) { continue; }
                        if (!tags.HasAny_IgnoreCase(tag))
                        {
                            tags.Add(tag);
                        }
                    }
                }
            }
            return tags;
        }

        static List<string> GetAllTags(TagDesc[] tagData)
        {
            List<string> tags = new List<string>();
            if (tagData != null && tagData.Length > 0)
            {
                foreach (var tag in tagData)
                {
                    if (tag == null) { continue; }
                    if (!tags.HasAny_IgnoreCase(tag.mainTag))
                    {
                        tags.Add(tag.mainTag);
                    }
                    if (tag.relatedWords != null && tag.relatedWords.Length > 0)
                    {
                        foreach (var r in tag.relatedWords)
                        {
                            if (string.IsNullOrEmpty(r)) { continue; }
                            if (!tags.HasAny_IgnoreCase(r))
                            {
                                tags.Add(r);
                            }
                        }
                    }
                }
            }
            return tags;
        }

        public static void UpdateTags(SearchDataYoutube searchData, SearchDataEditor editor, Action<List<TagDesc>> OnComplete)
        {
            opCount = 0;
            singleOpTimeMax = 0D;
            var t1 = DateTime.Now;
            TagFetchOperationHasCompleted = false;
            TagFetchOperationProgress = 0f;
            procList = new List<string>();
            tagDescList = new List<TagDesc>();
            List<string> blacklistedTags = searchData.blacklist.GetAllTagWords();
            Debug.Log("blacklisted tag operation took: " + ((DateTime.Now - t1).TotalSeconds) + " seconds.");
            List<string> allTags = new List<string>();
            allTags = GetAllTags(searchData.videoData.allVideos);
            Debug.Log("get all tag operation took: " + ((DateTime.Now - t1).TotalSeconds) + " seconds.");
            if (allTags.Count > 0 && blacklistedTags.Count > 0)
            {
                allTags = allTags.RemoveBlacklistedTags(blacklistedTags);

                Debug.Log("remove duplicate operation took: " + ((DateTime.Now - t1).TotalSeconds) + " seconds.");
            }

            if (searchData.mustUseList.IsValid())
            {
                List<string> chosenTags = new List<string>();
                var mustHaveWords = searchData.mustUseList.GetAllTagWords();
                foreach (var tag in allTags)
                {
                    if (string.IsNullOrEmpty(tag)) { continue; }
                    if (mustHaveWords != null && mustHaveWords.Count > 0)
                    {
                        foreach (var thisMustBeContained in mustHaveWords)
                        {
                            if (string.IsNullOrEmpty(thisMustBeContained)) { continue; }
                            if (tag.Contains_IgnoreCase(thisMustBeContained))
                            {
                                chosenTags.Add(tag);
                                break;
                            }
                        }
                    }
                }
                allTags = chosenTags;
            }
            allTags = allTags.OrderBy(x => x).ToList();
            
            Debug.Log("order by operation took: "+((DateTime.Now - t1).TotalSeconds)+" seconds.");
            var cor = EditorCoroutineUtility.StartCoroutine(UpdateTag(editor, allTags, (tagDescList) =>
            {
                TagControl.TagFetchOperationHasCompleted = true;
                Debug.Log("the full tag operation took: " + ((DateTime.Now - t1).TotalSeconds) + " seconds. count: " + opCount + " max iteration time: " + singleOpTimeMax);

                OnComplete?.Invoke(tagDescList);
                
            }), editor);
            editor.AllCoroutines.Add(cor);
        }

        static IEnumerator UpdateTag(SearchDataEditor editor, List<string> allTags, Action<List<TagDesc>> OnFinish)
        {
            if (allTags.Count > 0)
            {
                for (int i = 0; i < allTags.Count; i++)
                {
                    TagFetchOperationProgress = (float)(i + 1) / (float)allTags.Count;
                    if (string.IsNullOrEmpty(allTags[i]) || procList.HasAny_IgnoreCase(allTags[i])
                        || allTags[i].IsItNeumeric_YT()) { continue; }
                    procList.Add(allTags[i]);

                    if (i % 50 == 0)
                    {
                        var t1 = DateTime.Now;
                        var cor = EditorCoroutineUtility.StartCoroutine(SingleTagProcCOR(allTags, allTags[i],
                     (thisDesc) =>
                     {
                         tagDescList.Add(thisDesc);
                     }), editor);
                        editor.AllCoroutines.Add(cor);
                        yield return cor;
                        opCount++;
                        if (singleOpTimeMax < (DateTime.Now - t1).TotalSeconds)
                        {
                            singleOpTimeMax = (DateTime.Now - t1).TotalSeconds;
                        }
                    }
                    else
                    {
                        var t1 = DateTime.Now;
                        SingleTagProc(allTags, allTags[i], (thisDesc) =>
                        {
                            tagDescList.Add(thisDesc);
                        });
                        opCount++;
                        if (singleOpTimeMax < (DateTime.Now - t1).TotalSeconds)
                        {
                            singleOpTimeMax = (DateTime.Now - t1).TotalSeconds;
                        }
                    }

                    //var t1 = DateTime.Now;
                    //SingleTagProc(allTags, allTags[i], (thisDesc) =>
                    //{
                    //    tagDescList.Add(thisDesc);
                    //});
                    //opCount++;
                    //if (singleOpTimeMax < (DateTime.Now - t1).TotalSeconds)
                    //{
                    //    singleOpTimeMax = (DateTime.Now - t1).TotalSeconds;
                    //}
                }
            }
            TagFetchOperationHasCompleted = true;
            OnFinish?.Invoke(tagDescList);
            EditorUtility.ClearProgressBar();
            yield return null;
        }

        static IEnumerator SingleTagProcCOR(List<string> allTags, string thisTag, Action<TagDesc> OnFinish)
        {
            SingleTagProc(allTags, thisTag, OnFinish);
            yield return null;
        }
        
        static void SingleTagProc(List<string> allTags, string thisTag, Action<TagDesc> OnFinish)
        {
            var tagDesc = new TagDesc();
            tagDesc.mainTag = thisTag;
            procList.Add(thisTag);
            List<string> relList = new List<string>();
            foreach (var tag in allTags)
            {
                if (string.IsNullOrEmpty(tag)
                || procList.HasAny_IgnoreCase(tag)) { continue; }

                if (tag.Contains_IgnoreCase(thisTag))
                {
                    relList.Add(tag);
                    procList.Add(tag);
                }
            }

            if (relList.Count > 0)
            {
                tagDesc.relatedWords = relList.ToArray();
            }
            else
            {
                tagDesc.relatedWords = null;
            }
            OnFinish?.Invoke(tagDesc);
        }
    }
}