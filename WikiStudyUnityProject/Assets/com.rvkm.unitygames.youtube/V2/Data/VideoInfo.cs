using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeV2
{
    [CreateAssetMenu(fileName = "New Youtube Channel Data", menuName = "Kaiyum/YouTube Channel Data(V2)", order = 51)]
    public class VideoInfo : ScriptableObject
    {
        public string channelName = "";
        public UrlData[] allVideoInfo;
    }
}