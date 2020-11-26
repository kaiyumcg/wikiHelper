using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace com.rvkm.unitygames.gameplayFramework
{
    public class TagManagerEditor : EditorWindow
    {
        bool willInspectRuntimeTags, willSeeEnumGenSoup;
        float dataUpdateTimer;
        Vector2 windowScroll;
        float dataUpdateRate = 1.23f;
        EditorTagData editorTagData;
        TagData runTimedata;
        AllTagEnumDescription allTagEnums;
        FileContentSoup enumSoup, enumTreeSoup;
        string CompileTagButton = Environment.NewLine + "Compile Tags " + Environment.NewLine;

        [MenuItem("RVKM/TagManager")]
        static void Init()
        {
            TagManagerEditor window = (TagManagerEditor)EditorWindow.GetWindow(typeof(TagManagerEditor));
            window.titleContent.text = "Tag Manager(RVKM)";
            window.CreateDataIfReq();
            window.SyncData();
            if (window.editorTagData == null || window.runTimedata == null)
            {
                //at this point we must have a valid tag asset data but if we do not have one, LOG BUG
                Debug.Log("LOG BUG!");
            }

            window.Show();
        }

        void CreateDataIfReq()
        {
            editorTagData = AssetDatabase.LoadAssetAtPath<EditorTagData>(EditorConstantManager.editorTagAssetFPath);
            if (editorTagData == null)
            {
                editorTagData = ScriptableObject.CreateInstance<EditorTagData>();
                editorTagData.saveTime = DateTime.Now;
                editorTagData.rootNode = new EditorTagNode();
                editorTagData.rootNode.tagValue = EditorConstantManager.rootTagName;
                editorTagData.rootNode.childTags = null;

                AssetDatabase.CreateAsset(editorTagData, EditorConstantManager.editorTagAssetFPath);
                Debug.Log("editor tag asset created! at: " + EditorConstantManager.editorTagAssetFPath);
                AssetDatabase.SaveAssets();
            }

            if (editorTagData.rootNode == null)
            {
                editorTagData.rootNode = new EditorTagNode();
                editorTagData.rootNode.childTags = null;
            }
            editorTagData.rootNode.tagValue = EditorConstantManager.rootTagName;
            editorTagData = Instantiate(editorTagData);
            AssetDatabase.SaveAssets();

            runTimedata = AssetDatabase.LoadAssetAtPath<TagData>(ConstantManager.runtimeTagAsset_RelativePath);
            if (runTimedata == null)
            {
                runTimedata = ScriptableObject.CreateInstance<TagData>();
                runTimedata.saveTime = DateTime.Now;
                runTimedata.rootNodeRuntime = new TagNode();
                runTimedata.rootNodeRuntime.tag = GameTag.rootTag;
                runTimedata.rootNodeRuntime.parentTag = null;
                runTimedata.rootNodeRuntime.childTags = null;
                AssetDatabase.CreateAsset(runTimedata, ConstantManager.runtimeTagAsset_RelativePath);
                Debug.Log("runtime tag asset created! at: " + ConstantManager.runtimeTagAsset_RelativePath);
                AssetDatabase.SaveAssets();
            }

            if (runTimedata.rootNodeRuntime == null)
            {
                runTimedata.rootNodeRuntime = new TagNode();
                runTimedata.rootNodeRuntime.tag = GameTag.rootTag;
                runTimedata.rootNodeRuntime.parentTag = null;
                runTimedata.rootNodeRuntime.childTags = null;
            }
            runTimedata.rootNodeRuntime.tag = GameTag.rootTag;
            runTimedata = Instantiate(runTimedata);

            enumSoup = ScriptableObject.CreateInstance<FileContentSoup>();
            enumTreeSoup = ScriptableObject.CreateInstance<FileContentSoup>();

            string dPath = Application.dataPath;
            dPath = dPath.Replace("Assets", "");
            enumSoup.savePath = Path.Combine(dPath, EditorConstantManager.tagEnumDefinesScriptFPath);
            enumTreeSoup.savePath = Path.Combine(dPath, EditorConstantManager.tagTreeDataDefinesScriptFPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void SyncData()
        {
            //sync code
            /*
            if (editorTagData.saveTime > runTimedata.saveTime)
            {
                //editor is latest
                if (runTimedata != null)
                {
                    AssetDatabase.DeleteAsset(ConstantManager.runtimeTagAsset_RelativePath);
                    AssetDatabase.Refresh();
                }
                //Now get data from editor version and create the runtime version
            }
            else
            {
                //runtime is latest
                if (editorTagData != null)
                {
                    AssetDatabase.DeleteAsset(ConstantManager.editorTagAssetFPath);
                    AssetDatabase.Refresh();
                }
                //Now get data from runtime version and create the editor version
            }
            */
        }

        private void OnDestroy()
        {
            System.GC.Collect();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Data Update Draw Rate");
             EditorGUILayout.LabelField("!!Increase it if you encounter bad editor performance!!");
            dataUpdateRate = EditorGUILayout.Slider("Rate : ", dataUpdateRate, 0.3f, 3f);
            windowScroll = EditorGUILayout.BeginScrollView(windowScroll);

            dataUpdateTimer += Time.unscaledDeltaTime;
            if (dataUpdateTimer > 0.8f)
            {
                dataUpdateTimer = 0f;
                SyncData();
                GenerateEnumDataAndClassStrSoup();
            }

            var editorTagsED = Editor.CreateEditor(editorTagData);
            editorTagsED.DrawDefaultInspector();

            willInspectRuntimeTags = EditorGUILayout.BeginToggleGroup("Inspect runtime Tag? ", willInspectRuntimeTags);
            if (willInspectRuntimeTags)
            {
                var ed = Editor.CreateEditor(runTimedata);
                ed.DrawDefaultInspector();
            }
            EditorGUILayout.EndToggleGroup();
            willSeeEnumGenSoup = EditorGUILayout.BeginToggleGroup("See the generated enum class? ", willSeeEnumGenSoup);
            if (willSeeEnumGenSoup)
            {
                EditorGUILayout.TextArea(enumSoup.soup);
            }
            EditorGUILayout.EndToggleGroup();

            if (GUILayout.Button(CompileTagButton))
            {
                SyncData();
                GenerateEnumDataAndClassStrSoup();
                Debug.Log("Compiled!");
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndScrollView();
        }

        void GenerateEnumDataAndClassStrSoup()
        {
            if (editorTagData != null && editorTagData.rootNode != null)
            {
                allTagEnums = new AllTagEnumDescription();
                TravelThroughTagTree(editorTagData.rootNode, ref allTagEnums);
            }

            string startStr = 
                "//This file is generated by editor code!" + Environment.NewLine +
                "using System.Collections;" + Environment.NewLine +
                "using System.Collections.Generic;" + Environment.NewLine + Environment.NewLine +
                "using UnityEngine;" + Environment.NewLine + Environment.NewLine +
                "namespace com.rvkm.unitygames.gameplayFramework" + Environment.NewLine +
                "{" + Environment.NewLine +
                "    public enum GameTag" + Environment.NewLine +
                "    {" + Environment.NewLine+"          ";
            string endingStr =
                "    }" + Environment.NewLine +
                "}" + Environment.NewLine;

            enumSoup.soup = startStr;
            for (int i = 0; i < allTagEnums.AllTagDesc.Count;i++)
            {
                var a = allTagEnums.AllTagDesc[i];
                if (i == allTagEnums.AllTagDesc.Count - 1)
                {
                    enumSoup.soup = enumSoup.soup + (a.EnumName + " = " + a.EnumValue + Environment.NewLine);
                }
                else
                {
                    enumSoup.soup = enumSoup.soup + (a.EnumName + " = " + a.EnumValue + ", ");
                }
                if (i % 4 == 0) { enumSoup.soup = enumSoup.soup + Environment.NewLine; }
                
            }
            enumSoup.soup += endingStr;
        }

        void TravelThroughTagTree(EditorTagNode startNode, ref AllTagEnumDescription desc)
        {
            if (startNode != null && !string.IsNullOrEmpty(startNode.tagValue))
            {
                desc.AddEnumIfNotPresent(startNode.tagValue);

                if (startNode.childTags != null && startNode.childTags.Length > 0)
                {
                    foreach (var c in startNode.childTags)
                    {
                        if (c == null) { continue; }
                        TravelThroughTagTree(c, ref desc);
                    }
                }
            }
        }
    }
}