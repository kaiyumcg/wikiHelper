using com.rvkm.unitygames.extensions.async;
using com.rvkm.unitygames.extensions.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace com.rvkm.unitygames.wiki
{
    public enum PageType { MainPage, AutoRemovedPage, ManualRemovedPage, PickedPage, ProcessingPage, ResultPage }
    public enum DevDataReadType { FromLatest, MergeAll, Browse}
    /// <summary>
    /// todo. uielem on other page, button think. WikiCon public method for UiCon, incomplete save?
    /// </summary>
    public class UiCon : MonoBehaviour
    {
        static UiCon instance;
        /// <summary>
        /// Reference to the app controller.
        /// </summary>
        [SerializeField] WikiCon wikiCon;
        [SerializeField] MainPage mainPage;
        [SerializeField] ProcessingPage procPage;
        [SerializeField] AutomaticallyRemovedPage autoPage;
        [SerializeField] ManuallyRemovedPage manualPage;
        [SerializeField] PickedPage pickedPage;
        [SerializeField] ResultPage resultPage;
        PageType currentPageType;
        List<Url_UI_Data> currentPageTemp;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                DestroyImmediate(this);
                return;
            }
        }

        void OnEnable()
        {
            wikiCon.OnStartUI += OnStartUI;
        }

        void OnDisable()
        {
            wikiCon.OnStartUI -= OnStartUI;
        }

        void OnStartUI()
        {
            StartPage(PageType.MainPage);
            InstallAllUI();
        }

        void StartPage(PageType pageType)
        {
            currentPageType = pageType;
            mainPage.Page.SetActive(pageType == PageType.MainPage);
            pickedPage.Page.SetActive(pageType == PageType.PickedPage);
            autoPage.Page.SetActive(pageType == PageType.AutoRemovedPage);
            manualPage.Page.SetActive(pageType == PageType.ManualRemovedPage);
            procPage.Page.SetActive(pageType == PageType.ProcessingPage);
            resultPage.Page.SetActive(pageType == PageType.ResultPage);
            string urlCur = wikiCon.GetCurrentUrlToProcess();
            var stat = StatUtility.GetCurrentStatJsonData(wikiCon);
            string mainStr = stat.autoCount + " M: " + stat.manualCount + " P: " + stat.pickedCount;
            string statString = stat.Completed ? "100% A: " + mainStr : mainStr;
            mainPage.UpdateStat(urlCur, statString);
            pickedPage.UpdateStat(urlCur, statString);
            autoPage.UpdateStat(urlCur, statString);
            manualPage.UpdateStat(urlCur, statString);
            procPage.UpdateStat(urlCur, statString);
            resultPage.UpdateStat(urlCur, statString);
        }

        UiElemOnPage CloneUI_Element(GameObject prefab, Transform holder)
        {
            GameObject g = Instantiate(prefab, holder) as GameObject;
            return g.GetComponent<UiElemOnPage>();
        }

        bool IsDataPresentInTmpPage(Url_UI_Data comparer, ref int index)
        {
            bool match = false;
            index = -1;
            if (currentPageTemp != null && currentPageTemp.Count > 0)
            {
                for (int i = 0; i < currentPageTemp.Count; i++)
                {
                    if (currentPageTemp[i] != null)
                    {
                        if (currentPageTemp[i].url == comparer.url || currentPageTemp[i].url_name == comparer.url_name)
                        {
                            index = i;
                            break;
                        }
                    }
                }
                if (index >= 0)
                {
                    match = true;
                }
            }
            return match;
        }

        void RemoveEntryFromPageAndAddToUIData(Url_UI_Data comparer)
        {
            int idx = -1;
            if (IsDataPresentInTmpPage(comparer, ref idx))
            {
                wikiCon.AddTempPageElementToUIData(comparer);
                currentPageTemp.RemoveAt(idx);
            }
        }

        void InstallAllUI()
        {
            BindButton(procPage.NextBTN, () =>
            {
                AppDataManager.CheckCurrentlyLoadedData(wikiCon, (success) =>
                {
                    if (success)
                    {
                        AppDataManager.RefreshWikiJsonData(wikiCon);
                        if (wikiCon.UI_Data.isDataProcessed)
                        {
                            //go to result page
                        }
                        else
                        {
                            wikiCon.AddTempPageToUIData(currentPageTemp);
                            wikiCon.RemoveCurrentUrlFromProcList();
                            AppDataManager.RefreshWikiJsonData(wikiCon);
                            if (wikiCon.UI_Data.isDataProcessed)
                            {
                                //go to result page
                            }
                            else
                            {
                                currentPageTemp = null;
                                currentPageTemp = wikiCon.GetUrlDataForUI_From(wikiCon.GetCurrentUrlToProcess());
                                if (currentPageTemp != null && currentPageTemp.Count > 0)
                                {
                                    foreach (var urlData in currentPageTemp)
                                    {
                                        if (urlData != null)
                                        {
                                            var elem = CloneUI_Element(procPage.UI_ElementPrefab, procPage.ElementsHolder);
                                            elem.Install(urlData.url, urlData.url_name,

                                            () =>
                                            {
                                                urlData.url_state = Url_State.Picked;
                                                RemoveEntryFromPageAndAddToUIData(urlData);
                                                AppDataManager.RefreshWikiJsonData(wikiCon);
                                                DestroyImmediate(elem.gameObject);
                                            },
                                            () =>
                                            {
                                                urlData.url_state = Url_State.Manual_NotRelated;
                                                RemoveEntryFromPageAndAddToUIData(urlData);
                                                AppDataManager.RefreshWikiJsonData(wikiCon);
                                                DestroyImmediate(elem.gameObject);
                                            });

                                        }
                                    }
                                }
                                else
                                {
                                    wikiCon.UI_Data.isDataProcessed = true;
                                    AppDataManager.RefreshWikiJsonData(wikiCon);
                                    //go to result page
                                }

                                //draw the page with json ui data
                                //?? on add/remove or minus/plus hobar upon UI element's button, we need to update tick/datetime
                                //and update INIT flag to auto, manual or picked
                                //on many point we must do refresh to keep ui json data and json data in sync
                                //if all complete then do not go to processing page, rather result page.   
                            }
                        }
                    }
                });
            });

            BindButton(mainPage.FetchAndLoadBTN, () =>
            {
                string url = mainPage.MainNodeUrlInputfield.text;
                AppDataManager.LoadDataOrCreate(DevDataReadType.FromLatest, wikiCon, (success) =>
                {

                });
            });

            BindButton(mainPage.FetchAndLoadBTN, () =>
            {
                string url = mainPage.MainNodeUrlInputfield.text;
                AppDataManager.LoadDataOrCreate(DevDataReadType.MergeAll, wikiCon, (success) =>
                {

                });
            });

            BindButton(mainPage.BrowseBTN, () =>
            {
                BrowseIO.OpenFileFrom(OpenFromDirType.AutoSelect,
                (WikiDataJson data) =>
                {
                    if (data == null)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        StackFrame sf = st.GetFrame(0);
                        DialogueBox.ShowOk("Error!", "invalid data! at line: " + sf.GetFileLineNumber() + " and in file: " + sf.GetFileName());
                    }
                    else
                    {
                        AppDataManager.SetBrowsedWikiData(data);
                        AppDataManager.LoadDataOrCreate(DevDataReadType.Browse, wikiCon, (success)=> { 
                        
                        });
                    }
                });

            });

            BindButton(mainPage.SaveBTN, () =>
            {
                mainPage.SaveBTN.interactable = false;
                AppDataManager.SaveCurrentData(wikiCon, (success) =>
                {
                    mainPage.SaveBTN.interactable = true;
                });
            });

            BindButton(mainPage.ExitBTN, () =>
            {
                DialogueBox.ShowYesNo("Confirmation!", "Any unsaved data will be lost! Are you sure!", () => { Application.Quit(); }, null);
            });
        }

        void BindButton(Button b, Action callback)
        {
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() =>
            {
                callback?.Invoke();
            });
        }

        void BindButton(Button[] allBtns, Action callback)
        {
            foreach (var b in allBtns)
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() =>
                {
                    callback?.Invoke();
                });
            }
        }

        public static string GetMainNodeStr(ref string errorMsgIfAny)
        {
            if (instance == null)
            {
                errorMsgIfAny = "UiCon instance is null";
                return null;
            }
            else
            {
                if (instance.mainPage == null)
                {
                    errorMsgIfAny = "UiCon instance's main page is null";
                    return null;
                }
                else if (instance.mainPage.MainNodeUrlInputfield == null)
                {
                    errorMsgIfAny = "UiCon instance's main page's mainNodeUrlInp is null";
                    return null;
                }
                else
                {
                    errorMsgIfAny = "";
                    string inputStr = instance.mainPage.MainNodeUrlInputfield.text;
                    inputStr = UrlUtility.FormatWikiUrlCommon(inputStr);
                    if (UrlUtility.IsUrlWiki(inputStr) == false)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        StackFrame sf = st.GetFrame(0);
                        errorMsgIfAny = "there is no valid wiki link in input field! at line: "
                            + sf.GetFileLineNumber() + " in file: " + sf.GetFileName() + " in method: " + sf.GetMethod().Name;
                        return null;
                    }
                    else
                    {
                        return inputStr;
                    }
                }
            }
        }
    }
}