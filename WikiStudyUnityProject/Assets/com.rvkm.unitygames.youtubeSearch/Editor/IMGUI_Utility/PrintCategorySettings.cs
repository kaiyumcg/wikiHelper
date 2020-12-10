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
        public static void Print(ref SearchDataYoutube mainData, SearchDataEditor editor, SerializedObject serializedObject)
        {
            SearchDataYoutube data = mainData;
            data.showCategorySetting = GUILayout.Toggle(data.showCategorySetting, "Category Settings");
            if (data.showCategorySetting)
            {
                EditorGUI.indentLevel += 2;
                PrintCategory.ShowCategoryArray(serializedObject.FindProperty(nameof(data.categories)), data.categories);
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
        }
    }
}