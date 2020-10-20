using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.rvkm.unitygames.wiki
{
    [System.Serializable]
    public class APage
    {
        [Header("Common references")]
        [SerializeField] GameObject page;
        [SerializeField] Button optionsButton, saveButton, exitButton;
        [SerializeField] Text mainNodeText, statusEntriesText;

        //Access points
        public GameObject Page { get { return page; } }
        public Button OptionsBTN { get { return optionsButton; } }
        public Button SaveBTN { get { return saveButton; } }
        public Button ExitBTN { get { return exitButton; } }
        public Text MainNodeText { get { return mainNodeText; } }
        public Text StatusEntriesText { get { return statusEntriesText; } }
        
    }

    [System.Serializable]
    public class MainPage : APage
    {
        [Header("Main page specific references")]
        [SerializeField] Button fetchAndLoadButton;
        [SerializeField] Button browseButton, deleteSaveFileButton, submitButton;
        [SerializeField] InputField mainNodeUrlInputfield;
        [SerializeField] Dropdown fetchModeDropDown;

        //Access points
        public Button FetchAndLoadBTN { get { return fetchAndLoadButton; } }
        public Button BrowseBTN { get { return browseButton; } }
        public Button DeleteSaveFileBTN { get { return deleteSaveFileButton; } }
        public Button SubmitBTN { get { return submitButton; } }
        public InputField MainNodeUrlInputfield { get { return mainNodeUrlInputfield; } }
        public Dropdown FetchModeDropDown { get { return fetchModeDropDown; } }
    }

    [System.Serializable]
    public class AutomaticallyRemovedPage : APage
    {
        [Header("Automatically removed entry page specific references")]
        [SerializeField] Button backButton;
        [SerializeField] Button homeButton;
        [SerializeField] GameObject ui_ElementPrefab;
        [SerializeField] Transform elementsHolder;

        //Access points
        public Button BackBTN { get { return backButton; } }
        public Button HomeBTN { get { return homeButton; } }
        public GameObject UI_ElementPrefab { get { return ui_ElementPrefab; } }
        public Transform ElementsHolder { get { return elementsHolder; } }
    }

    [System.Serializable]
    public class ManuallyRemovedPage : APage
    {
        [Header("Manually removed entry page specific references")]
        [SerializeField] Button backButton;
        [SerializeField] Button homeButton;
        [SerializeField] GameObject ui_ElementPrefab;
        [SerializeField] Transform elementsHolder;

        //Access points
        public Button BackBTN { get { return backButton; } }
        public Button HomeBTN { get { return homeButton; } }
        public GameObject UI_ElementPrefab { get { return ui_ElementPrefab; } }
        public Transform ElementsHolder { get { return elementsHolder; } }
    }

    [System.Serializable]
    public class PickedPage : APage
    {
        [Header("Picked entry page specific references")]
        [SerializeField] Button backButton;
        [SerializeField] Button homeButton;
        [SerializeField] GameObject ui_ElementPrefab;
        [SerializeField] Transform elementsHolder;

        //Access points
        public Button BackBTN { get { return backButton; } }
        public Button HomeBTN { get { return homeButton; } }
        public GameObject UI_ElementPrefab { get { return ui_ElementPrefab; } }
        public Transform ElementsHolder { get { return elementsHolder; } }
    }

    [System.Serializable]
    public class ResultPage : APage
    {
        [Header("Result page specific references")]
        [SerializeField] Button homeButton;
        [SerializeField] GameObject ui_ElementPrefab;
        [SerializeField] Transform elementsHolder;

        //Access point
        public Button HomeBTN { get { return homeButton; } }
        public GameObject UI_ElementPrefab { get { return ui_ElementPrefab; } }
        public Transform ElementsHolder { get { return elementsHolder; } }
    }

    [System.Serializable]
    public class ProcessingPage : APage
    {
        [Header("Processing page specific references")]
        [SerializeField] Button manualPageButton;
        [SerializeField] Button autoPageButton, pickedPageButton, nextButton, homeButton;
        [SerializeField] GameObject ui_ElementPrefab;
        [SerializeField] Transform elementsHolder;

        //Access points
        public Button ManualPageBTN { get { return manualPageButton; } }
        public Button AutoPageBTN { get { return autoPageButton; } }
        public Button PickedPageBTN { get { return pickedPageButton; } }
        public Button NextBTN { get { return nextButton; } }
        public Button HomeBTN { get { return homeButton; } }
        public GameObject UI_ElementPrefab { get { return ui_ElementPrefab; } }
        public Transform ElementsHolder { get { return elementsHolder; } }
    }
}