using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using System.Security.AccessControl;
using System.Linq;
using com.rvkm.unitygames.YouTubeSearch.Extensions;
using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using com.rvkm.unitygames.YouTubeSearch.HtmlPrinter;
using Mochineko.SimpleReorderableList;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [CustomEditor(typeof(SearchDataYoutube))]
    public class SearchDataEditor : KaiyumScriptableObjectEditor
    {
        SearchDataYoutube data;
        //public List<string> debugList = new List<string>();
        public static Action OnCategorize;
        public ReorderableList catList;

        public override void OnEnableScriptableObject()
        {
            data = (SearchDataYoutube)target;
            if (data.blacklist == null) { data.blacklist = new TagSearchDescription(); }
            if (data.mustUseList == null) { data.mustUseList = new TagSearchDescription(); }
            if (data.htmlPrintOptions == null) { data.htmlPrintOptions = new CategoryHtmlPrintDesc(); }
            catList = new ReorderableList(
                serializedObject.FindProperty(nameof(data.categories))
            );
            InitScript();
        }

        void InitScript()
        {
            TagControl.InitControl();
            YouTubeControl.InitControl();
            CategoryControl.InitControl();
            CategoryHtmlFilePrintControl.InitControl();
            IMGUIStatics.CreateGUIContents();
            EditorUtility.ClearProgressBar();
            anyButtonClicked = false;
        }

        public override void OnDisableScriptableObject()
        {
            InitScript();
        }

        void PrintSeperator()
        {
            string st = "";
            for(int i = 0; i < Screen.width;i++)
            {
                st += "-";
            }
            GUILayout.Label(st);
        }

        public override void OnUpdateScriptableObject()
        {
            anyButtonClicked = false;
            if (BusyControl.GetBusyFlagAndContinuePrintingRelatedIMGUIIfAny(this)) { return; }

            serializedObject.Update();
            data.showGenerationSetting = GUILayout.Toggle(data.showGenerationSetting, "Show Generation Setting? ");
            
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.showGenerationSetting)), true);
            if (data.showGenerationSetting)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.dataGenerationTestMode)), true);
                if (data.dataGenerationTestMode)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.dataGenerationCountForTestMode)), true);
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.forceUpdateForGeneration)), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.SearchName)), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.APIKEY)), true);
                PrintAssetFiles.ShowArrayWithBrowseOption<TextAsset>(serializedObject.FindProperty(nameof(data.InputHtmlFiles)), data);
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.InputUrls)), true);
                PrintAssetFiles.ShowDataWithBrowseOption<YoutubeVideoData>(serializedObject.FindProperty(nameof(data.videoData)), data);
                PrintAssetFiles.ShowDataWithBrowseOption<YoutubeVideoTags>(serializedObject.FindProperty(nameof(data.tagData)), data);
                int mins = 0;
                if (data.videoData != null && data.videoData.allVideos != null)
                {
                    foreach (var v in data.videoData.allVideos)
                    {
                        if (v == null) { continue; }
                        mins += v.durationInMinutes;
                    }
                }
                float hours = (float)mins / 60f;

                EditorGUILayout.LabelField("Total hours: " + (int)hours);
                GUILayout.Space(10);
                PrintYouTubeDataGenerationSettings.Print(ref data, this, serializedObject);
                PrintSeperator();
                GUILayout.Space(30);
                EditorGUI.indentLevel -= 1;
            }

            GUILayout.Space(10);
            PrintCategorySettings.Print(ref data, this, serializedObject);
            GUILayout.Space(10);
            PrintTagSettings.Print(ref data, ref anyButtonClicked, this, serializedObject);
            EditorUtility.SetDirty(data);
            if (data.videoData != null)
            {
                EditorUtility.SetDirty(data.videoData);
            }
            if (data.tagData != null)
            {
                EditorUtility.SetDirty(data.tagData);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}