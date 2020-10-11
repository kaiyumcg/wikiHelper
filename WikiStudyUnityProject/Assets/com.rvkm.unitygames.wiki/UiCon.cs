using com.rvkm.unitygames.extensions.async;
using com.rvkm.unitygames.extensions.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.rvkm.unitygames.wiki
{
    public enum PageType { MainPage, AutoRemovedPage, ManualRemovedPage, pickedPage, ProcessingPage}
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
        [SerializeField] Button plusPageBtn;

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
            InstallAllUI();

        }

        void StartPage(bool isMain, bool isManual, bool isAuto, bool isProc, bool isPicked)
        {
            mainPage.SetActive(isMain);
            pickedPage.SetActive(isPicked);
            autoBinPage.SetActive(isAuto);
            manualBinPage.SetActive(isManual);
            processingPage.SetActive(isProc);

            if (isMain)
            {
                currentPageType = PageType.MainPage;
                //todo
            }
        }

        void InstallAllUI()
        {
            BindButton(nextBtn, () =>
            {
                //if the proc list is empty or if the data is completed then we save data to disk and display result page
                //or always go to processing page with first proc list element

                string url = currentPageType == PageType.MainPage ? mainNodeUrlInp.text : wikiCon.GetCurrentUrlToProcess();//todo
                DialogueBox.ShowYesNo("Confirmation", "Will proceed with url: " + url,

                    //Yes, we will proceed
                    () =>
                    {
                        ProcessUrlNextBtn(url);
                    }, 
                    () =>
                    {
                        //Nope, do nothing.
                    });
            });
        }

        void ProcessUrlNextBtn(string url, bool isMainPage)
        {
            if (Utility.IsUrlValid(url) == false)
            {
                DialogueBox.ShowOk("Error!", "Url is invalid, please set a valid wiki url or " +
                    "check if url validator is implemented in utility class! " +
                    "Or check the proc list for debugging since this third case should not be executed at all!");
            }
            else
            {
                bool dataFetchSuccess = true;
                string errorMsg = "";
                FullScreenLoadingUI.Show("Loading", "Reading Json data", 0.2f);
                AsyncUtility.WaitXSeconds(0.3f, () => {

                    var allDevData = isMainPage ?
                    Utility.MergeAllDeviceData(Utility.FormatWikiUrlIfReq(url), ref dataFetchSuccess, ref errorMsg) : wikiCon.JsonData;
                    FullScreenLoadingUI.Show("Loading", "Reading data", 0.99f);
                    AsyncUtility.WaitOneFrame(() =>
                    {
                        if (dataFetchSuccess == false)
                        {
                            DialogueBox.ShowOk("Error!", "msg: " + errorMsg);
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
                                    });
                            }
                            else
                            {
                                //there are data and we will use it.
                                wikiCon.JsonData = allDevData;
                                wikiCon.UI_Data = new WikiUIData(allDevData);
                            }
                        }
                    });
                });

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