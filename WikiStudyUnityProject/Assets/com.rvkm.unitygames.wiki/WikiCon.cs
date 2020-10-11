using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.wiki
{
    [System.Serializable]
    public class LinkDesc
    {
        public string linkTitle, link;
    }

    public class WikiCon : MonoBehaviour
    {
        [SerializeField] string wikiNodeMain = "";
        public Action OnStartUI;
        public WikiDataJson JsonData { get; set; }
        public WikiUIData UI_Data { get; set; }
        public string CurrentUrl { get; set; }
        public List<LinkDesc> links = new List<LinkDesc>();
        // Start is called before the first frame update
        void Start()
        {

        }

        public WikiDataJson CreateFreshWikiJsonData()
        {
            throw new Exception();
        }

        public string GetCurrentUrlToProcess()
        {
            throw new Exception();
        }

        void DoIt()
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load(wikiNodeMain);
            links = new List<LinkDesc>();
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                var l = new LinkDesc();
                foreach (var at in link.Attributes)
                {
                    if (at.Name == "title")
                    {
                        l.linkTitle = at.Value;
                        break;
                    }
                }

                foreach (var at in link.Attributes)
                {
                    if (at.Name == "href")
                    {
                        l.link = at.Value;
                        break;
                    }
                }

                if (!(l.link.Contains("/wiki"))) { continue; }
                //if (!(l.link.Contains("https://") || l.link.Contains("http://") || l.link.Contains("/wiki"))) { continue; }

                links.Add(l);

                /*
                foreach (var at in link.Attributes)
                {
                    if (!(at.Name == "href" || at.Name == "title")) { continue; }
                    var l = new LinkDesc();
                    l.linkName = at.Name;
                    l.linkValue = at.Value;
                    links.Add(l);
                }
                */
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DoIt();
            }
        }
    }
}