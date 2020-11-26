using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTube
{
    [CreateAssetMenu(fileName = "Youtube channel data", menuName = "YT Data", order = 51)]
    public class ChannelVideoDescription : ScriptableObject
    {
        public ChannelVideoInfoRaw[] allVideoInformation;
    }
}