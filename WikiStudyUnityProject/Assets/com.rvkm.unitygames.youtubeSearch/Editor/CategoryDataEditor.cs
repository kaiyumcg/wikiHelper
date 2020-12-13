using com.rvkm.unitygames.YouTubeSearch.HtmlPrinter;
using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [CustomEditor(typeof(YoutubeCategory))]
    public class CategoryDataEditor : KaiyumScriptableObjectEditor
    {
        YoutubeCategory data;
        bool showVideoData;
        Editor catVideoDataEd;
        public override void OnEnableScriptableObject()
        {
            data = (YoutubeCategory)target;
            if (data.htmlPrintOptions == null) { data.htmlPrintOptions = new CategoryHtmlPrintDesc(); }
            if (catVideoDataEd == null)
            {
                catVideoDataEd = Editor.CreateEditor(data.videoData);
            }
            SearchDataEditor.OnCategorize += OnCategory;
            InitScript();
        }

        void OnCategory()
        {
            if (catVideoDataEd == null)
            {
                catVideoDataEd = Editor.CreateEditor(data.videoData);
            }
        }

        void InitScript()
        {
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
            SearchDataEditor.OnCategorize -= OnCategory;
        }

        public override void OnUpdateScriptableObject()
        {
            anyButtonClicked = false;
            if (BusyControl.GetBusyFlagAndContinuePrintingRelatedIMGUIIfAny(this)) { return; }

            serializedObject.Update();
            ShowCategoryData(serializedObject);
            EditorUtility.SetDirty(data);
            if (data.videoData != null && data.videoData.allVideos != null && data.videoData.allVideos.Length > 0)
            {
                EditorUtility.SetDirty(data.videoData);
            }
            serializedObject.ApplyModifiedProperties();
        }

        void ShowCategoryData(SerializedObject catObj)
        {
            GUILayout.BeginVertical("box");
            data.showUI = EditorGUILayout.Foldout(data.showUI, "" + data.categoryName);
            if (data.showUI)
            {
                EditorGUI.indentLevel += 1;
                data.categoryName = data.name;
                EditorGUILayout.LabelField("Category name: " + data.categoryName);
                //EditorGUILayout.PropertyField(catObj.FindProperty(nameof(data.categoryName)), IMGUIStatics.categoryName, true);

                if (data.categoryName != "_uncat_generated")
                {
                    data.StrOpShow = EditorGUILayout.Foldout(data.StrOpShow, "Word Operations");
                    if (data.StrOpShow)
                    {
                        EditorGUI.indentLevel += 1;
                        PrintStringOp(catObj, data.titleOp, IMGUIStatics.useTitle, nameof(data.titleOp));
                        PrintStringOp(catObj, data.descriptionOp, IMGUIStatics.useDescription, nameof(data.descriptionOp));
                        PrintStringOp(catObj, data.tagOp, IMGUIStatics.useTags, nameof(data.tagOp));
                        EditorGUI.indentLevel -= 1;
                    }

                    data.IntOpShow = EditorGUILayout.Foldout(data.IntOpShow, "Number Operations");
                    if (data.IntOpShow)
                    {
                        EditorGUI.indentLevel += 1;
                        PrintIntOp(catObj, data.viewCountOp, IMGUIStatics.useViewCount, nameof(data.viewCountOp), "View Count");
                        PrintIntOp(catObj, data.likeCountOp, IMGUIStatics.useLikeCount, nameof(data.likeCountOp), "Like Count");
                        PrintIntOp(catObj, data.dislikeCountOp, IMGUIStatics.useDislikeCount, nameof(data.dislikeCountOp), "Dislike Count");
                        PrintIntOp(catObj, data.commentCountOp, IMGUIStatics.useCommentCount, nameof(data.commentCountOp), "Comment Count");
                        EditorGUI.indentLevel -= 1;
                    }

                    data.DurationAndDateOpShow = EditorGUILayout.Foldout(data.DurationAndDateOpShow, "Time And/Or Date Operations");
                    if (data.DurationAndDateOpShow)
                    {
                        EditorGUI.indentLevel += 1;
                        PrintIntOp(catObj, data.durationOp, IMGUIStatics.useDuration, nameof(data.durationOp), "Duration in Minute");
                        PrintDateSearchOp(catObj, data.pubDateOp, IMGUIStatics.useDateOperation, nameof(data.pubDateOp), "Published Date");
                        EditorGUI.indentLevel -= 1;
                    }


                }

                data.OutputOptionShow = EditorGUILayout.Foldout(data.OutputOptionShow, "Outputs group of this category");
                if (data.OutputOptionShow)
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUILayout.PropertyField(catObj.FindProperty(nameof(data.videoData)), IMGUIStatics.videoData);
                    
                    string duration = "";
                    if (data.totalMinutes > 60)
                    {
                        float hourCount = data.totalMinutes / 60.0f;
                        int hourCountInt = (int)hourCount;
                        int minutes = (int)data.totalMinutes - hourCountInt * 60;
                        duration = hourCountInt + " hours";
                        if (minutes > 0)
                        {
                            duration += " and " + minutes + " minutes";

                        }
                    }
                    else
                    {
                        duration = data.totalMinutes + " Minutes";
                    }
                    EditorGUILayout.LabelField("Duration: " + duration);
                    showVideoData = EditorGUILayout.Foldout(showVideoData, "Videos");
                    if (showVideoData)
                    {
                        EditorGUI.indentLevel += 1;
                        catVideoDataEd.OnInspectorGUI();
                        EditorGUI.indentLevel -= 1;
                    }
                    EditorGUILayout.PropertyField(catObj.FindProperty(nameof(data.totalVideoCount)), IMGUIStatics.totalVideoCount);
                    EditorGUILayout.PropertyField(catObj.FindProperty(nameof(data.averageVideoDuration)), IMGUIStatics.averageDuration);
                    EditorGUILayout.PropertyField(catObj.FindProperty(nameof(data.medianVideoDuration)), IMGUIStatics.mediationDuration);
                    EditorGUILayout.PropertyField(catObj.FindProperty(nameof(data.frequentVideoDuration)), IMGUIStatics.frequentDuration);

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.htmlPrintOptions)), IMGUIStatics.htmlPrintOptions, true);
                    EditorGUILayout.PropertyField(catObj.FindProperty(nameof(data.sortMode)), IMGUIStatics.sortMode);
                    GUILayout.BeginHorizontal("box");
                    EditorGUILayout.PropertyField(catObj.FindProperty(nameof(data.AscendingOrder)), IMGUIStatics.IsAscendingOrder);
                    if (GUILayout.Button(Environment.NewLine + "Sort" + Environment.NewLine))
                    {
                        Debug.Log("Here we must sort!");
                        if (data != null && data.videoData != null && data.videoData.allVideos != null && data.videoData.allVideos.Length > 0)
                        {
                            data.Sort();
                        }
                    }

                    if (GUILayout.Button(Environment.NewLine + "Open Html" + Environment.NewLine))
                    {
                        Debug.Log("Here we must open html!");
                        if (data != null && data.videoData != null && data.videoData.allVideos != null && data.videoData.allVideos.Length > 0)
                        {
                            CategoryHtmlFilePrintControl.MakeCategoryWebPage(data, data.htmlPrintOptions, this, (errMsgIfAny) =>
                            {
                                StopAllEditorCoroutines();
                                EditorUtility.DisplayDialog("Error!", "Html operation error! message: " + errMsgIfAny, "Ok");
                            });
                        }
                    }
                    GUILayout.EndHorizontal();

                    
                    

                    EditorGUI.indentLevel -= 1;
                }
                EditorGUI.indentLevel -= 1;
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        void PrintStringOp(SerializedObject prop, StringSearchOp op, GUIContent useGuiContent, string varName)
        {
            EditorGUILayout.PropertyField(prop.FindProperty(varName + ".use"), useGuiContent, true);
            if (op.use)
            {
                EditorGUI.indentLevel += 1;
                //EditorGUILayout.PropertyField(prop.FindProperty(varName + ".compMode"), IMGUIStatics.stringComparisonMode, true);
                //EditorGUILayout.PropertyField(prop.FindProperty(varName + ".caseSensitive"), IMGUIStatics.isCaseSensitive, true);
                PrintAssetFiles.ShowArrayWithBrowseOption<TextAsset>(prop.FindProperty(varName + ".defAssets"), data);
                EditorGUI.indentLevel -= 1;
            }
        }

        void PrintIntOp(SerializedObject prop, IntSearchOp op, GUIContent useGuiContent, string varName, string goodName)
        {
            EditorGUILayout.PropertyField(prop.FindProperty(varName + ".use"), useGuiContent, true);
            if (op.use)
            {
                EditorGUI.indentLevel += 1;
                GUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(prop.FindProperty(varName + ".mode"), new GUIContent("Operation Mode: "), true);
                if (op.mode == IntSearchCompMode.SingleTarget)
                {
                    EditorGUILayout.PropertyField(prop.FindProperty(varName + ".comparison"), new GUIContent(goodName + " is "), true);
                    EditorGUILayout.PropertyField(prop.FindProperty(varName + ".target"), new GUIContent("To value"), true);

                }
                else
                {
                    EditorGUILayout.PropertyField(prop.FindProperty(varName + ".betweenMode"), new GUIContent("Range Mode: "), true);
                    GUILayout.BeginHorizontal("box");
                    EditorGUILayout.PropertyField(prop.FindProperty(varName + ".targetMin"), new GUIContent("Range Min: "), true);
                    EditorGUILayout.PropertyField(prop.FindProperty(varName + ".targetMax"), new GUIContent("Range Max: "), true);
                    GUILayout.EndHorizontal();
                }

                if (op.target < 0) { op.target = 0; }
                if (op.targetMin < 0) { op.targetMin = 0; }
                if (op.targetMax < 0) { op.targetMax = 0; }
                GUILayout.EndVertical();

                EditorGUI.indentLevel -= 1;
            }
        }

        void PrintDateSearchOp(SerializedObject prop, DateSearchOp op, GUIContent useGuiContent, string varName, string goodName)
        {
            EditorGUILayout.PropertyField(prop.FindProperty(varName + ".use"), useGuiContent, true);
            if (op.use)
            {
                EditorGUI.indentLevel += 1;
                GUILayout.BeginVertical("box");
                if (op.mode == DateSearchMode.TheLast || op.mode == DateSearchMode.OlderThan)
                {
                    EditorGUILayout.PropertyField(prop.FindProperty(varName + ".mode"), new GUIContent(""), true);
                    EditorGUILayout.PropertyField(prop.FindProperty(varName + ".target"), new GUIContent(""), true);
                    EditorGUILayout.PropertyField(prop.FindProperty(varName + ".scale"), new GUIContent(""), true);
                }
                else
                {
                    EditorGUILayout.PropertyField(prop.FindProperty(varName + ".mode"), new GUIContent(""), true);
                    string startStr = "Start Date", endStr = "End Date";
                    if (op.mode == DateSearchMode.OutSideRange)
                    {
                        startStr = "Range Start Date";
                        endStr = "Range End Date";
                    }
                    EditorGUILayout.PropertyField(prop.FindProperty(varName + ".startDate"), new GUIContent(startStr), true);
                    EditorGUILayout.PropertyField(prop.FindProperty(varName + ".endDate"), new GUIContent(endStr), true);
                    ValidateDateTime(ref op.startDate, op.startDate);
                    ValidateDateTime(ref op.endDate, op.startDate);
                }
                GUILayout.EndVertical();
                EditorGUI.indentLevel -= 1;
            }
        }

        void ValidateDateTime(ref OurDateTime dt, OurDateTime startDateTime)
        {
            if (dt.year <= 0 || dt.year < 1971) { dt.year = 1971; }
            if (dt.month <= 0) { dt.month = 1; }
            if (dt.month > 12) { dt.month = 12; }
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