using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.gameplayFramework
{
    public class EditorTagData : ScriptableObject
    {
        public EditorTagNode rootNode = null;
        public DateTime saveTime = default;
    }

    [System.Serializable]
    public class EditorTagNode
    {
        public string tagValue = null;
        public EditorTagNode[] childTags = null;

        public EditorTagNode()
        {
            tagValue = "";
            childTags = null;
        }
    }
}