using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;
using com.rvkm.unitygames.extensions;

namespace com.rvkm.unitygames.wiki
{
    public class WikiStat
    {
        public string mainNodeUrl;
        public bool Completed;
        public int autoCount, manualCount, pickedCount;
    }

    public class Utility
    {
        static bool logStateUtility = true;
        public static void SetLogState(bool enabled)
        {
            logStateUtility = enabled;
        }

        public static List<WikiDataJson> GetDataFromDeviceFiles(ref bool error, ref string errorMsg)
        {
            List<WikiDataJson> result = new List<WikiDataJson>();
            string dir = "";
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
            dir = Application.persistentDataPath;
#elif UNITY_STANDALONE
        dir = Application.dataPath;
#endif
            GameDebug.LogBlack("app dir: " + dir, logStateUtility);
            DirectoryInfo nfo = new DirectoryInfo(dir);
            if (nfo.Exists == false) { return result; }
            FileInfo[] jsonFiles = nfo.GetFiles("*.wrm", SearchOption.AllDirectories);
            if (jsonFiles == null || jsonFiles.Length < 1) { return result; }

            foreach (var j in jsonFiles)
            {
                string jsonStr = "";
                try
                {
                    jsonStr = File.ReadAllText(j.FullName);
                }
                catch (System.Exception ex)
                {
                    GameDebug.LogRed("error while reading .wrm files----will return empty data" +
                        "------file path: " + j.FullName + "-----error: " + ex.Message, logStateUtility);
                    result = new List<WikiDataJson>();

                    error = true;
                    errorMsg += Environment.NewLine + "error while reading .wrm files----will return empty data" +
                        "------file path: " + j.FullName + "-----error: " + ex.Message;
                    return result;
                    throw;
                }

                WikiDataJson dataFromJson = null;
                try
                {
                    dataFromJson = JsonUtility.FromJson<WikiDataJson>(jsonStr);
                }
                catch (System.Exception ex)
                {
                    GameDebug.LogRed("error while reading .wrm json content----will return empty data" +
                        "-----file path: " + j.FullName + "-----error: " + ex.Message, logStateUtility);
                    result = new List<WikiDataJson>();

                    error = true;
                    errorMsg += Environment.NewLine + "error while reading .wrm json content----will return empty data" +
                        "-----file path: " + j.FullName + "-----error: " + ex.Message;

                    return result;
                    throw;
                }
                GameDebug.LogGreen("json obtained from path: " + j.FullName, logStateUtility);
                result.Add(dataFromJson);
            }
            return result;
        }

        public static string FormatWikiUrlIfReq(string url)
        {
            string r_url = Regex.Replace(url, "http://", "");
            r_url = Regex.Replace(r_url, "https://", "");
            r_url = Regex.Replace(r_url, "en.wikipedia.org", "");
            r_url = Regex.Replace(r_url, "en.m.wikipedia.org", "");
            r_url = r_url.TrimUrlSlashesFromStart();
            r_url = r_url.TrimUrlSlashesFromEnd();
            return r_url;
        }

        public static WikiStat GetCurrentStatJsonData(WikiCon wikiCon)
        {
            throw new NotImplementedException();
        }

        public static string GetFullWikiUrlIfReq(string url)
        {
            bool isMobile = false;
#if UNITY_IOS || UNITY_ANDROID
        isMobile = true;
#endif
            string mainDomainFull = isMobile ? "https://en.m.wikipedia.org" : "https://en.wikipedia.org";
            string formattedUrl = FormatWikiUrlIfReq(url);

            mainDomainFull = mainDomainFull.TrimUrlSlashesFromEnd();
            formattedUrl = formattedUrl.TrimUrlSlashesFromStart();
            formattedUrl = formattedUrl.TrimUrlSlashesFromEnd();


            //Utility.LogYellow("main domain url: " + mainDomainFull + " and formatted url: " + formattedUrl);
            string newUrl = mainDomainFull + "/" + formattedUrl; //Path.Combine(mainDomainFull, formattedUrl);
            GameDebug.LogYellow("the full wiki url is: " + newUrl, logStateUtility);
            return newUrl;
        }

        public static WikiDataJson MergeAllDeviceData(string url, ref bool error, ref string errorMsg)
        {
            var allData = Utility.GetDataFromDeviceFiles(ref error, ref errorMsg);
            if (error) { return null; }
            return Utility.MergeAllDeviceData(allData, url, ref error, ref errorMsg);
        }

        public static List<WikiDataJson> GetMatchedDeviceData(List<WikiDataJson> allData, string url)
        {
            string r_url = Utility.FormatWikiUrlIfReq(url);
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
            string r_url = Utility.FormatWikiUrlIfReq(url);
            var allData = Utility.GetDataFromDeviceFiles(ref error, ref errorMsg);
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
            string r_url = Utility.FormatWikiUrlIfReq(mainNodeUrl);
            List<WikiDataJson> matched = GetMatchedDeviceData(allData, mainNodeUrl);

            if (matched.Count < 1)
            {
                GameDebug.LogRed("no data found in the device!", logStateUtility);
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
                    GameDebug.LogRed("merge operation failed. link data are empty!", logStateUtility);
                    error = true;
                    errorMsg += Environment.NewLine + "merge operation failed. link data are empty!";
                    return null;
                }
                result.url_s = allUrlJsons.ToArray();
                return result;
            }
        }

        public static void WriteDataToDevice(WikiDataJson data, ref bool error, ref string errorMsg)
        {
            if (data == null)
            {
                GameDebug.LogRed("invalid data. Write to device operation failed!", logStateUtility);
                error = true;
                errorMsg += "invalid data. Write to device operation failed!";
                return;
            }
            data.ticksAtSaveTime = DateTime.Now.Ticks;
            var json = JsonUtility.ToJson(data);

            string fPath = "";

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
            fPath = Path.Combine(Application.persistentDataPath, "" + DateTime.Now.Ticks + "_dt.wrm");
#elif UNITY_STANDALONE
        fPath = Path.Combine(Application.dataPath, "" + DateTime.Now.Ticks + "_dt.wrm");
#endif

            File.WriteAllText(fPath, json);
        }

        public static bool IsUrlValid(string url)
        {
            //todo
            throw new Exception();
        }

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

        public static Url_Json[] UI_To_Json(List<Url_UI_Data> urls)
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
    }
}