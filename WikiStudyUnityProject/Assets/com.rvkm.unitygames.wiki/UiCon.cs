using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.rvkm.unitygames.wiki
{
    /// <summary>
    /// todo. uielem on other page, button think. WikiCon public method for UiCon, incomplete save?
    /// </summary>
    public class UiCon : MonoBehaviour
    {
        /// <summary>
        /// Reference to the app controller.
        /// </summary>
        [SerializeField] WikiCon wikiCon;

        /// <summary>
        /// Main page's input field to take input from the user for the wiki link to process.
        /// </summary>
        [SerializeField] InputField mainNodeUrlInp;
        [SerializeField] Text mainNodeTxt;

        /// <summary>
        /// display status of entries all the pages, these are page specific UI, rather global.
        /// </summary>
        [Header("Entries Count UI")]
        [SerializeField] Text autoEntries;
        [SerializeField] Text manualEntries;
        [SerializeField] Text basketEntries;

        /// <summary>
        /// Main page buttons. AutoLoad to load the latest save file.
        /// FullLoad to try to load all save files for the given link in inputfield.
        /// Browse to open from system UI. 
        /// Submit to submit from input field. Save to save the progress to a file. 
        /// Exit to exit app and it will auto save the progress to file.
        /// </summary>
        [Header("Main Page Buttons")]
        [SerializeField] Button autoLoadBtn;
        [SerializeField] Button fullLoadBtn;
        [SerializeField] Button browseToOpenBtn;
        [SerializeField] Button submitBtn;
        [SerializeField] Button saveBtn;
        [SerializeField] Button exitBtn;

        /// <summary>
        /// Processing page buttons. Next to process into the next iterations. Home to go back to home screen, progress will be saved.
        /// ManualPage to go to visit manually selected ignore link list page. 
        /// AutoPage to visit automatically selected ignore link list page.
        /// PlusPage to visit picked or plus link list page.
        /// </summary>
        [Header("Processing Page Buttons")]
        [SerializeField] Button nextBtn;
        [SerializeField] Button homeBtn;
        [SerializeField] Button manualPageBtn;
        [SerializeField] Button autoPageBtn;
        [SerializeField] Button plusPageBtn;


        /// <summary>
        /// Manual page button. Back to processing page.
        /// Auto page button. Back to processing page.
        /// Plus page button. Back to processing page.
        /// </summary>
        [Header("All 'Back' buttons")]
        [SerializeField] Button backFromManualPageBtn;
        [SerializeField] Button backFromAutoPageBtn;
        [SerializeField] Button backFromPlusPageBtn;


        /// <summary>
        /// References to all the pages. 
        /// </summary>
        [Header("All 'Pages' references")]
        [SerializeField] GameObject mainPage;
        [SerializeField] GameObject pickedPage;
        [SerializeField] GameObject autoBinPage;
        [SerializeField] GameObject manualBinPage;
        [SerializeField] GameObject processingPage;

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
            StartPage(true, false, false, false, false);
            autoLoadBtn.onClick.RemoveAllListeners();
            autoLoadBtn.onClick.AddListener(() =>
            {
                autoLoadBtn.interactable = false;

                //show loading
                //do load async
                //and after that
                StartPage(false, false, false, true, false);
            });

        }

        void StartPage(bool isMain, bool isManual, bool isAuto, bool isProc, bool isPicked)
        {
            mainPage.SetActive(isMain);
            pickedPage.SetActive(isPicked);
            autoBinPage.SetActive(isAuto);
            manualBinPage.SetActive(isManual);
            processingPage.SetActive(isProc);
        }
    }
}