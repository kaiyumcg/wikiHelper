using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace com.rvkm.unitygames
{
    public class CustomScriptCreator
    {
        //[MenuItem("Assets/Create/RVKM/Create General Behaviour")]
        //public static void CreateGeneralBehavour(MenuCommand cmd)
        //{
        //    if (Selection.activeObject == null) { return; }
        //    var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        //    if (File.Exists(path))
        //        path = Path.GetDirectoryName(path);
        //    if (string.IsNullOrEmpty(path)) path = "Assets/";

        //    string fPath = Path.Combine(Application.dataPath, @"com.rvkm.unitygames\rvkmScript.cst");
        //    File.Copy(fPath, Path.Combine(path, "NewScript.cs"));
        //    AssetDatabase.Refresh();
        //}
    }
}