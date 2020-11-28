using com.rvkm.unitygames.YouTubeSearchInternal;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class EdUtility
    {
        public static bool IsUrl(string str)
        {
            return (!string.IsNullOrEmpty(str)) && (str.Contains("http://") || str.Contains("https://"));
        }

        static bool IsNodeVideo(HtmlNode n)
        {
            return n.Id == "video-title"
                && n.HasClass("yt-simple-endpoint")
                && n.HasClass("style-scope")
                && n.HasClass("ytd-grid-video-renderer")
                && n.Name == "a";
        }

        static bool GetLinkFromNode(HtmlNode node, ref string linkUrl)
        {
            bool success = false;
            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                foreach (var a in node.Attributes)
                {
                    if (a.Name == "href")
                    {
                        linkUrl = "https://www.youtube.com" + a.Value;
                        success = true;
                        break;
                    }
                }
            }
            return success;
        }

        public static List<YoutubeVideo> GetAllVideoInfo(string htmlText)
        {
            var vdList = new List<YoutubeVideo>();
            if (string.IsNullOrEmpty(htmlText) == false)
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlText);
                AddVideos(doc.DocumentNode, ref vdList);
            }
            return vdList;
        }

        static void AddVideos(HtmlNode node, ref List<YoutubeVideo> vdList)
        {
            if (IsNodeVideo(node))
            {
                string link = "";
                bool success = GetLinkFromNode(node, ref link);
                if (success)
                {

                    bool alreadyExist = vdList.Exists((pred) => { return pred.url == link; });
                    if (!alreadyExist)
                    {
                        vdList.Add(new YoutubeVideo(link));
                    }
                }
            }
            else
            {
                foreach (var cn in node.ChildNodes)
                {
                    AddVideos(cn, ref vdList);
                }
            }
        }

        static float GetDurationInMinute(string durationStr)
        {
            string hStr = "", mStr = "", sStr = "";
            if (durationStr.Contains("PT"))
            {
                durationStr = durationStr.Replace("PT", "");
            }

            var cArray = durationStr.ToCharArray();
            int h_id_start = 0, m_id_start = 0, s_id_start = 0;
            for (int i = 0; i < cArray.Length; i++)
            {
                if (cArray[i] == 'H')
                {
                    h_id_start = i;
                }
                if (cArray[i] == 'M')
                {
                    m_id_start = i;
                }
                if (cArray[i] == 'S')
                {
                    s_id_start = i;
                }
            }

            if (durationStr.Contains("H"))
            {
                for (int i = 0; i < h_id_start; i++)
                {
                    if (char.IsDigit(cArray[i]) || cArray[i] == '.')
                    {
                        if (hStr.Contains("."))
                        {
                            if (char.IsDigit(cArray[i]))
                            {
                                hStr += cArray[i];
                            }
                        }
                        else
                        {
                            hStr += cArray[i];
                        }

                    }
                }
            }

            if (durationStr.Contains("M"))
            {
                int m_end = 0;
                for (int i = m_id_start - 1; i >= 0; i--)
                {
                    if (char.IsDigit(cArray[i]) == false && cArray[i] != '.')
                    {
                        m_end = i;
                        break;
                    }
                }

                if (m_end > 0)
                {
                    for (int i = m_end + 1; i < m_id_start; i++)
                    {
                        if (char.IsDigit(cArray[i]) || cArray[i] == '.')
                        {
                            if (mStr.Contains("."))
                            {
                                if (char.IsDigit(cArray[i]))
                                {
                                    mStr += cArray[i];
                                }
                            }
                            else
                            {
                                mStr += cArray[i];
                            }
                        }
                    }
                }
            }

            if (durationStr.Contains("S"))
            {
                int s_end = 0;
                for (int i = s_id_start - 1; i >= 0; i--)
                {
                    if (char.IsDigit(cArray[i]) == false && cArray[i] != '.')
                    {
                        s_end = i;
                        break;
                    }
                }

                if (s_end > 0)
                {
                    for (int i = s_end + 1; i < s_id_start; i++)
                    {
                        if (char.IsDigit(cArray[i]) || cArray[i] == '.')
                        {
                            if (sStr.Contains("."))
                            {
                                if (char.IsDigit(cArray[i]))
                                {
                                    sStr += cArray[i];
                                }
                            }
                            else
                            {
                                sStr += cArray[i];
                            }
                        }
                    }
                }
            }
            float hour = 0f;
            float minute = 0f;
            float second = 0f;
            float.TryParse(hStr, out hour);
            float.TryParse(mStr, out minute);
            float.TryParse(sStr, out second);

            return hour * 60f + minute + (second / 60f);
        }

        static bool GetDate(string publishedDateStr, ref DateTime publishedDate)
        {
            publishedDate = new DateTime(1980, 1, 1, 1, 1, 1, 1);
            var success = DateTime.TryParse(publishedDateStr, out publishedDate);
            return success;
        }

        public static string GetWWWResponse(string Url)
        {
            string result = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (String.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                result = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
            return result;
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
                string videoID = GetVideoId(video.url);
                string url = "https://youtube.googleapis.com/youtube/v3/" +
                    "videos?part=snippet&part=contentDetails&part=topicDetails&part=statistics&id="
                    + videoID + "&key=" + API_KEY;
                bool titleOk = false, descriptionOk = false, channelNameOk = false,
                    thumbUrlOk = false, tagsOk = false, viewCountOk = false,
                    likeCountOk = false, dislikeCountOk = false, commentCountOk = false,
                    durationOk = false, pubDateOk = false;
                string json = EdUtility.GetWWWResponse(url);
                VideoInfoJson data = new VideoInfoJson();
                try
                {
                    EditorJsonUtility.FromJsonOverwrite(json, data);
                }
                catch (Exception ex)
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
                        video.durationInMinutes = (int)GetDurationInMinute(item.contentDetails.duration);
                        durationOk = true;
                    }


                    //item.snippet.channelTitle
                    if (item.snippet != null && string.IsNullOrEmpty(item.snippet.channelTitle) == false)
                    {
                        video.channelName = item.snippet.channelTitle;
                        channelNameOk = true;
                    }

                    //item.snippet.description
                    if (item.snippet != null && string.IsNullOrEmpty(item.snippet.description) == false)
                    {
                        video.description = item.snippet.description;
                        descriptionOk = true;
                    }

                    //item.snippet.publishedAt
                    if (item.snippet != null && string.IsNullOrEmpty(item.snippet.publishedAt) == false)
                    {
                        DateTime pubDate = DateTime.Now;
                        var success = GetDate(item.snippet.publishedAt, ref pubDate);
                        if (success)
                        {
                            video.publishedAtDate = pubDate.Ticks;
                            pubDateOk = true;
                        }
                    }

                    //item.snippet.tags
                    if (item.snippet != null && item.snippet.tags != null)
                    {
                        video.tags = item.snippet.tags;
                        tagsOk = true;
                    }

                    List<string> thumbUrls = new List<string>();
                    //item.snippet.thumbnails.Default
                    if (item.snippet != null && item.snippet.thumbnails != null
                        && item.snippet.thumbnails.Default != null && string.IsNullOrEmpty(item.snippet.thumbnails.Default.url) == false)
                    {
                        thumbUrls.Add(item.snippet.thumbnails.Default.url);
                    }

                    //item.snippet.thumbnails.standard
                    if (item.snippet != null && item.snippet.thumbnails != null
                        && item.snippet.thumbnails.standard != null && string.IsNullOrEmpty(item.snippet.thumbnails.standard.url) == false)
                    {
                        thumbUrls.Add(item.snippet.thumbnails.standard.url);
                    }

                    //item.snippet.thumbnails.medium
                    if (item.snippet != null && item.snippet.thumbnails != null
                        && item.snippet.thumbnails.medium != null && string.IsNullOrEmpty(item.snippet.thumbnails.medium.url) == false)
                    {
                        thumbUrls.Add(item.snippet.thumbnails.medium.url);
                    }

                    //item.snippet.thumbnails.maxres
                    if (item.snippet != null && item.snippet.thumbnails != null
                        && item.snippet.thumbnails.maxres != null && string.IsNullOrEmpty(item.snippet.thumbnails.maxres.url) == false)
                    {
                        thumbUrls.Add(item.snippet.thumbnails.maxres.url);
                    }

                    //item.snippet.thumbnails.high
                    if (item.snippet != null && item.snippet.thumbnails != null
                        && item.snippet.thumbnails.high != null && string.IsNullOrEmpty(item.snippet.thumbnails.high.url) == false)
                    {
                        thumbUrls.Add(item.snippet.thumbnails.high.url);
                    }

                    if (thumbUrls.Count > 0)
                    {
                        video.thumbUrls = thumbUrls.ToArray();
                        thumbUrlOk = true;
                    }

                    //item.snippet.title
                    if (item.snippet != null && string.IsNullOrEmpty(item.snippet.title) == false)
                    {
                        video.title = item.snippet.title;
                        titleOk = true;
                    }

                    //item.statistics.commentCount
                    if (item.statistics != null && string.IsNullOrEmpty(item.statistics.commentCount) == false)
                    {
                        var str = item.statistics.commentCount;
                        int count = 0;
                        var success = int.TryParse(str, out count);
                        if (success)
                        {
                            video.commentCount = count;
                            commentCountOk = true;
                        }
                    }

                    //item.statistics.dislikeCount
                    if (item.statistics != null && string.IsNullOrEmpty(item.statistics.dislikeCount) == false)
                    {
                        var str = item.statistics.dislikeCount;
                        int count = 0;
                        var success = int.TryParse(str, out count);
                        if (success)
                        {
                            video.dislikeCount = count;
                            dislikeCountOk = true;
                        }
                    }

                    //item.statistics.likeCount
                    if (item.statistics != null && string.IsNullOrEmpty(item.statistics.likeCount) == false)
                    {
                        var str = item.statistics.likeCount;
                        int count = 0;
                        var success = int.TryParse(str, out count);
                        if (success)
                        {
                            video.likeCount = count;
                            likeCountOk = true;
                        }
                    }

                    //item.statistics.viewCount
                    if (item.statistics != null && string.IsNullOrEmpty(item.statistics.viewCount) == false)
                    {
                        var str = item.statistics.viewCount;
                        int count = 0;
                        var success = int.TryParse(str, out count);
                        if (success)
                        {
                            video.viewCount = count;
                            viewCountOk = true;
                        }
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

        /// <summary>
        /// https://codesnippets.fesslersoft.de/get-the-youtube-videoid-from-url/
        /// </summary>
        const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
        public static string GetVideoId(string url)
        {
            var regex = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
            foreach (Match match in regex.Matches(url))
            {
                foreach (var groupdata in match.Groups.Cast<Group>().Where(groupdata => !groupdata.ToString().StartsWith("http://") && !groupdata.ToString().StartsWith("https://") && !groupdata.ToString().StartsWith("youtu") && !groupdata.ToString().StartsWith("www.")))
                {
                    return groupdata.ToString();
                }
            }
            return string.Empty;
        }

        static void ShowArray(SerializedProperty list, bool showListSize = true)
        {
            EditorGUILayout.PropertyField(list, false);
            EditorGUI.indentLevel += 1;
            if (list.isExpanded)
            {
                if (showListSize)
                {
                    EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
                }
                for (int i = 0; i < list.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                }
            }
            EditorGUI.indentLevel -= 1;
        }
    }
}