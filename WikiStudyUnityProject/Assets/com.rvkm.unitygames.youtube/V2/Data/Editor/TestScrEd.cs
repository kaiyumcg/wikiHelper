using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(TestScr))]
public class TestScrEd : Editor
{

    TestScr comp;
    static bool showTileEditor = false;

    public void OnEnable()
    {
        comp = (TestScr)target;
        if (comp.tValue == null)
        {
            comp.tValue = "Init value";
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        /*
        //MAP DEFAULT INFORMATION
        comp.mapName = EditorGUILayout.TextField("Name", comp.mapName);

        //WIDTH - HEIGHT
        int width = EditorGUILayout.IntField("Map Sprite Width", comp.mapSprites.GetLength(0));
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