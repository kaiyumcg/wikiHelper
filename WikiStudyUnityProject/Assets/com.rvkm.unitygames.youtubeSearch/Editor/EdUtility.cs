using com.rvkm.unitygames.YouTubeSearchInternal;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    }
}