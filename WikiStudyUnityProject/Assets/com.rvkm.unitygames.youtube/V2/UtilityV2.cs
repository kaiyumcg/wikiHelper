using HtmlAgilityPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeV2
{
    public static class UtilityV2
    {
        static bool IsNodeHolder(HtmlNode n)
        {
            return n.Id == "items" && n.HasClass("style-scope") && n.HasClass("ytd-grid-renderer") && n.Name == "div";
        }

        static bool IsNodeVideo(HtmlNode n)
        {
            return n.Id == "video-title" 
                && n.HasClass("yt-simple-endpoint") 
                && n.HasClass("style-scope") 
                && n.HasClass("ytd-grid-video-renderer") 
                && n.Name == "a";
        }

        public static void GetHolderNode(HtmlNode node, ref HtmlNode holder)
        {
            if (IsNodeHolder(node))
            {
                holder = node;
                return;
            }
            else
            {
                foreach (var n in node.ChildNodes)
                {
                    if (IsNodeHolder(n))
                    {
                        Debug.Log("Holder found!! Video count: " + n.ChildNodes.Count);
                        holder = n;
                        return;
                    }
                    else
                    {
                        GetHolderNode(n, ref holder);
                    }
                }
            }
        }

        public static void GetDataFromNode(HtmlNode node, ref UrlData data)
        {
            if (IsNodeVideo(node))
            {
                string linkName = "", linkUrl = "";
                var success = GetUrlAndLinkFromNodeAttribute(node, ref linkName, ref linkUrl);
                if (success)
                {
                    data = new UrlData(linkName, linkUrl);
                    return;
                }
                else
                {
                    throw new System.Exception("It is video not, yet we could not find link name and url!");
                }
            }
            else
            {
                foreach (var n in node.ChildNodes)
                {
                    if (IsNodeVideo(n))
                    {
                        Debug.Log("Video found!! Creating object...");
                        string linkName = "", linkUrl = "";
                        var success = GetUrlAndLinkFromNodeAttribute(n, ref linkName, ref linkUrl);
                        if (success)
                        {
                            data = new UrlData(linkName, linkUrl);
                        }
                        return;
                    }
                    else
                    {
                        GetDataFromNode(n, ref data);
                    }
                }
            }
        }

        static bool GetUrlAndLinkFromNodeAttribute(HtmlNode node, ref string linkName, ref string linkUrl)
        {
            bool success = false;
            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                foreach (var a in node.Attributes)
                {
                    if (a.Name == "title")
                    {
                        linkName = a.Value;
                    }

                    if (a.Name == "href")
                    {
                        linkUrl = "https://www.youtube.com" + a.Value;
                    }

                    if (a.Name == "title" || a.Name == "href")
                    {
                        success = true;
                    }
                }
            }
            return success;
        }
    }
}