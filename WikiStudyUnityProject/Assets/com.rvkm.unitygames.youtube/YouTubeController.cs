using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace com.rvkm.unitygames.YouTube
{
    /// <summary>
    /// Step 1: load the full page in chrome
    /// Step 2: open 'Inspect' and traverse the container and load all of them inside 'Inspect'
    /// Step 3: copy the full html doc and create an xml file inside of the content in it
    /// Step 4: open visual studio and delete all script tag in the begining. 
    /// Step 5: Open the file locally to see if you can actually see it.
    /// Step 6: Store it inside a localhost xampp server and put the file url in this script component
    /// Step 7: <>
    /// </summary>
    public class YouTubeController : MonoBehaviour
    {
        [SerializeField] string localhostFileUrl = "http://localhost/data.html";
        /// <summary>
        /// This component will update this asset file's data.
        /// After that we will show the UI through which we will create search operation
        /// And subsequently we will have the ability to watch the video directly from the app
        /// We will also record which video is clicked how much time and how long we watched the video. Save into player pref.
        /// We also have ability to clear save data
        /// </summary>
        [SerializeField] ChannelVideoDescription channelDataAsset;
        //11 no line for other information

        VideoHolderSelector getVideosHolder;
       // ThumbDownloader thumbDownloader;
        void Start()
        {
            getVideosHolder = VideoHolderSelector.Init(gameObject);
           // thumbDownloader = ThumbDownloader.Init(gameObject);
        }

        
        void GetAllVideoData()
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load(localhostFileUrl);
            HtmlNode allVideoHolderNode = null;
            getVideosHolder.SearchForAllVideoContainerNode(doc.DocumentNode, (data) =>
            {
                allVideoHolderNode = data;
                if (allVideoHolderNode == null) { return; }

                HtmlNode testVideoNode = null;
                foreach (var i in allVideoHolderNode.ChildNodes)
                {
                    testVideoNode = i;
                    break;
                }
                //thumbDownloader.GetThumbnail(testVideoNode, (thumb) => {
                //    tex = thumb;
                //    DurationData duration = new DurationData { hours = hour, minutes = minute, seconds = second };
                //    DurationFetcher.GetVideoDuration(testVideoNode, ref duration);
                //});
                Debug.Log("op completed");

            });
        }

        public int videoHolderNodeOpCount, thumbDownloadCount;
        [SerializeField] Texture2D tex;
        [SerializeField] int hour, minute, second;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetAllVideoData();
            }

            videoHolderNodeOpCount = getVideosHolder.VideoHolderSearchOperationCount;
            //thumbDownloadCount = thumbDownloader.ThumbDownloadCount;
        }
    }
}