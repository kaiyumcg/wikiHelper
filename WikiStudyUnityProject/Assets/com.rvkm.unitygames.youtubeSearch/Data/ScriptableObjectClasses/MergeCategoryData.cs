using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [CreateAssetMenu(fileName = "New Merge Utility", menuName = "Kaiyum/Youtube Caregory Data Merge Utility", order = 3)]
    public class MergeCategoryData : ScriptableObject
    {
        public YoutubeCategory[] inputCategories;
        [ReadOnly]
        public YoutubeCategory outputCategory;
    }
}