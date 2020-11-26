using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeV2
{
    [CreateAssetMenu(fileName = "New Category Data", menuName = "Kaiyum/Category data(V2)", order = 54)]
    public class CategorizedDataDesc : ScriptableObject
    {
        public string channelName;
        public List<Category> allCategories;                                              
    }
}