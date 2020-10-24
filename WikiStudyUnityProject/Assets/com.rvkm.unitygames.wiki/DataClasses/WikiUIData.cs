using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.wiki
{
    [System.Serializable]
    public class WikiUIData
    {
        public string mainNode;
        public DateTime ticksAtSaveTime;
        public bool isDataProcessed;
        public List<Url_UI_Data> url_s;
        public List<Url_UI_Data> procList;

        public WikiUIData(WikiDataJson jsonDataWiki)
        {
            this.mainNode = jsonDataWiki.mainNode;
            this.ticksAtSaveTime = new DateTime(jsonDataWiki.ticksAtSaveTime);
            this.isDataProcessed = jsonDataWiki.isDataProcessed;
            this.url_s = DataConversionUtility.Json_To_UI(jsonDataWiki.url_s);
            this.procList = DataConversionUtility.Json_To_UI(jsonDataWiki.procList);
        }
    }

    [System.Serializable]
    public class Url_UI_Data
    {
        public string url, url_name;
        public Url_State url_state;
        public DateTime ticksForDateTime;
    }
}