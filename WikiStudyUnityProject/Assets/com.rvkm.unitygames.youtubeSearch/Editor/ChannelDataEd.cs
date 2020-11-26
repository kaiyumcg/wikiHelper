using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [CustomEditor(typeof(SearchDataYoutube))]
    public class ChannelDataEd : Editor
    {
        SearchDataYoutube data;
        public void OnEnable()
        {
            data = (SearchDataYoutube)target;
            //if (comp.tValue == null)
            //{
            //    comp.tValue = "Init value";
            //}
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Fetch Local"))
            {
                Debug.Log("yap!");
                List<YoutubeVideo> vList = new List<YoutubeVideo>();
                if (data.InputHtmlFiles != null)
                {
                    foreach (var txtAsset in data.InputHtmlFiles)
                    {
                        vList.CopyUniqueFrom(Utility.GetAllVideoInfo(txtAsset.text));
                    }
                }

                if (data.InputUrls != null)
                {
                    foreach (var url in data.InputUrls)
                    {
                        if (!Utility.IsUrl(url)) { continue; }
                        vList.CopyUniqueFrom(Utility.GetAllVideoInfo(Utility.GetWWWResponseSync(url)));
                    }
                }
                data.allVideos = vList.ToArray();
                data.fetchedFromLocal = true;
            }

            /*
            //MAP DEFAULT INFORMATION
            comp.mapName = EditorGUILayout.TextField("Name", comp.mapName);
            int width = EditorGUILayout.IntField("Map Sprite Width", comp.mapSprites.GetLength(0));

            //WIDTH - HEIGHT
            
            int height = EditorGUILayout.IntField("Map Sprite Height", comp.mapSprites.GetLength(1));

            if (width != comp.mapSprites.GetLength(0) || height != comp.mapSprites.GetLength(1))
            {
                comp.mapSprites = new Sprite[width, height];
            }

            showTileEditor = EditorGUILayout.Foldout(showTileEditor, "Tile Editor");

            if (showTileEditor)
            {
                for (int h = 0; h < height; h++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int w = 0; w < width; w++)
                    {
                        comp.mapSprites[w, h] = (Sprite)EditorGUILayout.ObjectField(comp.mapSprites[w, h], typeof(Sprite), false, GUILayout.Width(65f), GUILayout.Height(65f));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            */
        }
    }
}