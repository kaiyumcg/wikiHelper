using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.wiki
{
    public class WikiStat
    {
        public string mainNodeUrl;
        public bool Completed;
        public int autoCount, manualCount, pickedCount;
    }

    public static class StatUtility
    {
        public static WikiStat GetCurrentStatJsonData(WikiCon wikiCon)
        {
            string mainNodeUrl = "";
            bool Completed = false;
            int autoCount = -1, manualCount = -1, pickedCount = -1;
            if (wikiCon.UI_Data != null && wikiCon.UI_Data.url_s != null)
            {
                autoCount = manualCount = pickedCount = 0;
                mainNodeUrl = wikiCon.UI_Data.mainNode;
                Completed = wikiCon.UI_Data.isDataProcessed;
                foreach (var l in wikiCon.UI_Data.url_s)
                {
                    if (l.url_state == Url_State.Auto_NotRelated) { autoCount++; }
                    if (l.url_state == Url_State.Manual_NotRelated) { manualCount++; }
                    if (l.url_state == Url_State.Picked) { pickedCount++; }
                }
            }
            var stat = new WikiStat
            {
                mainNodeUrl = mainNodeUrl,
                Completed = Completed,
                autoCount = autoCount,
                manualCount = manualCount,
                pickedCount = pickedCount
            };
            return stat;
        }
    }
}