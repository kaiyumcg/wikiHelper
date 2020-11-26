using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// GET https://youtube.googleapis.com/youtube/v3/videos?part=snippet&part=statistics&part=contentDetails&id=[VIDEO_ID_INPUT]&key=[YOUR_API_KEY]
/// Json generated with the help of: https://jsonutils.com/
/// </summary>
namespace com.rvkm.unitygames.YouTube.YouTubeSearchInternal
{
    [System.Serializable]
    public class Default
    {
        public string url;
        public int width;
        public int height;
    }

    [System.Serializable]
    public class Medium
    {
        public string url;
        public int width;
        public int height;
    }

    [System.Serializable]
    public class High
    {
        public string url;
        public int width;
        public int height;
    }

    [System.Serializable]
    public class Standard
    {
        public string url;
        public int width;
        public int height;
    }

    [System.Serializable]
    public class Maxres
    {
        public string url;
        public int width;
        public int height;
    }

    [System.Serializable]
    public class Thumbnails
    {
        public Default Default;
        public Medium medium;
        public High high;
        public Standard standard;
        public Maxres maxres;
    }

    [System.Serializable]
    public class Localized
    {
        public string title;
        public string description;
    }

    [System.Serializable]
    public class Snippet
    {
        public string publishedAt;
        public string channelId;
        public string title;
        public string description;
        public Thumbnails thumbnails;
        public string channelTitle;
        public string[] tags;
        public string categoryId;
        public string liveBroadcastContent;
        public Localized localized;
        public string defaultAudioLanguage;
    }

    [System.Serializable]
    public class ContentRating
    {

    }

    [System.Serializable]
    public class ContentDetails
    {
        public string duration;
        public string dimension;
        public string definition;
        public string caption;
        public bool licensedContent;
        public ContentRating contentRating;
        public string projection;
    }

    [System.Serializable]
    public class Statistics
    {
        public string viewCount;
        public string likeCount;
        public string dislikeCount;
        public string favoriteCount;
        public string commentCount;
    }

    [System.Serializable]
    public class TopicDetails
    {
        public string[] relevantTopicIds;
        public string[] topicCategories;
    }

    [System.Serializable]
    public class Item
    {
        public string kind;
        public string etag;
        public string id;
        public Snippet snippet;
        public ContentDetails contentDetails;
        public Statistics statistics;
        public TopicDetails topicDetails;
    }

    [System.Serializable]
    public class PageInfo
    {
        public int totalResults;
        public int resultsPerPage;
    }

    [System.Serializable]
    public class VideoJsonData
    {
        public string kind;
        public string etag;
        public Item[] items;
        public PageInfo pageInfo;
    }
}