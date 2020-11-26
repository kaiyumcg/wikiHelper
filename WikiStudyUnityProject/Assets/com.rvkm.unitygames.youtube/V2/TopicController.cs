using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

namespace com.rvkm.unitygames.YouTubeV2
{
    public class TopicController : MonoBehaviour
    {
        [SerializeField] VideoInfo channelInfo;
        [SerializeField] SearchKeywordList keywords;
        [SerializeField] TextAsset ignoreWordList;
        [SerializeField] TopicList output;

        List<string> ignoreList;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ProcessForKeywords();
                Debug.Log("Done!!");
            }
        }

        

        void ProcessForKeywords()
        {
            output.topics = new List<string>();
            ignoreList = new List<string>();

            if (ignoreWordList == null)
            {
                throw new Exception("there isn't any global ignore list defined and referenced in this component!");
            }
            string igStr = ignoreWordList.text;
            string[] allIgWords = Regex.Split(igStr, Environment.NewLine);
            if (allIgWords != null && allIgWords.Length > 0)
            {
                Debug.Log("ignore word list file has " + allIgWords.Length + " lines!");
                foreach (var s in allIgWords)
                {
                    if (string.IsNullOrEmpty(s) || ignoreList.ContainsCaseInsensitive(s)) { continue; }
                    if (s.ContainsCaseInsensitive(" "))
                    {
                        var innerParts = Regex.Split(s, " ");
                        if (innerParts != null && innerParts.Length > 0)
                        {
                            foreach (var s_inner in innerParts)
                            {
                                if (string.IsNullOrEmpty(s_inner) || ignoreList.ContainsCaseInsensitive(s_inner)) { continue; }
                                ignoreList.Add(s_inner);
                                ignoreList.Add(s_inner+":");
                            }
                        }
                    }
                    else
                    {
                        ignoreList.Add(s);
                        ignoreList.Add(s + ":");
                    }
                    

                }
            }

            var invalidIgnoreWordPresent = ignoreList.Exists((str) => { return string.IsNullOrEmpty(str) || str == " " || str == Environment.NewLine; });
            if (invalidIgnoreWordPresent)
            {
                throw new Exception("check the ignore list, it is invalid!");
            }

            List<string> store = new List<string>();
            if (channelInfo != null && channelInfo.allVideoInfo != null && channelInfo.allVideoInfo.Length > 0)
            {
                foreach (var v in channelInfo.allVideoInfo)
                {
                    if (v == null) { continue; }
                    string title = v.linkName;
                    string[] parts = Regex.Split(title, " ");
                    if (parts != null && parts.Length > 0)
                    {
                        foreach (var s in parts)
                        {
                            if (string.IsNullOrEmpty(s) 
                                || store.ContainsCaseInsensitive(s) 
                                || ignoreList.ContainsCaseInsensitive(s)
                                || s.IsItNeumeric()
                                || s.Length <= output.minTopicLength) { continue; }

                            if (keywords != null)
                            {
                                var foundInWL = keywords.whiteList.Exists((str) =>
                                { return string.Equals(str, s, StringComparison.CurrentCultureIgnoreCase); });
                                var foundInBL = keywords.blackList.Exists((str) =>
                                { return string.Equals(str, s, StringComparison.CurrentCultureIgnoreCase); });
                                if (foundInWL)
                                {
                                    store.Add(s);
                                }
                                else if (foundInBL)
                                {

                                }
                                else
                                {
                                    store.Add(s);
                                }
                            }
                            else
                            {
                                store.Add(s);
                            }
                        }
                    }
                }
            }

            foreach (var s in store)
            {
                output.topics.Add(s);
            }
        }
    }
}