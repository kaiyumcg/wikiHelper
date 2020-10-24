using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.wiki
{
    public static class DataConversionUtility
    {
        public static List<Url_UI_Data> Json_To_UI(Url_Json[] urls)
        {
            List<Url_UI_Data> result = new List<Url_UI_Data>();
            if (urls != null)
            {
                foreach (var l in urls)
                {
                    if (l == null) { continue; }
                    var l_new = new Url_UI_Data
                    {
                        ticksForDateTime = new DateTime(l.ticksForDateTime),
                        url = l.url,
                        url_name = l.url_name,
                        url_state = l.url_state
                    };
                    result.Add(l_new);
                }
            }
            return result;
        }

        static Url_Json[] UI_To_Json(List<Url_UI_Data> urls)
        {
            Url_Json[] result = null;
            if (urls != null && urls.Count > 0)
            {
                result = new Url_Json[urls.Count];
                for (int i = 0; i < urls.Count; i++)
                {
                    if (urls[i] == null) { continue; }
                    var l_new = new Url_Json
                    {
                        ticksForDateTime = urls[i].ticksForDateTime.Ticks,
                        url = urls[i].url,
                        url_name = urls[i].url_name,
                        url_state = urls[i].url_state
                    };
                    result[i] = l_new;
                }
            }
            return result;
        }

        public static WikiDataJson GetJsonData(WikiUIData uiJsonData)
        {
            WikiDataJson result = new WikiDataJson
            {
                isDataProcessed = uiJsonData.isDataProcessed,
                mainNode = uiJsonData.mainNode,
                ticksAtSaveTime = uiJsonData.ticksAtSaveTime.Ticks,
                url_s = UI_To_Json(uiJsonData.url_s),
                procList = UI_To_Json(uiJsonData.procList),
            };
            return result;
        }
    }
}