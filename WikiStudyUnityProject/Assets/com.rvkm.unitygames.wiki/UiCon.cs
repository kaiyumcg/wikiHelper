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
            currentPageType = PageType.MainPage;
            PaintHome();
            InstallAllUI();
        }

        void StartPage(PageType pageType)
        {
            mainPage.Page.SetActive(pageType == PageType.MainPage);
            pickedPage.Page.SetActive(pageType == PageType.PickedPage);
            autoPage.Page.SetActive(pageType == PageType.AutoRemovedPage);
            manualPage.Page.SetActive(pageType == PageType.ManualRemovedPage);
            procPage.Page.SetActive(pageType == PageType.ProcessingPage);
            resultPage.Page.SetActive(pageType == PageType.ResultPage);
        }

        void InstallAllUI()
        {
            BindButton(procPage.NextBTN, () =>
            {
                if (currentPageType == PageType.MainPage)
                {
                    DeviceWikiDataManager.LoadDataOrCreate(DevDataReadType.MergeAll, wikiCon, (success) =>
                    {
                        DeviceWikiDataManager.RefreshWikiJsonData(wikiCon);
                        if (wikiCon.UI_Data.isDataProcessed)
                        {
                            //go to result page
                        }
                        else
                        {
                            //do things
                            //go to processing page
                        }
                        throw new System.NotImplementedException();
                    });
                }
                else if (currentPageType == PageType.ResultPage)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    StackFrame sf = st.GetFrame(0);
                    DialogueBox.ShowOk("Error!", "This button should not be here in this page. Report bug to developer!" +
                        "in line: "+sf.GetFileLineNumber()+" at file: "+sf.GetFileName());
                }
                else
                {
                    DeviceWikiDataManager.CheckCurrentlyLoadedData(wikiCon, (success) =>
                    {
                        DeviceWikiDataManager.RefreshWikiJsonData(wikiCon);
                        if (wikiCon.UI_Data.isDataProcessed)
                        {
                            //go to result page
                        }
                        else
                        {
                            if (currentPageType == PageType.ProcessingPage)
                            {
                                wikiCon.RemoveCurrentUrlFromProcList();
                                wikiCon.AddTempPageToUIData(currentPageTemp);
                                DeviceWikiDataManager.RefreshWikiJsonData(wikiCon);
                                currentPageTemp = wikiCon.GetUrlDataForUI_From(wikiCon.GetCurrentUrlToProcess());
                                //draw the page with json ui data
                                //?? on add/remove or minus/plus hobar upon UI element's button, we need to update tick/datetime
                                //and update INIT flag to auto, manual or picked
                                //on many point we must do refresh to keep ui json data and json data in sync
                                //if all complete then do not go to processing page, rather result page.
                            }
                        }
                        
                        throw new System.NotImplementedException();
                    });
                }
            });

            BindButton(autoPage.BackBTN, () =>
            {
                if (currentPageType == PageType.AutoRemovedPage || currentPageType == PageType.ManualRemovedPage
                || currentPageType == PageType.PickedPage)
                {
                    PaintProcessingPage();
                }
                else
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    StackFrame sf = st.GetFrame(0);
                    DialogueBox.ShowOk("Error!!", "This button should not be here in this page!, " +
                        "File a bug report to the developer! in line: "+sf.GetFileLineNumber()+" at: "+sf.GetFileName());
                }
            });


            BindButton(mainPage.FetchAndLoadBTN, () =>
            {
                string url = mainPage.MainNodeUrlInputfield.text;
                DeviceWikiDataManager.LoadDataOrCreate(DevDataReadType.FromLatest, wikiCon, (success) =>
                {

                });
            });

            BindButton(mainPage.FetchAndLoadBTN, () =>
            {
                string url = mainPage.MainNodeUrlInputfield.text;
                DeviceWikiDataManager.LoadDataOrCreate(DevDataReadType.MergeAll, wikiCon, (success) =>
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
                        DeviceWikiDataManager.SetBrowsedWikiData(data);
                        DeviceWikiDataManager.LoadDataOrCreate(DevDataReadType.Browse, wikiCon, (success)=> { 
                        
                        });
                    }
                });

            });

            BindButton(mainPage.SaveBTN, () =>
            {
                mainPage.SaveBTN.interactable = false;
                DeviceWikiDataManager.SaveCurrentData(wikiCon, (success) =>
                {
                    mainPage.SaveBTN.interactable = true;
                });
            });

            BindButton(mainPage.ExitBTN, () =>
            {
                DialogueBox.ShowYesNo("Confirmation!", "Any unsaved data will be lost! Are you sure!", () => { Application.Quit(); }, null);
            });

            BindButton(resultPage.HomeBTN, () =>
            {
                PaintHome();
            });

            BindButton(procPage.ManualPageBTN, () =>
            {
                currentPageType = PageType.ManualRemovedPage;
                StartPage(PageType.ManualRemovedPage);
                PaintPageCommonUI(PageType.ManualRemovedPage);
                Paint_UI_Elements_ScrollRect(PageType.ManualRemovedPage);
            });

            BindButton(procPage.AutoPageBTN, () =>
            {
                currentPageType = PageType.AutoRemovedPage;
                StartPage(PageType.AutoRemovedPage);
                PaintPageCommonUI(PageType.AutoRemovedPage);
                Paint_UI_Elements_ScrollRect(PageType.AutoRemovedPage);
            });

            BindButton(procPage.PickedPageBTN, () =>
            {
                currentPageType = PageType.PickedPage;
                StartPage(PageType.PickedPage);
                PaintPageCommonUI(PageType.PickedPage);
                Paint_UI_Elements_ScrollRect(PageType.PickedPage);
            });
        }

        void PaintProcessingPage()
        {
            currentPageType = PageType.ProcessingPage;
            StartPage(PageType.ProcessingPage);
            PaintPageCommonUI(PageType.ProcessingPage);
            Paint_UI_Elements_ScrollRect(PageType.ProcessingPage);
        }

        void PaintHome()
        {
            currentPageType = PageType.MainPage;
            StartPage(PageType.MainPage);
            PaintPageCommonUI(PageType.MainPage);
        }

        void PaintPageCommonUI(PageType pageType)
        {
            mainPage.MainNodeText.text = wikiCon.GetCurrentUrlToProcess();
            var stat = Utility.GetCurrentStatJsonData(wikiCon);
            string mainStr = stat.autoCount + " M: " + stat.manualCount + " P: " + stat.pickedCount;
            mainPage.StatusEntriesText.text = stat.Completed ? "100% A: " + mainStr : mainStr;
            if (pageType == PageType.MainPage)
            {
                throw new NotImplementedException();
            }
        }

        void Paint_UI_Elements_ScrollRect(PageType pageType)
        {
            throw new NotImplementedException();
            if (pageType == PageType.ResultPage)
            {
                throw new NotImplementedException();
            }
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
                    inputStr = Utility.FormatWikiUrlCommon(inputStr);
                    if (Utility.IsUrlWiki(inputStr) == false)
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