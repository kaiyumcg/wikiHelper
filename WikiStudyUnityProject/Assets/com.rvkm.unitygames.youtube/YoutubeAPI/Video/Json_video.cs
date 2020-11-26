using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// GET https://youtube.googleapis.com/youtube/v3/videos?part=snippet&part=statistics&part=contentDetails&id=[VIDEO_ID_INPUT]&key=[YOUR_API_KEY]
/// Json generated with the help of: https://jsonutils.com/
/// </summary>
namespace com.rvkm.unitygames.YouTube.YTAPI_Internal_Video
{
    public class DefaultSizeThumb
    {
        public string url;
        public int width;
        public int height;
    }

    public class MediumSizeThumb
    {
        public string url;
        public int width;
        public int height;
    }

    public class HighSizeThumb
    {
        public string url;
        public int width;
        public int height;
    }

    public class StandardSizeThumb
    {
        public string url;
        public int width;
        public int height;
    }

    public class MaxresThumb
    {
        public string url;
        public int width;
        public int height;
    }

    public class Thumbnails
    {
        /// <summary>
        /// Might not load this one because youtube API expect the field name to be 'default', yet
        /// 'default' is a keyword in C#
        /// </summary>
        public DefaultSizeThumb Default;
        public MediumSizeThumb medium;
        public HighSizeThumb high;
        public StandardSizeThumb standard;
        public MaxresThumb maxres;
    }

    public class Localized
    {
        public string title;
        public string description;
    }

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

    public class ContentRating
    {

    }

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

    public class Statistics
    {
        public string viewCount;
        public string likeCount;
        public string dislikeCount;
        public string favoriteCount;
        public string commentCount;
    }

    public class Item
    {
        public string kind;
        public string etag;
        public string id;
        public Snippet snippet;
        public ContentDetails contentDetails;
        public Statistics statistics;
    }

    public class PageInfo
    {
        public int totalResults;
        public int resultsPerPage;
    }

    public class VideoInfoFromYtDataApi
    {
        public string kind;
        public string etag;
        public Item[] items;
        public PageInfo pageInfo;
    }
}