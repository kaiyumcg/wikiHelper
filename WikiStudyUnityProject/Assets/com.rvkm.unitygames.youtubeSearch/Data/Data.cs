using com.rvkm.unitygames.YouTubeSearch.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [System.Serializable]
    public class YoutubeVideo
    {
        public string title, url;
        [HideInInspector]
        public string channelName, description;
        [HideInInspector]
        public string[] thumbUrls, tags;
        [HideInInspector]
        public int viewCount, likeCount, dislikeCount, commentCount;
        [HideInInspector]
        public int durationInMinutes;
        [HideInInspector]
        public long publishedAtDate;
        public bool YouTubeDataAPI_Cooked;

        public YoutubeVideo(string url)
        {
            this.url = url;
            this.YouTubeDataAPI_Cooked = false;
        }

        public YoutubeVideo(YoutubeVideo data)
        {
            this.title = data.title;
            this.url = data.url;
        }
    }

    [System.Serializable]
    public class TagSearchDescription
    {
        public bool use, useTextArea, useFiles;
        public string textAreaString;
        public TextAsset[] textFiles;

        bool IsTextAssetsValid(TextAsset[] assets)
        {
            bool filesValid = false;
            if (assets != null && assets.Length > 0)
            {
                foreach (var t in assets)
                {
                    if (t != null && string.IsNullOrEmpty(t.text) == false)
                    {
                        filesValid = true;
                        break;
                    }
                }
            }
            return filesValid;
        }

        public bool IsValid()
        {
            bool textFilesValid = useFiles && IsTextAssetsValid(textFiles);
            bool textAreaValid = useTextArea && string.IsNullOrEmpty(textAreaString) == false;
            return use && (useFiles || useTextArea) && (textAreaValid || textFilesValid);
        }

        string GetAllTagString()
        {
            string strTags = "";
            if (useTextArea)
            {
                strTags = textAreaString;
            }

            if (useFiles)
            {
                if (textFiles != null && textFiles.Length > 0)
                {
                    foreach (var f in textFiles)
                    {
                        if (f == null) { continue; }
                        var str = f.text;
                        if (string.IsNullOrEmpty(str)) { continue; }
                        strTags += Environment.NewLine + str;
                    }
                }
            }
            return strTags;
        }

        public List<string> GetAllTagWords()
        {
            List<string> tagWords = new List<string>();
            if (IsValid())
            {
                string bListMainData = GetAllTagString();
                bListMainData = bListMainData.Replace("[S]", "");
                var outputs = Regex.Split(Regex.Replace(bListMainData, "^[,\r\n]+|[,\r\n]+$", ""), "[,\r\n]+");
                if (outputs != null && outputs.Length > 0)
                {
                    foreach (var b in outputs)
                    {
                        if (string.IsNullOrEmpty(b)) { continue; }
                        if (tagWords.HasAny_IgnoreCase(b) == false)
                        {
                            tagWords.Add(b);
                        }
                    }
                }
            }
            return tagWords;
        }
    }

    [System.Serializable]
    public class StringSearchOp
    {
        public bool useWhitelist, useBlacklist;
        public List<string> blacklistedWords, whitelistedWords;
    }

    public enum IntSearchComp { Equal, LessthanOrEqual, GreaterthanOrEqual, Greaterthan, Lessthan, NotEqual}
    [System.Serializable]
    public class IntSearchOp
    {
        public int sValue;
        public IntSearchComp comparison;
    }

    public enum DateSearchComp { Days, Months, Years}
    [System.Serializable]
    public class DateSearchOp
    {
        public float sValue;
        public DateSearchComp comparison;
    }

    [System.Serializable]
    public class YoutubeCategory
    {
        public string categoryName;
        public bool useTitle = true, useDescription = false, useViewCount = false, 
            useLikeCount = false, useDislikeCount = false, useCommentCount = false
            , useDuration = false, usePublishedDate = false, useTags = false;
        public StringSearchOp titleOp = null, descriptionOp = null, tagOp = null;
        public IntSearchOp viewCountOp = null, likeCountOp = null, dislikeCountOp = null, commentCountOp = null, durationOp = null;
        public DateSearchOp pubDateOp = null;
        public List<YoutubeVideo> videos = null;
        public float totalMinutes;
    }

    [System.Serializable]
    public class TagDesc
    {
        public string mainTag;
        public string[] relatedWords;
    }
}