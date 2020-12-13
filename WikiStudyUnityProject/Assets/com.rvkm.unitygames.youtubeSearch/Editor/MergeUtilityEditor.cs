using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using Mochineko.SimpleReorderableList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [CustomEditor(typeof(MergeCategoryData))]
    public class MergeUtilityEditor : KaiyumScriptableObjectEditor
    {
        MergeCategoryData data;
        public ReorderableList catList;
        static string nl;
        public override void OnEnableScriptableObject()
        {
            data = (MergeCategoryData)target;
            catList = new ReorderableList(
                serializedObject.FindProperty(nameof(data.inputCategories))
            );
            InitScript();
        }

        void InitScript()
        {
            //init controls if any
            IMGUIStatics.CreateGUIContents();
            EditorUtility.ClearProgressBar();
        }

        public override void OnDisableScriptableObject()
        {
            InitScript();
        }

        void PrintSeperator()
        {
            string st = "";
            for (int i = 0; i < Screen.width; i++)
            {
                st += "-";
            }
            GUILayout.Label(st);
        }

        bool IsValidInputCategories(YoutubeCategory[] cats)
        {
            bool isValid = true;
            if (cats != null && cats.Length > 0)
            {
                foreach (var c in cats)
                {
                    if (c == null || c.videoData == null || c.videoData.allVideos == null || c.videoData.allVideos.Length == 0)
                    {
                        isValid = false;
                        break;
                    }
                }
            }
            else
            {
                isValid = false;
            }
            return isValid;
        }

        string GetDirForFileOpenDialogue(SerializedProperty unityObjectSer, ScriptableObject dataObject)
        {
            if (unityObjectSer.objectReferenceValue != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(unityObjectSer.objectReferenceValue);
                assetPath = UrlUtility.GetAbsolutePath(assetPath);
                FileInfo nfo = new FileInfo(assetPath);
                return UrlUtility.GetRelativePath(nfo.Directory.FullName);
            }
            else
            {
                string fOpenDir = Application.dataPath;
                if (dataObject != null)
                {
                    fOpenDir = UrlUtility.GetDataDirRelative(dataObject);
                    if (unityObjectSer.objectReferenceValue != null)
                    {
                        var validScriptableObject = unityObjectSer.objectReferenceValue.GetType() == typeof(ScriptableObject);
                        if (validScriptableObject == true)
                        {
                            fOpenDir = UrlUtility.GetDataDirRelative((ScriptableObject)unityObjectSer.objectReferenceValue);
                        }
                    }
                }
                fOpenDir = UrlUtility.GetRelativePath(fOpenDir);
                return fOpenDir;
            }
        }

        void CreateOrLoadVideoDataWithVideoList(ref YoutubeCategory cat, List<YoutubeVideo> catVlist, YoutubeCategory[] inputCategories)
        {
            var depDir = UrlUtility.GetDataDirRelative(inputCategories[0].videoData);
            string catName = cat.categoryName;
            catName = catName.Replace(" ", "_");
            string videoDataFilePathRel = Path.Combine(depDir, catName + "_vData_" + cat.GetHashCode() + ".asset");
            videoDataFilePathRel = videoDataFilePathRel.Replace('\\', '/');
            string assetPathAbs = UrlUtility.GetAbsolutePath(videoDataFilePathRel);
            if (File.Exists(assetPathAbs) == false)
            {
                //create
                cat.videoData = ScriptableObject.CreateInstance<YoutubeVideoData>();
                AssetDatabase.CreateAsset(cat.videoData, videoDataFilePathRel);
            }
            else
            {
                //load
                cat.videoData = AssetDatabase.LoadAssetAtPath<YoutubeVideoData>(videoDataFilePathRel);
            }
            cat.videoData.searchName = "Merged Data";
            cat.videoData.belongedCategory = cat.categoryName;
            cat.videoData.allVideos = catVlist.ToArray();
            cat.UpdateStat();
            cat.Sort();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public override void OnUpdateScriptableObject()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            {
                if (catList != null)
                {
                    catList.Layout();
                }
            }
            PrintSeperator();
            GUILayout.Space(30);

            if (data.outputCategory != null)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(data.outputCategory)), true);
                PrintSeperator();
                GUILayout.Space(30);
            }
            
            if (string.IsNullOrEmpty(nl)) { nl = Environment.NewLine; }
            if (GUILayout.Button(nl + "Merge" + nl))
            {
                if (IsValidInputCategories(data.inputCategories) == false)
                {
                    EditorUtility.DisplayDialog("Error!", "Input categories are invalid." +
                        " Check the categories and their video data. " +
                        "All categories must have valid video data for them to be merged!", "ok");
                    return;
                }

                var outputCatSer = serializedObject.FindProperty(nameof(data.outputCategory));
                string fOpenDir = GetDirForFileOpenDialogue(outputCatSer, data);
                var path = EditorUtility.SaveFilePanelInProject("Save Asset","merged category", "asset", "Save category output file to", fOpenDir);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("path: " + path);
                if (string.IsNullOrEmpty(path) == false)
                {
                    path = UrlUtility.GetRelativePath(path);
                    Debug.Log("rel path: " + path);
                    var absPath = UrlUtility.GetAbsolutePath(path);
                    var dataPath = Application.dataPath;
                    dataPath = dataPath.Replace("/", "\\");
                    Debug.Log("abs path: " + absPath + " and data path: " + dataPath);
                    if (absPath.StartsWith(dataPath))
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        UnityEngine.Object loadedData = AssetDatabase.LoadAssetAtPath<YoutubeCategory>(path);
                        if (loadedData != null)
                        {
                            outputCatSer.objectReferenceValue = loadedData;
                        }
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error!", "You must save it inside project folder!", "ok");
                        return;
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Error!", "The path is invalid! Please try again.", "ok");
                    return;
                }

                if (data.outputCategory == null)
                {
                    data.outputCategory = ScriptableObject.CreateInstance<YoutubeCategory>();
                    AssetDatabase.CreateAsset(data.outputCategory, path);
                    AssetDatabase.SaveAssets();
                    data.outputCategory.categoryName = data.outputCategory.name;
                    AssetDatabase.Refresh();
                }
                List<YoutubeVideo> vList = new List<YoutubeVideo>();
                foreach (var c in data.inputCategories)
                {
                    vList.AddRange(c.videoData.allVideos);
                }


                CreateOrLoadVideoDataWithVideoList(ref data.outputCategory, vList, data.inputCategories);
            }


            EditorUtility.SetDirty(data);
            if (data.outputCategory != null)
            {
                EditorUtility.SetDirty(data.outputCategory);
            }
            serializedObject.ApplyModifiedProperties();

        }
    }

}