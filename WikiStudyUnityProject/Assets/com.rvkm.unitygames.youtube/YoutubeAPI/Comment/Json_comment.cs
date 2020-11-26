using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// GET https://www.googleapis.com/youtube/v3/commentThreads?key=[API_KEY]&textFormat=plainText&part=snippet&videoId=[VIDEO_ID_INPUT]&maxResults=50
/// Json generated with the help of: https://jsonutils.com/
/// Caution: you need to manually resolve the duplicate class name
/// </summary>
namespace com.rvkm.unitygames.YouTube.YTAPI_Internal_Comment
{
    [System.Serializable]
    public class PageInfo
    {
        public int totalResults;
        public int resultsPerPage;
    }

    [System.Serializable]
    public class AuthorChannelId
    {
        public string value;
    }

    [System.Serializable]
    public class Snippet2
    {
        public string videoId;
        public string textDisplay;
        public string textOriginal;
        public string authorDisplayName;
        public string authorProfileImageUrl;
        public string authorChannelUrl;
        public AuthorChannelId authorChannelId;
        public bool canRate;
        public string viewerRating;
        public int likeCount;
        public string publishedAt;
        public string updatedAt;
    }

    [System.Serializable]
    public class TopLevelComment
    {
        public string kind;
        public string etag;
        public string id;
        public Snippet2 snippet;
    }

    [System.Serializable]
    public class Snippet1
    {
        public string videoId;
        public TopLevelComment topLevelComment;
        public bool canReply;
        public int totalReplyCount;
        public bool isPublic;
    }

    [System.Serializable]
    public class Item
    {
        public string kind;
        public string etag;
        public string id;
        public Snippet1 snippet;
    }

    [System.Serializable]
    public class CommentInfoFromYtDataApi
    {
        public string kind;
        public string etag;
        public PageInfo pageInfo;
        public Item[] items;
    }
}