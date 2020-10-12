using com.rvkm.unitygames.wiki;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.rvkm.unitygames.extensions.UI
{
    public enum OpenFromDirType { PersistentDataPath, DataPath, Resources, StreamingAsset, CustomPath, AutoSelect }
    public class BrowseIO : MonoBehaviour
    {
        static BrowseIO instance;
        const string dialogueBoxControllerPrefabFullPath = "rvkm/UI/browseIOCon";
        const string dialogueBoxPrefabFullPath = "rvkm/UI/browseIOBox";
        private void Awake()
        {
            InitScriptIfReq();
        }

        void InitScriptIfReq()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                if (BrowseIO.instance != this)
                {
                    DestroyImmediate(this);
                    return;
                }
            }
        }

        public static void OpenFileFrom(OpenFromDirType dirType, Action<byte[]> OnOpen, Action OnCancel = null,
            List<string> extensions = null, string customPathIfAny = "")
        {
            throw new System.NotImplementedException("IO file folder open UI not implemented!");
        }

        public static void OpenFileFrom(OpenFromDirType dirType, Action<FileInfo> OnOpen, Action OnCancel = null,
            List<string> extensions = null, string customPathIfAny = "")
        {
            throw new System.NotImplementedException("IO file folder open UI not implemented!");
        }

        public static void OpenFileFrom(OpenFromDirType dirType, Action<WikiDataJson> OnOpen, Action OnCancel = null, 
            string customPathIfAny = "")
        {
            throw new System.NotImplementedException("IO file folder open UI not implemented!");
        }

        public static void OpenDirectory(OpenFromDirType dirType, Action<DirectoryInfo> OnSetDirectory, Action OnCancel = null,
            string customPathIfAny = "")
        {
            throw new System.NotImplementedException("IO file folder open UI not implemented!");
        }

    }
}