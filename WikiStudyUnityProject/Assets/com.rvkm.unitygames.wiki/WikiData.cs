using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.wiki
{
    [System.Serializable]
    public class WikiDataJson
    {
        public string mainNode;
        public long ticksAtSaveTime;
        public bool isDataProcessed;
        public Url_Json[] url_s;
        public Url_Json[] procList;
    }

    [System.Serializable]
    public class Url_Json
    {
        public string url, url_name;
        public Url_State url_state;
        public long ticksForDateTime;
    }

    [System.Serializable]
    public enum Url_State
    {
        Related, Manual_NotRelated, Auto_NotRelated
    };
}