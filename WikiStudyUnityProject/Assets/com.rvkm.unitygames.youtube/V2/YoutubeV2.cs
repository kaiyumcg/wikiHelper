using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeV2
{
    public class YoutubeV2 : MonoBehaviour
    {
        [SerializeField] TextAsset fullPageData;
        List<UrlData> allVideoUrls = new List<UrlData>();
        [SerializeField] VideoInfo videoInfo;

        void GetAllVideoInfo()
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument(); //hw.Load("url");
            doc.LoadHtml(fullPageData.text);
            HtmlNode holderNode = null;
            UtilityV2.GetHolderNode(doc.DocumentNode, ref holderNode);
            allVideoUrls = new List<UrlData>();
            if (holderNode == null)
            {
                throw new Exception("holder is null!");
            }
            else
            {
                if (holderNode.ChildNodes != null && holderNode.ChildNodes.Count > 0)
                {
                    foreach (var v in holderNode.ChildNodes)
                    {
                        if (v == null) { continue; }
                        UrlData urlData = null;
                        UtilityV2.GetDataFromNode(v, ref urlData);
                        if (urlData == null)
                        {
                            throw new Exception("url data can not be null!");
                        }
                        else
                        {
                            allVideoUrls.Add(urlData);
                        }
                    }
                }

                videoInfo.allVideoInfo = null;
                videoInfo.allVideoInfo = new UrlData[allVideoUrls.Count];
                for (int i = 0; i < allVideoUrls.Count; i++)
                {
                    videoInfo.allVideoInfo[i] = new UrlData(allVideoUrls[i]);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetAllVideoInfo();
            }
        }
    }
}