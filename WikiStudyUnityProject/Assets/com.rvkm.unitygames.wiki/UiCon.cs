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
    public enum PageType { MainPage, AutoRemovedPage, ManualRemovedPage, pickedPage, ProcessingPage}
    public enum MainPageDataProcType { FromLatest, MergeAll, Browse}
    /// <summary>
    /// todo. uielem on other page, button think. WikiCon public method for UiCon, incomplete save?
    /// </summary>
    public class UiCon : MonoBehaviour
    {
        /// <summary>
        /// Reference to the app controller.
        /// </summary>
        [SerializeField] WikiCon wikiCon;
        [SerializeField] InputField mainNodeUrlInp;
        [SerializeField] Text mainNodeTxt;
        [SerializeField] Text statusEntriesTxt; //only update when we add or remove entries
        [SerializeField] Button nextBtn;
        
        /// <summary>
        /// All buttons in option
        /// </summary>
        [Header("All option buttons")]
        [SerializeField] Button backBtn;
        [SerializeField] Button latestSaveFileLoadBtn;
        [SerializeField] Button allSaveFileLoadBtn;
        [SerializeField] Button browseToOpenBtn;
        [SerializeField] Button saveBtn;
        [SerializeField] Button exitBtn;
        [SerializeField] Button homeBtn;
        [SerializeField] Button manualPageBtn;
        [SerializeField] Button autoPageBtn;
        [SerializeField] Button pickedPageBtn;

        /// <summary>
        /// References to all the pages. 
        /// </summary>
        [Header("All 'Pages' references")]
        [SerializeField] GameObject mainPage;
        [SerializeField] GameObject pickedPage;
        [SerializeField] GameObject autoBinPage;
        [SerializeField] GameObject manualBinPage;
        [SerializeField] GameObject processingPage;

        PageType currentPageType;
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

        void StartPage(bool isMain, bool isManual, bool isAuto, bool isProc, bool isPicked)
        {
            mainPage.SetActive(isMain);
            pickedPage.SetActive(isPicked);
            autoBinPage.SetActive(isAuto);
            manualBinPage.SetActive(isManual);
            processingPage.SetActive(isProc);
        }

        void InstallAllUI()
        {
            BindButton(nextBtn, () =>
            {
                if (currentPageType == PageType.MainPage)
                {
                    ProcessDataFromMainPage(MainPageDataProcType.MergeAll);
                }
                else
                {
                    OtherPageNextBtn();
                }
            });

            BindButton(backBtn, () =>
            {
                if (currentPageType == PageType.AutoRemovedPage || currentPageType == PageType.ManualRemovedPage
                || currentPageType == PageType.pickedPage)
                {
                    PaintProcessingPage();
                }
                else
                {
                    DialogueBox.ShowOk("Error!!", "This button should not be here in this page!, File a bug report to the developer!");
                }
            });


            BindButton(latestSaveFileLoadBtn, () =>
            {
                ProcessDataFromMainPage(MainPageDataProcType.FromLatest);
            });

            BindButton(allSaveFileLoadBtn, () =>
            {
                ProcessDataFromMainPage(MainPageDataProcType.MergeAll);
            });

            BindButton(browseToOpenBtn, () =>
            {
                BrowseIO.OpenFileFrom(OpenFromDirType.AutoSelect,
                (WikiDataJson data) =>
                {
                    wikiCon.JsonDataFromBrowse = data;
                    if (data == null)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        StackFrame sf = st.GetFrame(0);
                        DialogueBox.ShowOk("Error!", "invalid data! at line: " + sf.GetFileLineNumber() + " and in file: " + sf.GetFileName());
                    }
                    else
                    {
                        ProcessDataFromMainPage(MainPageDataProcType.Browse);
                    }
                });

            });

            BindButton(saveBtn, () =>
            {
                if (wikiCon.JsonData == null)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    StackFrame sf = st.GetFrame(0);
                    DialogueBox.ShowOk("Error!", "No valid data to write!"
                        + " at line: " + sf.GetFileLineNumber() + " in file: " + sf.GetFileName());
                }
                else
                {
                    bool dataWriteSuccess = true;
                    string errorMsg = "";
                    Utility.WriteDataToDevice(wikiCon.JsonData, ref dataWriteSuccess, ref errorMsg);
                    FullScreenLoadingUI.Show("Saving", "Writing Json data", 0.2f);
                    AsyncUtility.WaitXSeconds(0.3f, () =>
                    {
                        WikiDataJson allDevData = wikiCon.JsonData;
                        FullScreenLoadingUI.Show("Saving", "Writing Json data", 0.99f);
                        AsyncUtility.WaitOneFrame(() =>
                        {
                            FullScreenLoadingUI.HideIfAny();
                            if (dataWriteSuccess == false)
                            {
                                StackTrace st = new StackTrace(new StackFrame(true));
                                StackFrame sf = st.GetFrame(0);
                                DialogueBox.ShowOk("Error!", "Write failure! msg: " + errorMsg
                                    + " at line: " + sf.GetFileLineNumber() + " in file: " + sf.GetFileName());
                            }
                            else
                            {
                                DialogueBox.ShowOk("Complete!", "Data written successfully into the device!" +
                                    "");
                            }
                        });
                    });
                }
            });

            BindButton(exitBtn, () =>
            {
                DialogueBox.ShowYesNo("Confirmation!", "Any unsaved data will be lost! Are you sure!", () => { Application.Quit(); }, null);
            });

            BindButton(homeBtn, () =>
            {
                PaintHome();
            });

            BindButton(manualPageBtn, () =>
            {
                currentPageType = PageType.ManualRemovedPage;
                StartPage(false, true, false, false, false);
                PaintPageCommonUI(PageType.ManualRemovedPage);
                Paint_UI_Elements_ScrollRect(PageType.ManualRemovedPage);
            });

            BindButton(autoPageBtn, () =>
            {
                currentPageType = PageType.AutoRemovedPage;
                StartPage(false, false, true, false, false);
                PaintPageCommonUI(PageType.AutoRemovedPage);
                Paint_UI_Elements_ScrollRect(PageType.AutoRemovedPage);
            });

            BindButton(pickedPageBtn, () =>
            {
                currentPageType = PageType.pickedPage;
                StartPage(false, false, false, false, true);
                PaintPageCommonUI(PageType.pickedPage);
                Paint_UI_Elements_ScrollRect(PageType.pickedPage);
            });
        }

        void OtherPageNextBtn()
        {
            string url = wikiCon.GetCurrentUrlToProcess();
            if (Utility.IsUrlValid(url) == false)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                DialogueBox.ShowOk("Error!", "Url is invalid, please set a valid wiki url or " +
                    "check if url validator is implemented in utility class! " +
                    "Or check the proc list for debugging since this third case should not be executed at all!" +
                    " at line: "+sf.GetFileLineNumber()+" in file: "+sf.GetFileName());
            }
            else
            {
                FullScreenLoadingUI.Show("Loading", "Reading Json data", 0.2f);
                AsyncUtility.WaitXSeconds(0.3f, () =>
                {
                    var allDevData = wikiCon.JsonData;
                    FullScreenLoadingUI.Show("Loading", "Reading Json data", 0.99f);
                    AsyncUtility.WaitOneFrame(() =>
                    {
                        FullScreenLoadingUI.HideIfAny();
                        if (allDevData == null)
                        {
                            StackTrace st = new StackTrace(new StackFrame(true));
                            StackFrame sf = st.GetFrame(0);
                            //no data in device, we will create a fresh
                            DialogueBox.ShowOk("Error!", "wiki data should not be null here! " +
                                "at line: " + sf.GetFileLineNumber()+ " in file: " + sf.GetFileName() +
                            "", () =>
                            {
                                PaintHome();
                            });
                        }
                        else
                        {
                            //there are data and we will use it.
                            wikiCon.JsonData = allDevData;
                            wikiCon.UI_Data = new WikiUIData(allDevData);
                            PaintProcessingPage();
                        }
                    });
                });

            }
        }

        void ProcessDataFromMainPage(MainPageDataProcType processType)
        {
            string url = mainNodeUrlInp.text;
            if (processType == MainPageDataProcType.Browse)
            {
                MainPageInner("", MainPageDataProcType.Browse);
            }
            else
            {
                DialogueBox.ShowYesNo("Confirmation", "Will proceed with url: " + url + " ?",

                //Yes, we will proceed
                () =>
                {
                    if (Utility.IsUrlValid(url) == false)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        StackFrame sf = st.GetFrame(0);
                        DialogueBox.ShowOk("Error!", "Url is invalid, please set a valid wiki url or " +
                            "check if url validator is implemented in utility class! " +
                            "Or check the proc list for debugging since this third case should not be executed at all! at line: "
                            + sf.GetFileLineNumber()+" in file: "+sf.GetFileName());
                    }
                    else
                    {
                        MainPageInner(url, processType);
                    }
                },
                () =>
                {
                    //Nope, do nothing.
                });
            }
        }

        void MainPageInner(string urlIfAny, MainPageDataProcType processType)
        {
            bool dataFetchSuccess = true;
            string errorMsg = "";
            FullScreenLoadingUI.Show("Loading", "Reading Json data", 0.2f);
            AsyncUtility.WaitXSeconds(0.3f, () =>
            {
                WikiDataJson allDevData = null;
                if (processType == MainPageDataProcType.FromLatest)
                {
                    allDevData = Utility.MergeAllDeviceData(Utility.FormatWikiUrlIfReq(urlIfAny), ref dataFetchSuccess, ref errorMsg);
                }
                else if (processType == MainPageDataProcType.MergeAll)
                {
                    allDevData = Utility.GetLatestSave(Utility.FormatWikiUrlIfReq(urlIfAny), ref dataFetchSuccess, ref errorMsg);
                }
                else if (processType == MainPageDataProcType.Browse)
                {
                    allDevData = wikiCon.JsonDataFromBrowse;
                }
                else
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    StackFrame sf = st.GetFrame(0);
                    DialogueBox.ShowOk("Error!", "Unknown process type that wasn't implemented! at line: " 
                        + sf.GetFileLineNumber() + " in file: " + sf.GetFileName());
                    PaintHome();
                    return;
                }
                
                FullScreenLoadingUI.Show("Loading", "Reading Json data", 0.99f);
                AsyncUtility.WaitOneFrame(() =>
                {
                    FullScreenLoadingUI.HideIfAny();
                    if (dataFetchSuccess == false)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        StackFrame sf = st.GetFrame(0);
                        DialogueBox.ShowOk("Error!", "msg: " + errorMsg
                            + " at line: " + sf.GetFileLineNumber() + " in file: " + sf.GetFileName());
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
                                PaintProcessingPage();
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
                                PaintProcessingPage();
                            });
                        }
                    }
                });
            });
        }

        void PaintProcessingPage()
        {
            currentPageType = PageType.ProcessingPage;
            StartPage(false, false, false, true, false);
            PaintPageCommonUI(PageType.ProcessingPage);
            Paint_UI_Elements_ScrollRect(PageType.ProcessingPage);
        }

        void PaintHome()
        {
            currentPageType = PageType.MainPage;
            StartPage(true, false, false, false, false);
            PaintPageCommonUI(PageType.MainPage);
        }

        void PaintPageCommonUI(PageType pageType)
        {
            throw new Exception();
            if (pageType == PageType.MainPage)
            {
                
            }
        }

        void Paint_UI_Elements_ScrollRect(PageType pageType)
        {
            throw new Exception();
            if (pageType == PageType.MainPage)
            {

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
    }
}