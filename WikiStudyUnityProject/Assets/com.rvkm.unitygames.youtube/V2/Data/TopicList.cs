using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeV2
{
    [CreateAssetMenu(fileName = "New Topic List", menuName = "Kaiyum/YouTube Topic List(V2)", order = 53)]
    public class TopicList : ScriptableObject
    {
        public List<string> topics;
        public int minTopicLength;
    }
}