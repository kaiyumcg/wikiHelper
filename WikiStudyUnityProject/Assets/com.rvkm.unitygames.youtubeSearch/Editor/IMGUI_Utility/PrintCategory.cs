using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility
{
    public static class PrintCategory
    {
        static float textAreaGlobalSize;
        public static void ShowCategoryArray(float textAreaSizeGlobal, SerializedProperty list, YoutubeCategory[] catData)
        {
            PrintCategory.textAreaGlobalSize = textAreaSizeGlobal;
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
                        if (catData[i] == null) { catData[i] = new YoutubeCategory(); }
                        ShowCategoryData(elementSerProp, catData[i]);
                    }
                }
            }
            EditorGUI.indentLevel -= 1;
        }

        static void ShowCategoryData(SerializedProperty catObj, YoutubeCategory catData)
        {
            GUILayout.BeginVertical("box");
            if (catData != null && catData.videoData != null && string.IsNullOrEmpty(catData.videoData.belongedCategory) == false)
            {
                catData.categoryName = catData.videoData.belongedCategory;
                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Category name: " + catData.categoryName);
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.PropertyField(catObj.FindPropertyRelative(nameof(catData.categoryName)), IMGUIStatics.categoryName, true);
            }
            EditorGUILayout.PropertyField(catObj.FindPropertyRelative(nameof(catData.showUI)), IMGUIStatics.willShowUI, true);
            if (catData.showUI)
            {
                if (catData.categoryName != "Uncategorized")
                {
                    PrintStringOp(catObj, catData.titleOp, IMGUIStatics.useTitle, nameof(catData.titleOp));
                    PrintStringOp(catObj, catData.descriptionOp, IMGUIStatics.useDescription, nameof(catData.descriptionOp));
                    PrintStringOp(catObj, catData.tagOp, IMGUIStatics.useTags, nameof(catData.tagOp));
                    PrintIntOp(catObj, catData.viewCountOp, IMGUIStatics.useViewCount, nameof(catData.viewCountOp), "View Count");
                    PrintIntOp(catObj, catData.likeCountOp, IMGUIStatics.useLikeCount, nameof(catData.likeCountOp), "Like Count");
                    PrintIntOp(catObj, catData.dislikeCountOp, IMGUIStatics.useDislikeCount, nameof(catData.dislikeCountOp), "Dislike Count");
                    PrintIntOp(catObj, catData.commentCountOp, IMGUIStatics.useCommentCount, nameof(catData.commentCountOp), "Comment Count");
                    PrintIntOp(catObj, catData.durationOp, IMGUIStatics.useDuration, nameof(catData.durationOp), "Duration in Minute");
                    PrintDateSearchOp(catObj, catData.pubDateOp, IMGUIStatics.useDateOperation, nameof(catData.pubDateOp), "Published Date");

                    EditorGUILayout.PropertyField(catObj.FindPropertyRelative(nameof(catData.sortMode)), IMGUIStatics.sortMode);
                    EditorGUILayout.PropertyField(catObj.FindPropertyRelative(nameof(catData.AscendingOrder)), IMGUIStatics.IsAscendingOrder);

                    if (GUILayout.Button(Environment.NewLine + "Sort" + Environment.NewLine))
                    {
                        Debug.Log("Here we must sort!");
                        if (catData != null && catData.videoData != null && catData.videoData.allVideos != null && catData.videoData.allVideos.Length > 0)
                        {
                            //use linq

                            throw new System.NotImplementedException(); //TODO
                        }
                    }
                }
                EditorGUILayout.PropertyField(catObj.FindPropertyRelative(nameof(catData.videoData)), IMGUIStatics.videoData);
                EditorGUILayout.PropertyField(catObj.FindPropertyRelative(nameof(catData.totalMinutes)), IMGUIStatics.totalMinutes);
                EditorGUILayout.PropertyField(catObj.FindPropertyRelative(nameof(catData.totalVideoCount)), IMGUIStatics.totalVideoCount);
            }
            
            GUILayout.EndVertical();
            GUILayout.Space(30);
        }

        static void PrintStringOp(SerializedProperty prop, StringSearchOp op, GUIContent useGuiContent, string varName)
        {
            EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".use"), useGuiContent, true);
            if (op.use)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".compMode"), IMGUIStatics.stringComparisonMode, true);
                EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".caseSensitive"), IMGUIStatics.isCaseSensitive, true);
                EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".useBlacklist"), IMGUIStatics.useBlacklist, true);
                if (op.useBlacklist)
                {
                    EditorGUI.indentLevel += 1;
                    op.scrollBlacklistUI = EditorGUILayout.BeginScrollView(op.scrollBlacklistUI);
                    op.blacklistedWords = EditorGUILayout.TextArea(op.blacklistedWords,
                        GUILayout.Height(Screen.height * textAreaGlobalSize));
                    EditorGUILayout.EndScrollView();
                    EditorGUI.indentLevel -= 1;
                }

                EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".useWhitelist"), IMGUIStatics.useWhitelist, true);
                if (op.useWhitelist)
                {
                    EditorGUI.indentLevel += 1;
                    op.scrollWhitelistUI = EditorGUILayout.BeginScrollView(op.scrollWhitelistUI);
                    op.whitelistedWords = EditorGUILayout.TextArea(op.whitelistedWords,
                        GUILayout.Height(Screen.height * textAreaGlobalSize));
                    EditorGUILayout.EndScrollView();
                    EditorGUI.indentLevel -= 1;
                }

                EditorGUI.indentLevel -= 1;
            }
        }

        static void PrintIntOp(SerializedProperty prop, IntSearchOp op, GUIContent useGuiContent, string varName, string goodName)
        {
            EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".use"), useGuiContent, true);
            if (op.use)
            {
                EditorGUI.indentLevel += 1;
                GUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".comparison"), new GUIContent(goodName + " is "), true);
                EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".target"), new GUIContent("To value"), true);
                if (op.target < 0) { op.target = 0; }
                GUILayout.EndVertical();

                EditorGUI.indentLevel -= 1;
            }
        }

        static void PrintDateSearchOp(SerializedProperty prop, DateSearchOp op, GUIContent useGuiContent, string varName, string goodName)
        {
            EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".use"), useGuiContent, true);
            if (op.use)
            {
                EditorGUI.indentLevel += 1;
                GUILayout.BeginVertical("box");
                if (op.mode == DateSearchMode.TheLast || op.mode == DateSearchMode.OlderThan)
                {
                    EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".mode"), new GUIContent(""), true);
                    EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".target"), new GUIContent(""), true);
                    EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".scale"), new GUIContent(""), true);
                }
                else
                {
                    EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".mode"), new GUIContent(""), true);
                    EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".targetMin"), new GUIContent("Start Date"), true);
                    EditorGUILayout.PropertyField(prop.FindPropertyRelative(varName + ".targetMax"), new GUIContent("End Date"), true);
                    ValidateDateTime(ref op.targetMin, op.targetMin);
                    ValidateDateTime(ref op.targetMax, op.targetMin);
                }
                GUILayout.EndVertical();
                EditorGUI.indentLevel -= 1;
            }
        }

        static void ValidateDateTime(ref OurDateTime dt, OurDateTime startDateTime)
        {
            if (dt.year <= 0 || dt.year < 1971) { dt.year = 1971; }
            if (dt.month <= 0) {  dt.month = 1; }
            if (dt.month > 12) {  dt.month = 12; }
            if (dt.day <= 0) { dt.day = 1; }

            if (dt.month == 2) 
            {
                if (DateTime.IsLeapYear(dt.year))
                {
                    if (dt.day > 29)
                    {
                        dt.day = 29;
                    }
                }
                else
                {
                    if (dt.day > 28)
                    {
                        dt.day = 28;
                    }
                }
            }

            if (dt.month == 1 || dt.month == 3 || dt.month == 5
                || dt.month == 7 || dt.month == 8 || dt.month == 10
                || dt.month == 12)
            {
                if (dt.day > 31)
                {
                    dt.day = 31;
                }
            }
            else if (dt.month != 2)
            {
                if (dt.day > 30)
                {
                    dt.day = 30;
                }
            }

            var startDt = new DateTime(startDateTime.year, startDateTime.month, startDateTime.day);
            var nowDt = new DateTime(dt.year, dt.month, dt.day);
            if (nowDt < startDt)
            {
                dt.year = startDt.Year;
                dt.month = startDt.Month;
                dt.day = startDt.Day;
            }
        }
    }
}