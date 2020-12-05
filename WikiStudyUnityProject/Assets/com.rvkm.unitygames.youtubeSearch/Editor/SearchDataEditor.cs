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
        static float progress;
        static bool busy;
        string nl;
        string ProcessButtonString, UpdateTagButtonString, CancelButtonString;
        //public List<string> debugList = new List<string>();
        bool loadWindowCalled = false;
        public override void OnEnableScriptableObject()
        {
            data = (SearchDataYoutube)target;
            if (data.blacklist == null) { data.blacklist = new TagSearchDescription(); }
            if (data.mustUseList == null) { data.mustUseList = new TagSearchDescription(); }
            nl = Environment.NewLine;
            ProcessButtonString = nl + "Generate youtube data" + nl;
            UpdateTagButtonString = nl + "Update tags" + nl;
            CancelButtonString = nl + "Cancel" + nl;
            TagControl.InitControl();
            YouTubeControl.InitControl();
            CategoryControl.InitControl();
            IMGUIStatics.CreateGUIContents();
            loadWindowCalled = false;
        }

        public override void OnDisableScriptableObject()
        {
            busy = false;
            EditorUtility.ClearProgressBar();
        }

        void UpdateTags()
        {
            busy = true;
            TagControl.UpdateTags(data, this, (tagList) => {

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
                    StopAllEditorCoroutines();
                    EditorUtility.DisplayDialog("Error!", "Tag Operation Error! meg: " + errorMsgIfAny, "Ok");
                });
            });
        }

        public override void OnUpdateScriptableObject()
        {
            //var edObject = new SerializedObject(this);
            //edObject.Update();
            //EditorGUILayout.PropertyField(edObject.FindProperty("debugList"), true);
            //EditorUtility.SetDirty(this);
            //edObject.ApplyModifiedProperties();

            loadWindowCalled = false;
            busy = false;
            if (TagControl.TagFetchOperationHasCompleted == false || YouTubeControl.YoutubeAPIOperationHasCompleted == false || CategoryControl.categoryOperationHasCompleted == false)
            {
                busy = true;
                if (TagControl.TagFetchOperationHasCompleted == false) { progress = TagControl.TagFetchOperationProgress; }
                else if (YouTubeControl.YoutubeAPIOperationHasCompleted == false) { progress = YouTubeControl.YoutubeAPIOperationProgress; }
                else if (CategoryControl.categoryOperationHasCompleted == false) { progress = CategoryControl.categoryOperationProgress; }
            }

            if (busy)
            {
                string status = "";
                if (TagControl.TagFetchOperationHasCompleted)
                {
                    status = "Updating tags. Please wait...";
                }
                else if (CategoryControl.categoryOperationHasCompleted)
                {
                    status = "Processing category information from youtube. Please wait...";
                }
                else
                {
                    status = "Processing video information from youtube. Please wait...";
                }

                GUILayout.Label(status);
                GUILayout.BeginHorizontal("box");
                progress = (float)Math.Round((double)progress, 4);
                
                GUILayout.Label("Progress: ");
                GUILayout.Button("" + (100 * progress) + "% ");
                if (loadWindowCalled == false)
                {
                    GUILayout.EndHorizontal();
                }
                

                GUILayout.Space(10);
                if (GUILayout.Button(CancelButtonString))
                {
                    StopAllEditorCoroutines();
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Error!", "Operation aborted by user!", "Ok");
                    TagControl.InitControl();
                    YouTubeControl.InitControl();
                    loadWindowCalled = true;
                    busy = false;
                }
                return;
            }

            serializedObject.Update();
            GUILayout.Label("Text Area global Size: ");
            data.textAreaSizeUI = GUILayout.HorizontalSlider(data.textAreaSizeUI, 0f, 1f);
            GUILayout.Space(30);

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
            
            data.showCategorySetting = GUILayout.Toggle(data.showCategorySetting, "Category Settings");
            if (data.showCategorySetting)
            {
                EditorGUI.indentLevel += 2;
                PrintCategory.ShowCategoryArray(data.textAreaSizeUI, serializedObject.FindProperty(nameof(data.categories)), data.categories, this);
                data.showAllCategoryOutputUI= EditorGUILayout.Foldout(data.showAllCategoryOutputUI, "Outputs Group");
                if (data.showAllCategoryOutputUI)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label("");
                    if (GUILayout.Button(Environment.NewLine + "Categorize" + Environment.NewLine))
                    {
                        //the video data should be created in similar name of this search
                        CategoryControl.Categorize(ref data, this);
                        StopAllEditorCoroutines();
                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog("Success!", "Successfully done category task!", "Ok");
                        busy = false;
                        loadWindowCalled = true;
                    }

                    if (GUILayout.Button(Environment.NewLine + "Open Html" + Environment.NewLine))
                    {
                        Debug.Log("Here we must open html!");
                        if (data != null && data.categories != null)
                        {
                            string errMsgIfAny = "";
                            CategoryHtmlFilePrint.MakeCategoryWebPage(data.categories, data.SearchName, ref errMsgIfAny, () =>
                            {
                                StopAllEditorCoroutines();
                                EditorUtility.DisplayDialog("Error!", "Category Operation Error! meg: " + errMsgIfAny, "Ok");
                            });
                        }
                        loadWindowCalled = true;
                    }
                    if (loadWindowCalled == false)
                    {
                        GUILayout.EndHorizontal();
                    }
                    
                    GUILayout.BeginVertical("box");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.publishedYearInHtml)), IMGUIStatics.printPublishedYearInHtml, true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.durationInHtml)), IMGUIStatics.printDurationInHtml, true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.showThumbnailInHtml)), IMGUIStatics.printThumbnailInHtml, true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.viewCountInHtml)), IMGUIStatics.printViewCountInHtml, true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.likeCountInHtml)), IMGUIStatics.printLikeCountInHtml, true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.dislikeCountInHtml)), IMGUIStatics.printDislikeCountInHtml, true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.commentCountInHtml)), IMGUIStatics.printCommentCountInHtml, true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.categoryProcessType)), IMGUIStatics.categoryListType, true);

                    if (loadWindowCalled == false)
                    {
                        GUILayout.EndVertical();
                    }
                }
                EditorGUI.indentLevel -= 2;
            }

            GUILayout.Space(30);
            data.showTagSetting = GUILayout.Toggle(data.showTagSetting, "Tag Settings");
            if (data.showTagSetting)
            {
                EditorGUI.indentLevel += 1;
                var serBlack = serializedObject.FindProperty(nameof(data.blacklist));
                
                EditorGUILayout.PropertyField(serBlack.FindPropertyRelative(nameof(data.blacklist.use)), IMGUIStatics.useBlacklist, true);//serializedObject.FindProperty("blacklist.use")
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

                if (GUILayout.Button(UpdateTagButtonString))
                {
                    if (data.videoData.IsDataOk() == false)
                    {
                        EditorUtility.DisplayDialog("Error!", "You are trying to update tags but youtube data is invalid! "
                            + Environment.NewLine + "Please generate the data first!", "Ok");
                    }
                    else
                    {
                        UpdateTags();
                    }
                    loadWindowCalled = true;
                }
                EditorGUI.indentLevel -= 1;
            }

            if (GUILayout.Button(ProcessButtonString))
            {
                loadWindowCalled = true;
                YoutubeVideoData vData = null;
                YoutubeVideoTags tData = null;
                var dataObtained = DependencyDataUtility.GetDependencyDataIfAny(data, ref vData, ref tData);
                if (!dataObtained)
                {
                    bool createNew = EditorUtility.DisplayDialog("----Choice----", "There is no dependency data for this '" + data.SearchName + "' search. "
                         + nl + " You can manually assign them or you can create one!", "Create them and start fresh", "Abort now to assign them");
                    if (createNew)
                    {
                        var creationSuccess = DependencyDataUtility.CreateFreshDependencyData(data, ref vData, ref tData);
                        if (!creationSuccess)
                        {
                            EditorUtility.DisplayDialog("Error", "Could not create dependency data. Check the log!", "Ok");
                            return;
                        }
                    }
                }
                if (vData == null || tData == null) { return; }
                data.videoData = vData;
                data.tagData = tData;
                data.videoData.searchName = data.SearchName;
                data.tagData.searchName = data.SearchName;
                var vSer = new SerializedObject(data.videoData);
                vSer.Update();
                var tSer = new SerializedObject(data.tagData);
                tSer.Update();

                busy = true;
                EditorUtility.DisplayProgressBar("Processing", "Reading html files and/or textasset containing html files", 0);
                List<YoutubeVideo> vList = new List<YoutubeVideo>();
                vList.CopyUniqueFrom(data.videoData.allVideos);
                if (data.InputHtmlFiles != null)
                {
                    foreach (var txtAsset in data.InputHtmlFiles)
                    {
                        vList.CopyUniqueFrom(HtmlNodeUtility.GetAllVideoInfo(txtAsset.text));
                    }
                }

                if (data.InputUrls != null)
                {
                    foreach (var url in data.InputUrls)
                    {
                        if (!UrlUtility.IsUrl(url)) { continue; }
                        vList.CopyUniqueFrom(HtmlNodeUtility.GetAllVideoInfo(Utility.GetWWWResponse(url)));
                    }
                }

                EditorUtility.ClearProgressBar();
                var cor = EditorCoroutineUtility.StartCoroutine(YouTubeControl.LoopAllYoutubeAPI(vList, data.APIKEY, this, data, (procList) =>
                {
                    data.videoData.allVideos = procList.ToArray();
                }), this);
                AllCoroutines.Add(cor);
                vSer.ApplyModifiedProperties();
                tSer.ApplyModifiedProperties();
                ChannelDataEditorUtility.SaveYoutubeDataToDisk(serializedObject, data);
            }

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