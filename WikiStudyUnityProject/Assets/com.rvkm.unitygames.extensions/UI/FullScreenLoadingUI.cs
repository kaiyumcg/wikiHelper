using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace com.rvkm.unitygames.extensions.UI
{
    public class FullScreenLoadingUI : MonoBehaviour
    {
        [SerializeField] GameObject loaderFullScreen;
        [SerializeField] Text progressTxt, headerTxt, subHeaderTxt;
        [SerializeField] Canvas canvas;
        static FullScreenLoadingUI instance;
        const string prefabFullPath = "rvkm/UI/_LoadingUI_fullscreen";

        private void Awake()
        {
            InitScriptIfReq();
        }

        void InitScriptIfReq()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                if (FullScreenLoadingUI.instance != this)
                {
                    DestroyImmediate(this);
                    return;
                }
            }
        }

        static void CloneLoaderIfReq()
        {
            if (FullScreenLoadingUI.instance == null)
            {
                GameObject loaderUI = Instantiate(Resources.Load(prefabFullPath, typeof(GameObject))) as GameObject;
                FullScreenLoadingUI scr = loaderUI.GetComponent<FullScreenLoadingUI>();
                scr.InitScriptIfReq();
            }
        }

        static void UpdateLoaderCanvasSortingOrder()
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            int sortOrder = -1;
            foreach (var c in allCanvases)
            {
                if (c == instance.canvas) { continue; }
                if (c.sortingOrder > sortOrder)
                {
                    sortOrder = c.sortingOrder;
                }
            }
            instance.canvas.sortingOrder = sortOrder + 1;
        }
        
        public static void Show(string header, string subHeader = "", float progress = -1f)
        {
            //if null, spawn prefab from resource folder
            CloneLoaderIfReq();

            //iterate through all canvas and pick the highest ordered sorting order
            UpdateLoaderCanvasSortingOrder();

            instance.loaderFullScreen.SetActive(true);
            instance.headerTxt.text = header;
            if (string.IsNullOrEmpty(subHeader))
            {
                instance.subHeaderTxt.gameObject.SetActive(false);
            }
            else
            {
                instance.subHeaderTxt.gameObject.SetActive(true);
                instance.subHeaderTxt.text = subHeader;
            }

            if (progress >= 0f)
            {
                float percentage = (progress * 100f);
                string pValue = String.Format("{0:0.00}", percentage);
                instance.progressTxt.text = pValue + "%";
                instance.progressTxt.gameObject.SetActive(true);
            }
            else
            {
                instance.progressTxt.gameObject.SetActive(false);
            }
        }
        
        public static void HideIfAny()
        {
            //if null, spawn prefab from resource folder
            CloneLoaderIfReq();

            //iterate through all canvas and pick the highest ordered sorting order
            UpdateLoaderCanvasSortingOrder();

            instance.loaderFullScreen.SetActive(false);
        }
    }
}