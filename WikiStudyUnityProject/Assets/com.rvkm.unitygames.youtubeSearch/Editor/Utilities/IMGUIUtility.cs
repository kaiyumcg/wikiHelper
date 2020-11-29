using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class IMGUIUtility
    {
        public static void ShowArray(SerializedProperty list, bool showListSize = true)
        {
            EditorGUILayout.PropertyField(list, false);
            EditorGUI.indentLevel += 1;
            if (list.isExpanded)
            {
                if (showListSize)
                {
                    EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
                }
                for (int i = 0; i < list.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                }
            }
            EditorGUI.indentLevel -= 1;
        }
    }
}