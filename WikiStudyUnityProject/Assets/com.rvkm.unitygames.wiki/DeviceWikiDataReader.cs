using com.rvkm.unitygames.extensions.async;
using com.rvkm.unitygames.extensions.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace com.rvkm.unitygames.wiki
{
    public class DeviceWikiDataReader : MonoBehaviour
    {
        public static void OnNextButton(WikiCon wikiCon, Action OnCompleteLoad)
        {
            string url = wikiCon.GetCurrentUrlToProcess();
            if (Utility.IsUrlValid(url) == false)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                DialogueBox.ShowOk("Error!", "Url is invalid, at line: "
                    + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name);
            }
            else
            {
                if (wikiCon.JsonData == null)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    StackFrame sf = st.GetFrame(0);
                    //no data in device, we will create a fresh
                    DialogueBox.ShowOk("Error!", "wiki data should not be null here! " +
                        "at line: " + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name);
                }
                else
                {
                    throw new System.NotImplementedException();
                    OnCompleteLoad?.Invoke();
                }
            }
        }

        public static void OnNextButton_MainPage(DevDataReadType processType, WikiCon wikiCon, string url, 
            Action OnCompleteLoad, WikiDataJson browsedWikiFileIfAny = null)
        {
            string diagMsg = processType == DevDataReadType.Browse ? "Load from file? " : "Will proceed with url: " + url + " ?";
            DialogueBox.ShowYesNo("Confirmation", diagMsg,

                //Yes, we will proceed
                () =>
                {
                    if (Utility.IsUrlValid(url) == false && processType != DevDataReadType.Browse)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        StackFrame sf = st.GetFrame(0);
                        DialogueBox.ShowOk("Error!", "Url is invalid, at line: "
                            + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name);
                    }
                    else
                    {
                        bool dataFetchSuccess = true;
                        string errorMsg = "";
                        WikiDataJson allDevData = null;
                        if (processType == DevDataReadType.FromLatest)
                        {
                            allDevData = Utility.MergeAllDeviceData(Utility.FormatWikiUrlIfReq(url), ref dataFetchSuccess, ref errorMsg);
                        }
                        else if (processType == DevDataReadType.MergeAll)
                        {
                            allDevData = Utility.GetLatestSave(Utility.FormatWikiUrlIfReq(url), ref dataFetchSuccess, ref errorMsg);
                        }
                        else if (processType == DevDataReadType.Browse)
                        {
                            allDevData = browsedWikiFileIfAny;
                        }
                        else
                        {
                            StackTrace st = new StackTrace(new StackFrame(true));
                            StackFrame sf = st.GetFrame(0);
                            DialogueBox.ShowOk("Error!", "Unimplemented Unknown process! at line: "
                                + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name);
                            return;
                        }

                        if (dataFetchSuccess == false)
                        {
                            StackTrace st = new StackTrace(new StackFrame(true));
                            StackFrame sf = st.GetFrame(0);
                            DialogueBox.ShowOk("Load Error!", "msg: " + errorMsg
                                + " at line: " + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name);
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
                                    throw new System.NotImplementedException();
                                    OnCompleteLoad?.Invoke();
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
                                    throw new System.NotImplementedException();
                                    OnCompleteLoad?.Invoke();
                                });
                            }
                        }
                    }
                });
        }
    }
}