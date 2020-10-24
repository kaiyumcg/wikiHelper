using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using com.rvkm.unitygames.extensions.saveData;
using com.rvkm.unitygames.debug;

namespace com.rvkm.unitygames.wiki
{
    public static class WikiSaveDataUtility
    {
        public static WikiDataJson MergeAllDeviceData(string url, ref bool error, ref string errorMsg)
        {
            var allData = MinSaveDataManager.GetDataFromDeviceFiles<WikiDataJson>(ConstantManager.wikiSaveFileExtension, ref error, ref errorMsg);
            if (error) { return null; }
            return MergeAllDeviceData(allData, url, ref error, ref errorMsg);
        }

        public static List<WikiDataJson> GetMatchedDeviceData(List<WikiDataJson> allData, string url)
        {
            string r_url = url;
            List<WikiDataJson> matched = new List<WikiDataJson>();
            if (allData != null && allData.Count > 0)
            {
                foreach (var d in allData)
                {
                    if (d == null) { continue; }
                    if (d.mainNode == r_url)
                    {
                        matched.Add(d);
                    }
                }
            }
            return matched;
        }

        public static WikiDataJson GetLatestSave(string url, ref bool error, ref string errorMsg)
        {
            string r_url = url;
            var allData = MinSaveDataManager.GetDataFromDeviceFiles<WikiDataJson>(ConstantManager.wikiSaveFileExtension, ref error, ref errorMsg);
            if (error)
            {
                return null;
            }
            List<WikiDataJson> matched = GetMatchedDeviceData(allData, r_url);

            if (matched != null && matched.Count > 0)
            {
                WikiDataJson sel = matched[0];
                foreach (var m in matched)
                {
                    if (m.ticksAtSaveTime > sel.ticksAtSaveTime)
                    {
                        //so this one is latest compared to the saved one, sel. updating
                        sel = m;
                    }
                }
                return sel;
            }
            error = true;
            errorMsg += Environment.NewLine + "Can not get the latest save!";
            return null;
        }

        public static WikiDataJson MergeAllDeviceData(List<WikiDataJson> allData, string mainNodeUrl, ref bool error, ref string errorMsg)
        {
            WikiDataJson result = null;
            string r_url = mainNodeUrl;
            List<WikiDataJson> matched = GetMatchedDeviceData(allData, mainNodeUrl);

            if (matched.Count < 1)
            {
                DebugRVKM.LogRed("no data found in the device!");
                error = true;
                errorMsg += Environment.NewLine + "no data found in the device!";
                return result;
            }
            else
            {
                result = new WikiDataJson();
                result.mainNode = r_url;
                result.url_s = null;//todo

                List<Url_Json> allUrlJsons = new List<Url_Json>();
                foreach (var d in matched)
                {
                    if (d == null) { continue; }
                    if (d.url_s == null) { continue; }
                    foreach (var l in d.url_s)
                    {
                        if (l == null) { continue; }
                        DateTime dataTime = new DateTime(l.ticksForDateTime);
                        bool uriExists = false;
                        int ex_url_idx = 0;
                        for (int i = 0; i < allUrlJsons.Count; i++)
                        {
                            if (allUrlJsons[i].url == l.url)
                            {
                                ex_url_idx = i;
                                uriExists = true;
                                break;
                            }
                        }

                        if (uriExists)
                        {
                            //already list e ase
                            DateTime listTime = new DateTime(allUrlJsons[ex_url_idx].ticksForDateTime);
                            if (listTime > dataTime)
                            {
                                //list er time up to date

                            }
                            else
                            {
                                allUrlJsons[ex_url_idx].url_state = l.url_state;
                            }
                        }
                        else
                        {
                            Url_Json newL = new Url_Json();
                            newL.ticksForDateTime = l.ticksForDateTime;
                            newL.url_state = l.url_state;
                            newL.url = l.url;
                            newL.url_name = l.url_name;
                            allUrlJsons.Add(newL);
                        }
                    }
                }
                if (allUrlJsons == null || allUrlJsons.Count < 1)
                {
                    DebugRVKM.LogRed("merge operation failed. link data are empty!");
                    error = true;
                    errorMsg += Environment.NewLine + "merge operation failed. link data are empty!";
                    return null;
                }
                result.url_s = allUrlJsons.ToArray();
                return result;
            }
        }
    }
}