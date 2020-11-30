using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using System.Security.AccessControl;
using System.Linq;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public abstract class KaiyumScriptableObjectEditor : Editor
    {
        static List<EditorCoroutine> allCORs;
        public List<EditorCoroutine> AllCoroutines { get { return allCORs; } }
        public void OnEnable()
        {
            allCORs = new List<EditorCoroutine>();
            StopAllEditorCoroutines();
            OnEnableScriptableObject();
        }

        public abstract void OnEnableScriptableObject();
        public abstract void OnUpdateScriptableObject();
        public abstract void OnDisableScriptableObject();

        public override void OnInspectorGUI()
        {
            OnUpdateScriptableObject();
        }

        public void StopAllEditorCoroutines()
        {
            if (allCORs != null && allCORs.Count > 0)
            {
                foreach (var c in allCORs)
                {
                    if (c == null) { continue; }
                    EditorCoroutineUtility.StopCoroutine(c);
                }
                allCORs.Clear();
                allCORs = new List<EditorCoroutine>();
            }
        }

        private void OnDisable()
        {
            OnDisableScriptableObject();
            StopAllEditorCoroutines();
        }
    }
}