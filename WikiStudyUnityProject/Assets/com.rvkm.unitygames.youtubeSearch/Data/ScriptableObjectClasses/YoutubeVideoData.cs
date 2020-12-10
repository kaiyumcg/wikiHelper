using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.rvkm.unitygames.YouTubeSearch
{
    public class YoutubeVideoData : ScriptableObject
    {
        [HideInInspector]
        public string searchName, belongedCategory;
        public YoutubeVideo[] allVideos;
        public bool IsDataOk()
        {
            bool isOk = true;
            if (allVideos != null && allVideos.Length > 0)
            {
                foreach (var v in allVideos)
                {
                    if (v == null || v.YouTubeDataAPI_Cooked == false)
                    {
                        isOk = false;
                        break;
                    }
                }
            }
            else
            {
                isOk = false;
            }
            return isOk;
        }
    }
}