using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.rvkm.unitygames.extensions.UI
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] GameObject loaderFullScreen;
        [SerializeField] Text progressTxt, headerTxt, subHeaderTxt;
        static LoadingUI instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                if (LoadingUI.instance != this)
                {
                    DestroyImmediate(this);
                    return;
                }
            }
        }

        public static void Show(string header, string subHeader, float progress)
        {
            //todo
        }

        public static void Hide()
        {
            //todo
        }
    }
}