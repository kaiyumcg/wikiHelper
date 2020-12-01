using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class IMGUIUtility
    {
        /// <summary>
        /// Show array 'list' as well as browse option to select it from. If you set optional 'thisDataObject' parameter, the window will start from here
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="thisDataObject"></param>
        /// <param name="showListSize"></param>
        public static void ShowArrayWithBrowseOption<T>(SerializedProperty list, ScriptableObject thisDataObject = null, bool showListSize = true) where T : Object
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
                    var elementSerProp = list.GetArrayElementAtIndex(i);
                    ShowDataWithBrowseOption<T>(elementSerProp, thisDataObject);   
                }
            }
            EditorGUI.indentLevel -= 1;
        }

        /// <summary>
        /// Show data 'unityObjectData' as well as browse option to select it from. If you set optional 'thisDataObject' parameter, the window will start from here
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unityObjectData"></param>
        /// <param name="thisDataObject"></param>
        public static void ShowDataWithBrowseOption<T>(SerializedProperty unityObjectData, ScriptableObject thisDataObject = null) where T : Object
        {
            bool loadCalled = false;
            GUILayout.BeginHorizontal("box");
            EditorGUILayout.PropertyField(unityObjectData);
            if (GUILayout.Button("Browse"))
            {
                loadCalled = true;
                string fOpenDir = GetDirForFileOpenDialogue(unityObjectData, thisDataObject);
                var path = EditorUtility.OpenFilePanel("Open " + typeof(T).Name + " files", fOpenDir, "");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Object data = null;
                bool pathEmpty = false;
                if (string.IsNullOrEmpty(path) == false)
                {
                    path = UrlUtility.GetRelativePath(path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    data = AssetDatabase.LoadAssetAtPath<T>(path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                else
                {
                    pathEmpty = true;
                }
                
                if (data != null)
                {
                    unityObjectData.objectReferenceValue = data;
                }
                else
                {
                    if (pathEmpty == false)
                    {
                        EditorUtility.DisplayDialog("Error", "Load failure! You probably tried to load an invalid data. Old value will retain!", "Ok");
                    }
                }
            }

            if (typeof(T) == typeof(TextAsset) && unityObjectData.objectReferenceValue != null)
            {
                if (GUILayout.Button("Open "+unityObjectData.objectReferenceValue.name+" 'Text'"))
                {
                    loadCalled = true;
                    var oPath = AssetDatabase.GetAssetPath(unityObjectData.objectReferenceValue);
                    oPath = UrlUtility.GetAbsolutePath(oPath);
                    if (File.Exists(oPath) == false || string.IsNullOrEmpty(oPath))
                    {
                        EditorUtility.DisplayDialog("Error", "Could not get path or the file does not exist in the disk! " +
                            System.Environment.NewLine + "Are you trying to open something from memory? This is not supported yet", "Ok");
                    }
                    else
                    {
                        EditorUtility.OpenWithDefaultApp(oPath);
                    }
                }
            }

            if (!loadCalled)
            {
                GUILayout.EndHorizontal();
            }
        }

        static string GetDirForFileOpenDialogue(SerializedProperty unityObjectSer, ScriptableObject dataObject)
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
    }
}