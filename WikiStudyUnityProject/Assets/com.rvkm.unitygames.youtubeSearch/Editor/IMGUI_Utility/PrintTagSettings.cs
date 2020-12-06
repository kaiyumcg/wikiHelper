using com.rvkm.unitygames.YouTubeSearch.HtmlPrinter;
using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class PrintTagSettings
    {
        static string nl;

        public static void Print(ref SearchDataYoutube mainData, ref bool loadWindowCalled, SearchDataEditor editor, SerializedObject serializedObject)
        {
            if (string.IsNullOrEmpty(nl)) { nl = Environment.NewLine; }
            SearchDataYoutube data = mainData;
            data.showTagSetting = GUILayout.Toggle(data.showTagSetting, "Tag Settings");
            if (data.showTagSetting)
            {
                EditorGUI.indentLevel += 1;
                var serBlack = serializedObject.FindProperty(nameof(data.blacklist));

                EditorGUILayout.PropertyField(serBlack.FindPropertyRelative(nameof(data.blacklist.use)), IMGUIStatics.useBlacklist, true);
                if (data.blacklist.use)
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUILayout.PropertyField(serBlack.FindPropertyRelative(nameof(data.blacklist.useFiles)), true);
                    if (data.blacklist.useFiles)
                    {
                        EditorGUI.indentLevel += 1;
                        PrintAssetFiles.ShowArrayWithBrowseOption<TextAsset>(serBlack.FindPropertyRelative(nameof(data.blacklist.textFiles)), data);
                        EditorGUI.indentLevel -= 1;
                    }

                    EditorGUILayout.PropertyField(serBlack.FindPropertyRelative(nameof(data.blacklist.useTextArea)), true);
                    if (data.blacklist.useTextArea)
                    {
                        EditorGUI.indentLevel += 1;
                        data.blacklist.scrollPositionUI = EditorGUILayout.BeginScrollView(data.blacklist.scrollPositionUI);
                        data.blacklist.textAreaString = EditorGUILayout.TextArea(data.blacklist.textAreaString, GUILayout.Height(Screen.height - Screen.height * 0.67f));
                        EditorGUI.indentLevel -= 1;
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUI.indentLevel -= 1;
                }
                GUILayout.Space(10);

                var serWhite = serializedObject.FindProperty(nameof(data.mustUseList));
                EditorGUILayout.PropertyField(serWhite.FindPropertyRelative(nameof(data.mustUseList.use)), IMGUIStatics.useWhitelist, true);
                if (data.mustUseList.use)
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUILayout.PropertyField(serWhite.FindPropertyRelative(nameof(data.mustUseList.useFiles)), true);
                    if (data.mustUseList.useFiles)
                    {
                        EditorGUI.indentLevel += 1;
                        PrintAssetFiles.ShowArrayWithBrowseOption<TextAsset>(serWhite.FindPropertyRelative(nameof(data.mustUseList.textFiles)), data);
                        EditorGUI.indentLevel -= 1;
                    }

                    EditorGUILayout.PropertyField(serWhite.FindPropertyRelative(nameof(data.mustUseList.useTextArea)), true);
                    if (data.mustUseList.useTextArea)
                    {
                        EditorGUI.indentLevel += 1;
                        data.mustUseList.scrollPositionUI = EditorGUILayout.BeginScrollView(data.mustUseList.scrollPositionUI);
                        data.mustUseList.textAreaString = EditorGUILayout.TextArea(data.mustUseList.textAreaString, GUILayout.Height(Screen.height - Screen.height * 0.67f));
                        EditorGUI.indentLevel -= 1;
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUI.indentLevel -= 1;
                }
                GUILayout.Space(20);

                if (GUILayout.Button(nl + "Update tags" + nl))
                {
                    if (data.videoData.IsDataOk() == false)
                    {
                        EditorUtility.DisplayDialog("Error!", "You are trying to update tags but youtube data is invalid! "
                            + Environment.NewLine + "Please generate the data first!", "Ok");
                    }
                    else
                    {
                        TagControl.UpdateTags(data, editor, (tagList) => 
                        
                        {
                            if (tagList.Count > 0)
                            {
                                data.tagData.allTags = tagList.ToArray();
                            }
                            else
                            {
                                data.tagData.allTags = null;
                            }
                            ChannelDataEditorUtility.SaveYoutubeDataToDisk(serializedObject, data);
                            string errorMsgIfAny = "";
                            TagsHtmlFilePrint.MakeTagWebPage(data.tagData, ref errorMsgIfAny, () =>
                            {
                                editor.StopAllEditorCoroutines();
                                EditorUtility.DisplayDialog("Error!", "Tag Operation Error! meg: " + errorMsgIfAny, "Ok");
                            });
                        });
                    }
                    loadWindowCalled = true;
                }
                EditorGUI.indentLevel -= 1;
            }
        }
    }
}