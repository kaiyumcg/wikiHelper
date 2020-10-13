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
        /// <summary>
        /// Reference to the app controller.
        /// </summary>
        [SerializeField] WikiCon wikiCon;
        [SerializeField] InputField mainNodeUrlInp;
        [SerializeField] Text currentNodeTxt;
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
        [SerializeField] Button resultPageBtn;

        /// <summary>
        /// References to all the pages. 
        /// </summary>
        [Header("All 'Pages' references")]
        [SerializeField] GameObject mainPage;
        [SerializeField] GameObject pickedPage;
        [SerializeField] GameObject autoBinPage;
        [SerializeField] GameObject manualBinPage;
        [SerializeField] GameObject processingPage;
        [SerializeField] GameObject resultPage;

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

        void StartPage(PageType pageType)
        {
            mainPage.SetActive(pageType == PageType.MainPage);
            pickedPage.SetActive(pageType == PageType.PickedPage);
            autoBinPage.SetActive(pageType == PageType.AutoRemovedPage);
            manualBinPage.SetActive(pageType == PageType.ManualRemovedPage);
            processingPage.SetActive(pageType == PageType.ProcessingPage);
            resultPage.SetActive(pageType == PageType.ResultPage);
        }

        void InstallAllUI()
        {
            BindButton(nextBtn, () =>
            {
                if (currentPageType == PageType.MainPage)
                {
                    DeviceWikiDataReader.OnNextButton_MainPage(DevDataReadType.MergeAll, wikiCon, mainNodeUrlInp.text, () =>
                    {
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
                else if (currentPageType == PageType.ProcessingPage)
                {
                    throw new System.NotImplementedException();
                }
                else
                {
                    DeviceWikiDataReader.OnNextButton(wikiCon, () =>
                    {
                        throw new System.NotImplementedException();
                    });
                }
            });

            BindButton(backBtn, () =>
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


            BindButton(latestSaveFileLoadBtn, () =>
            {
                string url = mainNodeUrlInp.text;
                DeviceWikiDataReader.OnNextButton_MainPage(DevDataReadType.FromLatest, wikiCon, url, () =>
                {

                });
            });

            BindButton(allSaveFileLoadBtn, () =>
            {
                string url = mainNodeUrlInp.text;
                DeviceWikiDataReader.OnNextButton_MainPage(DevDataReadType.MergeAll, wikiCon, url, () =>
                {

                });
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
                        DeviceWikiDataReader.OnNextButton_MainPage(DevDataReadType.Browse, wikiCon, );
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
                StartPage(PageType.ManualRemovedPage);
                PaintPageCommonUI(PageType.ManualRemovedPage);
                Paint_UI_Elements_ScrollRect(PageType.ManualRemovedPage);
            });

            BindButton(autoPageBtn, () =>
            {
                currentPageType = PageType.AutoRemovedPage;
                StartPage(PageType.AutoRemovedPage);
                PaintPageCommonUI(PageType.AutoRemovedPage);
                Paint_UI_Elements_ScrollRect(PageType.AutoRemovedPage);
            });

            BindButton(pickedPageBtn, () =>
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
            currentNodeTxt.text = wikiCon.GetCurrentUrlToProcess();
            var stat = Utility.GetCurrentStatJsonData(wikiCon);
            string mainStr = stat.autoCount + " M: " + stat.manualCount + " P: " + stat.pickedCount;
            statusEntriesTxt.text = stat.Completed ? "100% A: " + mainStr : mainStr;
            if (pageType == PageType.MainPage)
            {
                nextBtn
            }
        }

        void Paint_UI_Elements_ScrollRect(PageType pageType)
        {
            throw new Exception();
            if (pageType == PageType.ResultPage)
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