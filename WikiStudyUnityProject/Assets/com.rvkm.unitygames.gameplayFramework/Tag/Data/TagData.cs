using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.gameplayFramework
{
    public class TagData : ScriptableObject
    {
        public TagNode rootNodeRuntime = null;
        public DateTime saveTime;
    }

    [System.Serializable]
    public class TagNode
    {
        public GameTag tag = GameTag.UnAssigned;
        public TagNode[] childTags = null;
        public TagNode parentTag = null;

        public TagNode()
        {
            tag = GameTag.UnAssigned;
            childTags = null;
            parentTag = null;
        }
    }
}