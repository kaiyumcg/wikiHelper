using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class ScrapUtility
    {
        static bool IsNodeHolder(HtmlNode n)
        {
            return n.Id == "items" && n.HasClass("style-scope") && n.HasClass("ytd-grid-renderer") && n.Name == "div";
        }

        public static List<YoutubeVideo> GetAllVideoInfo(string htmlText)
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlText);
            HtmlNode holderNode = null;
            GetHolderNode(doc.DocumentNode, ref holderNode);
            var vdList = new List<YoutubeVideo>();
            if (holderNode == null)
            {
                throw new Exception("We couldnot find any holder!");
            }
            else
            {
                if (holderNode.ChildNodes != null && holderNode.ChildNodes.Count > 0)
                {
                    foreach (var v in holderNode.ChildNodes)
                    {
                        if (v == null) { continue; }
                        YoutubeVideo urlData = null;
                        GetDataFromNode(v, ref urlData);
                        if (urlData == null)
                        {
                            throw new Exception("url data can not be null!");
                        }
                        else
                        {
                            vdList.Add(urlData);
                        }
                    }
                }
            }
            return vdList;
        }

        public static void CategoryIt(SearchDataYoutube vInfo, ref List<YoutubeCategory> catList)
        {
            List<YoutubeVideo> processedList = new List<YoutubeVideo>();
            if (catList != null && catList.Count > 0)
            {
                YoutubeCategory uncat = null;
                uncat = catList.Find((data) => { return data.categoryName == "uncategorized"; });
                if (uncat != null) { catList.Remove(uncat); }

                if (catList != null && catList.Count > 0)
                {
                    foreach (var c in catList)
                    {
                        if (c == null) { continue; }
                        c.videos = new List<YoutubeVideo>();
                        if (vInfo != null && vInfo.allVideos != null && vInfo.allVideos.Length > 0)
                        {
                            for (int i = 0; i < vInfo.allVideos.Length; i++)
                            {
                                var vdoInfo = vInfo.allVideos[i];
                                if (vdoInfo == null) { continue; }
                                var exist = c.videos.Exists((pred) =>
                                { return string.Equals(pred.url, vdoInfo.url, System.StringComparison.OrdinalIgnoreCase); });

                                if (exist == false)
                                {
                                    if (vdoInfo.title.ContainsAnyOf(c.titleOp.whitelistedWords)
                                        && !vdoInfo.title.ContainsAnyOf(c.titleOp.blacklistedWords))
                                    {
                                        c.videos.Add(new YoutubeVideo(vdoInfo));
                                        processedList.Add(vInfo.allVideos[i]);
                                    }
                                }
                            }
                        }

                    }
                }
            }

            List<YoutubeVideo> uncatList = new List<YoutubeVideo>();
            if (vInfo != null && vInfo.allVideos != null && vInfo.allVideos.Length > 0)
            {
                foreach (var v in vInfo.allVideos)
                {
                    if (v == null) { continue; }
                    if (!processedList.Contains(v))
                    {
                        uncatList.Add(v);
                    }
                }
            }

            var uncatV2 = new YoutubeCategory() { categoryName = "uncategorized", videos = uncatList };
            catList.Add(uncatV2);
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

        public static void GetDataFromNode(HtmlNode node, ref YoutubeVideo data)
        {
            if (IsNodeVideo(node))
            {
                string linkUrl = "";
                var success = GetUrlAndLinkFromNodeAttribute(node, ref linkUrl);
                if (success)
                {
                    data = new YoutubeVideo(linkUrl);
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
                        string linkUrl = "";
                        var success2 = GetUrlAndLinkFromNodeAttribute(n, ref linkUrl);
                        if (success2)
                        {
                            data = new YoutubeVideo(linkUrl);
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

        private static bool IsNodeVideo(HtmlNode node)
        {
            throw new System.NotImplementedException();
        }

        private static bool GetUrlAndLinkFromNodeAttribute(HtmlNode node, ref string linkUrl)
        {
            throw new System.NotImplementedException();
        }
    }
}