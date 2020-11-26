using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.gameplayFramework
{
    public class FileContentSoup : ScriptableObject
    {
        [Multiline]
        public string soup = "";
        [HideInInspector]
        public string savePath = "";
    }
}