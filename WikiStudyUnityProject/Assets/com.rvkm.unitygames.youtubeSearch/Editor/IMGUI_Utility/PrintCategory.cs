using com.rvkm.unitygames.YouTubeSearch.HtmlPrinter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility
{
    public static class PrintCategory
    {
        public static void ShowCategoryArray(SerializedProperty list, YoutubeCategory[] catData)
        {
            EditorGUILayout.PropertyField(list, false);
            EditorGUI.indentLevel += 1;
            if (list.isExpanded)
            {
                EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
                if (catData != null && catData.Length == list.arraySize) 
                {
                    for (int i = 0; i < list.arraySize; i++)
                    {
                        var elementSerProp = list.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(elementSerProp);
                    }
                }
            }
            EditorGUI.indentLevel -= 1;
        }

        
    }
}