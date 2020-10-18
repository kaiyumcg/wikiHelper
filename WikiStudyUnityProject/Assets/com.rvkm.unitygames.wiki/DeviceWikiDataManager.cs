using com.rvkm.unitygames.extensions.async;
using com.rvkm.unitygames.extensions.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace com.rvkm.unitygames.wiki
{
    public static class DeviceWikiDataManager
    {
        public static void RefreshWikiJsonData(WikiCon wikiCon)
        {
            if (wikiCon == null)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                DialogueBox.ShowOk("!Error!", "wiki controller is null. Can not refresh! " +
                    " at line: " + sf.GetFileLineNumber()
                    + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name);
            }
            else if (wikiCon.UI_Data == null)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                DialogueBox.ShowOk("!Error!", "wiki controller's UI data is null. Can not refresh! " +
                    " at line: " + sf.GetFileLineNumber()
                    + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name);
            }
            else
            {
                bool isCompleted = wikiCon.UI_Data.procList == null || wikiCon.UI_Data.procList.Count == 0;
                wikiCon.UI_Data.isDataProcessed = isCompleted;
                wikiCon.JsonData = GetJsonData(wikiCon.UI_Data);
            }
        }

        public static void CheckCurrentlyLoadedData(WikiCon wikiCon, Action<bool> OnCompleteLoad)
        {
            string url = wikiCon.GetCurrentUrlToProcess();
            if (Utility.IsUrlWiki(url) == false)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                DialogueBox.ShowOk("Error!", "Url is invalid, at line: "
                    + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name, () =>
                    {
                        OnCompleteLoad?.Invoke(false);
                    });
            }
            else
            {
                if (wikiCon.JsonData == null)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    StackFrame sf = st.GetFrame(0);
                    //no data in device, we will create a fresh
                    DialogueBox.ShowOk("Error!", "wiki data should not be null here! " +
                        "at line: " + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name, () =>
                        {
                            OnCompleteLoad?.Invoke(false);
                        });
                }
                else
                {
                    OnCompleteLoad?.Invoke(true);
                }
            }
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

        static WikiDataJson GetJsonData(WikiUIData uiJsonData)
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

        public static void SaveCurrentData(WikiCon wikiCon, Action<bool> OnCompleteSave)
        {
            if (wikiCon.UI_Data == null)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                DialogueBox.ShowOk("Error!", "No valid UI json data to write!"
                    + " at line: " + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name, () =>
                    {
                        OnCompleteSave?.Invoke(false);
                    });
            }
            else
            {
                wikiCon.JsonData = GetJsonData(wikiCon.UI_Data);
                bool dataWriteSuccess = true;
                string errorMsg = "";
                Utility.WriteDataToDevice(wikiCon.JsonData, ref dataWriteSuccess, ref errorMsg);
                FullScreenLoadingUI.Show("Saving", "Writing Json data", 0.2f);
                AsyncUtility.WaitXSeconds(0.3f, () =>
                {
                    FullScreenLoadingUI.Show("Saving", "Writing Json data", 0.99f);
                    AsyncUtility.WaitOneFrame(() =>
                    {
                        FullScreenLoadingUI.HideIfAny();
                        if (dataWriteSuccess == false)
                        {
                            StackTrace st = new StackTrace(new StackFrame(true));
                            StackFrame sf = st.GetFrame(0);
                            DialogueBox.ShowOk("Error!", "Write failure! msg: " + errorMsg
                                + " at line: " + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name, () =>
                                {
                                    OnCompleteSave?.Invoke(false);
                                });
                        }
                        else
                        {
                            DialogueBox.ShowOk("Complete!", "Data written successfully into the device!" +
                                "", () =>
                                {
                                    OnCompleteSave?.Invoke(true);
                                });
                        }
                    });
                });
            }
        }

        static WikiDataJson browsedWikiFileIfAny;
        public static void SetBrowsedWikiData(WikiDataJson data)
        {
            browsedWikiFileIfAny = data;
        }

        public static void LoadDataOrCreate(DevDataReadType processType, WikiCon wikiCon, 
            Action<bool> OnCompleteLoad)
        {
            string mainNodeUrl = wikiCon.GetMainNodeUrl();
            string diagMsg = processType == DevDataReadType.Browse ? "Load from file? " : "Will proceed with url: " + mainNodeUrl + " ?";
            DialogueBox.ShowYesNo("Confirmation", diagMsg,

                //Yes, we will proceed
                () =>
                {
                    if (Utility.IsUrlWiki(mainNodeUrl) == false && processType != DevDataReadType.Browse)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        StackFrame sf = st.GetFrame(0);
                        DialogueBox.ShowOk("Error!", "Url is invalid, at line: "
                            + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name, () =>
                            {
                                OnCompleteLoad?.Invoke(false);
                            });
                    }
                    else
                    {
                        bool dataFetchSuccess = true;
                        string errorMsg = "";
                        WikiDataJson allDevData = null;
                        if (processType == DevDataReadType.FromLatest)
                        {
                            allDevData = Utility.MergeAllDeviceData(mainNodeUrl, ref dataFetchSuccess, ref errorMsg);
                        }
                        else if (processType == DevDataReadType.MergeAll)
                        {
                            allDevData = Utility.GetLatestSave(mainNodeUrl, ref dataFetchSuccess, ref errorMsg);
                        }
                        else if (processType == DevDataReadType.Browse)
                        {
                            if (browsedWikiFileIfAny == null)
                            {
                                StackTrace st = new StackTrace(new StackFrame(true));
                                StackFrame sf = st.GetFrame(0);
                                DialogueBox.ShowOk("Error!", "Tried to get data from file in 'Browse mode' but the file is null!" +
                                    " You probably forgot to set browsed data by calling 'SetBrowsedWikiData(data)' before this method! At line: "
                                    + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name, () =>
                                    {
                                        OnCompleteLoad?.Invoke(false);
                                    });
                                return;
                            }
                            else
                            {
                                allDevData = browsedWikiFileIfAny;
                            }
                            
                        }
                        else
                        {
                            StackTrace st = new StackTrace(new StackFrame(true));
                            StackFrame sf = st.GetFrame(0);
                            DialogueBox.ShowOk("Error!", "Unimplemented Unknown process! at line: "
                                + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name, () =>
                                {
                                    OnCompleteLoad?.Invoke(false);
                                });
                            return;
                        }

                        if (dataFetchSuccess == false)
                        {
                            StackTrace st = new StackTrace(new StackFrame(true));
                            StackFrame sf = st.GetFrame(0);
                            DialogueBox.ShowOk("Load Error!", "msg: " + errorMsg
                                + " at line: " + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name, () =>
                                {
                                    OnCompleteLoad?.Invoke(false);
                                });
                        }
                        else
                        {
                            if (allDevData == null)
                            {
                                //no data in device, we will create a fresh
                                DialogueBox.ShowOk("Complete!", "No Data in the device! " +
                                "<color='green'>We will create for you!</color>", () =>
                                {
                                    wikiCon.JsonData = wikiCon.CreateFreshWikiJsonData();
                                    wikiCon.UI_Data = new WikiUIData(wikiCon.JsonData);
                                    OnCompleteLoad?.Invoke(true);
                                });
                            }
                            else
                            {
                                DialogueBox.ShowOk("Complete!", "Found save files! We merged them and made a fresh copy!! " +
                                "", () =>
                                {
                                    //there are data and we will use it.
                                    wikiCon.JsonData = allDevData;
                                    wikiCon.UI_Data = new WikiUIData(allDevData);
                                    OnCompleteLoad?.Invoke(true);
                                });
                            }
                        }
                    }
                }, 
                //no we do not want that
                () =>
                {
                    OnCompleteLoad?.Invoke(false);
                });
        }
    }
}