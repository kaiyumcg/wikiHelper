using com.rvkm.unitygames.extensions.UI;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace com.rvkm.unitygames.wiki
{
    public class WikiCon : MonoBehaviour
    {
        [SerializeField] string wikiNodeMain = "";
        public Action OnStartUI;
        public WikiDataJson JsonData { get; set; }
        public WikiUIData UI_Data { get; set; }

        public WikiDataJson CreateFreshWikiJsonData()
        {
            string errMsg = "";
            string mainNodeStr = UiCon.GetMainNodeStr(ref errMsg);
            if (string.IsNullOrEmpty(mainNodeStr))
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                DialogueBox.ShowOk("Error!", "Error getting main node from UI controller, " +
                    "please give any valid input and/or check error message. Msg: " + errMsg +
                    "at line: " + sf.GetFileLineNumber() +
                    " in file: " + sf.GetFileName() +
                    " in method: " + sf.GetMethod().Name);
                return null;
            }
            else
            {
                var data = new WikiDataJson
                {
                    isDataProcessed = false,
                    mainNode = mainNodeStr,
                    ticksAtSaveTime = -1,
                    procList = new Url_Json[0],
                    url_s = new Url_Json[0]
                };
                return data;
            }
            
        }

        public string GetCurrentUrlToProcess()
        {
            if (UI_Data == null)
            {
                
            }
            else if(UI_Data.)
        }

        public string GetMainNodeUrl()
        {
            if (JsonData == null)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                DialogueBox.ShowOk("Error!", "Main Json data is null." +
                    "at line: " + sf.GetFileLineNumber() +
                    " in file: " + sf.GetFileName() +
                    " in method: " + sf.GetMethod().Name);
                return null;
            }
            else
            {
                return JsonData.mainNode;
            }
        }

        public bool IsValidUrlToConsider(string url, string url_name)
        {
            bool result = false;
            if (url.Contains("/wiki")) 
            {
                if (UI_Data.url_s != null && UI_Data.url_s.Count > 0)
                {
                    //possible scope of optimization but I dont care this now.

                    bool isBlackListed = false;
                    foreach (var l in UI_Data.url_s)
                    {
                        if (l == null) { continue; }
                        bool isFlagIgnore = l.url_state == Url_State.Auto_NotRelated || l.url_state == Url_State.Manual_NotRelated;
                        bool canBeSameUrl = l.url == url || l.url_name == url_name;
                        if (isFlagIgnore && canBeSameUrl)
                        {
                            isBlackListed = true;
                            break;
                        }
                    }
                    
                    bool isAlreadyProcessed = false;
                    foreach (var l in UI_Data.url_s)
                    {
                        if (l == null) { continue; }
                        bool isPickable = l.url_state == Url_State.Picked;
                        bool isSameUrl = l.url == url && l.url_name == url_name;
                        if (isPickable && isSameUrl)
                        {
                            isAlreadyProcessed = true;
                            break;
                        }
                    }

                    if (!(isAlreadyProcessed || isBlackListed))
                    {
                        result = true;
                    }

                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        public List<Url_UI_Data> GetUrlDataForUI_From(string url)
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load(wikiNodeMain);
            var result = new List<Url_UI_Data>();
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                string linkName = ""; 
                string linkUrl = "";
                foreach (var at in link.Attributes)
                {
                    if (at.Name == "title")
                    {
                        linkName = at.Value;
                        break;
                    }
                }

                foreach (var at in link.Attributes)
                {
                    if (at.Name == "href")
                    {
                        linkUrl = at.Value;//??
                        break;
                    }
                }

                if (IsValidUrlToConsider(linkUrl, linkName))
                {
                    var l = new Url_UI_Data { ticksForDateTime = DateTime.Now, url = linkUrl, url_name = linkName, url_state = Url_State.INIT };
                    result.Add(l);
                }
            }

            return result;
        }
    }
}