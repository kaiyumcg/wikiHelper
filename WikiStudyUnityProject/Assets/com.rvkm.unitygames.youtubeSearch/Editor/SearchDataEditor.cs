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

namespace com.rvkm.unitygames.YouTubeSearch
{
    /// <summary>
    /// TODO: horizontal box indent, fold/expand item group, implement unimplemented methods, indent buttons
    /// </summary>
    [CustomEditor(typeof(SearchDataYoutube))]
    public class SearchDataEditor : KaiyumScriptableObjectEditor
    {
        SearchDataYoutube data;
        //public List<string> debugList = new List<string>();
        public static Action OnCategorize;

        public override void OnEnableScriptableObject()
        {
            data = (SearchDataYoutube)target;
            if (data.blacklist == null) { data.blacklist = new TagSearchDescription(); }
            if (data.mustUseList == null) { data.mustUseList = new TagSearchDescription(); }
            if (data.htmlPrintOptions == null) { data.htmlPrintOptions = new CategoryHtmlPrintDesc(); }
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

        public override void OnUpdateScriptableObject()
        {
            anyButtonClicked = false;
            if (BusyControl.GetBusyFlagAndContinuePrintingRelatedIMGUIIfAny(this)) { return; }

            serializedObject.Update();

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
            PrintCategorySettings.Print(ref data, this, serializedObject);
            GUILayout.Space(30);
            PrintTagSettings.Print(ref data, ref anyButtonClicked, this, serializedObject);
            PrintYouTubeDataGenerationSettings.Print(ref data, this, serializedObject);

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