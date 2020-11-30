using com.rvkm.unitygames.YouTubeSearchInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class YoutubeDataAPI
    {
        static void GetThumbs(Thumbnails thumb, ref YoutubeVideo video, ref bool thumbOk)
        {
            if (thumb == null) { return; }
            List<string> thumbUrls = new List<string>();
            if (thumb.Default != null && string.IsNullOrEmpty(thumb.Default.url) == false)
            {
                thumbUrls.Add(thumb.Default.url);
            }
            if (thumb.standard != null && string.IsNullOrEmpty(thumb.standard.url) == false)
            {
                thumbUrls.Add(thumb.standard.url);
            }
            if (thumb.medium != null && string.IsNullOrEmpty(thumb.medium.url) == false)
            {
                thumbUrls.Add(thumb.medium.url);
            }
            if (thumb.maxres != null && string.IsNullOrEmpty(thumb.maxres.url) == false)
            {
                thumbUrls.Add(thumb.maxres.url);
            }
            if (thumb.high != null && string.IsNullOrEmpty(thumb.high.url) == false)
            {
                thumbUrls.Add(thumb.high.url);
            }

            if (thumbUrls.Count > 0)
            {
                video.thumbUrls = thumbUrls.ToArray();
                thumbOk = true;
            }
        }

        public static bool UpdateFromYoutubeDataAPI(ref YoutubeVideo video, ref string status, string API_KEY, bool forceUpdate = false)
        {
            if (video.YouTubeDataAPI_Cooked && forceUpdate == false)
            {
                Debug.Log("this video is cooked! no need to proceed further!");
                status = "this video is cooked! no need to proceed further!";
                return true;
            }
            else
            {
                string videoID = ChannelDataEditorUtility.GetVideoId(video.url);
                string url = "https://youtube.googleapis.com/youtube/v3/" +
                    "videos?part=snippet&part=contentDetails&part=topicDetails&part=statistics&id="
                    + videoID + "&key=" + API_KEY;
                bool titleOk = false, descriptionOk = false, channelNameOk = false,
                    thumbUrlOk = false, tagsOk = false, viewCountOk = false,
                    likeCountOk = false, dislikeCountOk = false, commentCountOk = false,
                    durationOk = false, pubDateOk = false;
                string json = ChannelDataEditorUtility.GetWWWResponse(url);
                VideoInfoJson data = new VideoInfoJson();
                try
                {
                    EditorJsonUtility.FromJsonOverwrite(json, data);
                }
                catch (Exception)
                {
                    string msg = "could not perse json for video info!";
                    status = msg;
                    throw new Exception(msg);
                }
                var item = GetItem(data);
                if (item != null)
                {
                    //item.contentDetails.duration
                    if (item.contentDetails != null && string.IsNullOrEmpty(item.contentDetails.duration) == false)
                    {
                        video.durationInMinutes = (int)DateTimeUtility.GetDurationInMinute(item.contentDetails.duration);
                        durationOk = true;
                    }

                    if (item.snippet != null)
                    {
                        video.channelName = ChannelDataEditorUtility.GetStringIfNullOrEmpty(item.snippet.channelTitle, ref channelNameOk);
                        video.description = ChannelDataEditorUtility.GetStringIfNullOrEmpty(item.snippet.description, ref descriptionOk);
                        if (string.IsNullOrEmpty(item.snippet.publishedAt) == false)
                        {
                            DateTime pubDate = DateTime.Now;
                            var success = DateTimeUtility.GetDate(item.snippet.publishedAt, ref pubDate);
                            if (success)
                            {
                                video.publishedAtDate = pubDate.Ticks;
                                pubDateOk = true;
                            }
                        }
                        if (item.snippet.tags != null)
                        {
                            video.tags = item.snippet.tags;
                            tagsOk = true;
                        }

                        List<string> thumbUrls = new List<string>();
                        GetThumbs(item.snippet.thumbnails, ref video, ref thumbUrlOk);
                        video.title = ChannelDataEditorUtility.GetStringIfNullOrEmpty(item.snippet.title, ref titleOk);
                    }

                    if (item.statistics != null)
                    {
                        video.commentCount = ChannelDataEditorUtility.GetIntFromString(item.statistics.commentCount, ref commentCountOk);
                        video.dislikeCount = ChannelDataEditorUtility.GetIntFromString(item.statistics.dislikeCount, ref dislikeCountOk);
                        video.likeCount = ChannelDataEditorUtility.GetIntFromString(item.statistics.likeCount, ref likeCountOk);
                        video.viewCount = ChannelDataEditorUtility.GetIntFromString(item.statistics.viewCount, ref viewCountOk);
                    }
                }

                if (titleOk && channelNameOk && durationOk && pubDateOk)
                {
                    video.YouTubeDataAPI_Cooked = true;
                    Debug.Log("set cooked!");
                    return true;
                }
                else
                {
                    string msg = "";
                    if (data == null)
                    {
                        msg += "The json data was not ok. Your Youtube API quota has probably been consumed! Use another API key or wait for tomorrow. " +
                            "Or Check your internet connection and/or your proxy setting to make sure you can access the web pages " +
                            "and/or http://youtube.com . ";
                    }
                    else
                    {
                        if (item == null)
                        {
                            msg += "The item was not ok. Your Youtube API quota has probably been consumed! Use another API key or wait for tomorrow. " +
                            "Or Check your internet connection and/or your proxy setting to make sure you can access the web pages " +
                            "and/or http://youtube.com . ";
                        }
                        else
                        {
                            if (!titleOk) { msg += "Cound not fetch title! Check the log in red color! "; }
                            if (!channelNameOk) { msg += "Cound not fetch the channel name!. Check the log in red color! "; }
                            if (!durationOk) { msg += "Cound not fetch the duration! Check the log in red color! "; }
                            if (!pubDateOk) { msg += "Cound not fetch the publishing date! Check the log in red color! "; }
                        }
                    }


                    status = "could not get video for this ID: " + videoID + " msg: " + msg;
                    return false;
                }
            }
        }

        static Item GetItem(VideoInfoJson data)
        {
            Item result = null;
            if (data != null && data.items != null && data.items.Length > 0)
            {
                foreach (var i in data.items)
                {
                    if (i == null) { continue; }
                    if (i != null)
                    {
                        result = i;
                        break;
                    }
                }
            }
            return result;
        }
    }
}