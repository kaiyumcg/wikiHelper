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
            Copy_Data_From(jsonDataWiki);
        }

        public WikiDataJson GetJsonData()
        {
            WikiDataJson result = new WikiDataJson 
            { 
                isDataProcessed = this.isDataProcessed, 
                mainNode = this.mainNode, 
                ticksAtSaveTime = this.ticksAtSaveTime.Ticks,
                url_s = Utility.UI_To_Json(url_s),
                procList = Utility.UI_To_Json(procList),
            };
            return result;    
        }

        public void Copy_Data_From(WikiDataJson wikiData)
        {
            this.mainNode = wikiData.mainNode;
            this.ticksAtSaveTime = new DateTime(wikiData.ticksAtSaveTime);
            this.isDataProcessed = wikiData.isDataProcessed;
            this.url_s = Utility.Json_To_UI(wikiData.url_s);
            this.procList = Utility.Json_To_UI(wikiData.procList);           
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