using com.rvkm.unitygames.YouTubeSearch.HtmlPrinter;
using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class PrintCategorySettings
    {
        static void PrintSeperator()
        {
            string st = "";
            for (int i = 0; i < Screen.width; i++)
            {
                st += "-";
            }
            GUILayout.Label(st);
        }

        public static void Print(ref SearchDataYoutube mainData, SearchDataEditor editor, SerializedObject serializedObject)
        {
            SearchDataYoutube data = mainData;
            data.showCategorySetting = GUILayout.Toggle(data.showCategorySetting, "Category Settings");
            if (data.showCategorySetting)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.useManualVideoDataForCategory)),
                    new GUIContent("Manual Video Source? "));
                if (data.useManualVideoDataForCategory)
                {
                    if (data.categoryDataForManualMode == null || data.categoryDataForManualMode.Length < 1)
                    {
                        data.categoryDataForManualMode = new YoutubeCategory[1];
                    }
                    EditorGUI.indentLevel += 1;
                    PrintAssetFiles.ShowArrayWithBrowseOption<YoutubeCategory>(serializedObject.FindProperty(nameof(data.categoryDataForManualMode)), data);
                    EditorGUI.indentLevel -= 1;
                }
                EditorGUI.indentLevel -= 1;

                EditorGUI.indentLevel += 2;
                
                    //GUILayout.Toggle(data.useManualVideoDataForCategory, "Use Video Data manually for category operation?");
                if (data.useManualVideoDataForCategory)
                {
                    
                }

                EditorGUI.BeginChangeCheck();
                {
                    if (editor.catList != null)
                    {
                        editor.catList.Layout();
                    }
                }
               
                //PrintCategory.ShowCategoryArray(serializedObject.FindProperty(nameof(data.categories)), data.categories);
                data.showAllCategoryOutputUI = EditorGUILayout.Foldout(data.showAllCategoryOutputUI, "Outputs Group");
                if (data.showAllCategoryOutputUI)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label("");
                    if (GUILayout.Button(Environment.NewLine + "Categorize" + Environment.NewLine))
                    {
                        //the video data should be created in similar name of this search
                        CategoryControl.Categorize(ref data, editor);
                        editor.StopAllEditorCoroutines();
                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog("Success!", "Successfully done category task!", "Ok");
                        SearchDataEditor.OnCategorize?.Invoke();
                        editor.anyButtonClicked = true;
                    }

                    if (GUILayout.Button(Environment.NewLine + "Open Html" + Environment.NewLine))
                    {
                        Debug.Log("Here we must open html!");
                        if (data != null && data.categories != null)
                        {
                            string errMsgIfAny = "";
                            CategoryHtmlFilePrintControl.MakeCategoryWebPage(data.categories, data.SearchName, data.htmlPrintOptions, editor, (errMsg) =>
                            {
                                editor.StopAllEditorCoroutines();
                                EditorUtility.DisplayDialog("Error!", "Category Operation Error! meg: " + errMsgIfAny, "Ok");
                            });
                        }
                        editor.anyButtonClicked = true;
                    }
                    if (editor.anyButtonClicked == false)
                    {
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.BeginVertical("box");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.htmlPrintOptions)), IMGUIStatics.htmlPrintOptions, true);

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.categoryProcessType)), IMGUIStatics.categoryListType, true);

                    if (editor.anyButtonClicked == false)
                    {
                        GUILayout.EndVertical();
                    }
                }
                EditorGUI.indentLevel -= 2;
            }
            PrintSeperator();
        }
    }
}