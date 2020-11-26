using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTube
{
    public class VideoHolderSelector : MonoBehaviour
    {
        int holderSearchOpCount;
        public int VideoHolderSearchOperationCount { get { return holderSearchOpCount; } }
        public static VideoHolderSelector Init(GameObject youtubeControllerGameObject)
        {
            var data = youtubeControllerGameObject.GetComponent<VideoHolderSelector>();
            if (data == null)
            {
                data = youtubeControllerGameObject.AddComponent<VideoHolderSelector>();
            }
            data.holderSearchOpCount = 0;
            return data;
        }

        public void SearchForAllVideoContainerNode(HtmlNode node, Action<HtmlNode> OnGetResult)
        {
            StartCoroutine(SearchForAllVideoContainerNodeCOR(node, OnGetResult));
        }

        IEnumerator SearchForAllVideoContainerNodeCOR(HtmlNode node, Action<HtmlNode> OnGetResult)
        {
            holderSearchOpCount++;
            foreach (var n in node.ChildNodes)
            {
                if (n.Id == "items" && n.HasClass("style-scope") && n.HasClass("ytd-grid-renderer") && n.Name == "div")
                {
                    string clsStr = "";
                    foreach (var c in n.GetClasses())
                    {
                        clsStr += " , " + c;
                    }
                    Debug.Log("got it!: " + n.ChildNodes.Count + " and all classes: " + clsStr + " for node: " + n.Name);
                    OnGetResult?.Invoke(n);
                    break;
                }
                else
                {
                    if (holderSearchOpCount % 100 == 0)
                    {
                        yield return null;
                    }
                    StartCoroutine(SearchForAllVideoContainerNodeCOR(n, OnGetResult));
                }
            }
        }
    }
}