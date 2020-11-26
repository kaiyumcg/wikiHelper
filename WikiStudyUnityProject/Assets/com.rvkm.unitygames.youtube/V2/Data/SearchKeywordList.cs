using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeV2
{
    [CreateAssetMenu(fileName = "New Search Keyword Data", menuName = "Kaiyum/YouTube Keywords(V2)", order = 52)]
    public class SearchKeywordList : ScriptableObject
    {
        public List<string> blackList, whiteList;
    }
}