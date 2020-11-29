using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.rvkm.unitygames.YouTubeSearch
{
    public class YoutubeVideoData : ScriptableObject
    {
        public string searchName;
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