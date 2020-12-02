using com.rvkm.unitygames.YouTubeSearch.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    [System.Serializable]
    public class TagDesc
    {
        public string mainTag;
        public string[] relatedWords;
    }

    [System.Serializable]
    public class TagSearchDescription
    {
        public bool use, useTextArea, useFiles;
        public string textAreaString;
        public TextAsset[] textFiles;
        public Vector2 scrollPositionUI;

        bool IsTextAssetsValid(TextAsset[] assets)
        {
            bool filesValid = false;
            if (assets != null && assets.Length > 0)
            {
                foreach (var t in assets)
                {
                    if (t != null && string.IsNullOrEmpty(t.text) == false)
                    {
                        filesValid = true;
                        break;
                    }
                }
            }
            return filesValid;
        }

        public bool IsValid()
        {
            bool textFilesValid = useFiles && IsTextAssetsValid(textFiles);
            bool textAreaValid = useTextArea && string.IsNullOrEmpty(textAreaString) == false;
            return use && (useFiles || useTextArea) && (textAreaValid || textFilesValid);
        }

        string GetAllTagString()
        {
            string strTags = "";
            if (useTextArea)
            {
                strTags = textAreaString;
            }

            if (useFiles)
            {
                if (textFiles != null && textFiles.Length > 0)
                {
                    foreach (var f in textFiles)
                    {
                        if (f == null) { continue; }
                        var str = f.text;
                        if (string.IsNullOrEmpty(str)) { continue; }
                        strTags += Environment.NewLine + str;
                    }
                }
            }
            return strTags;
        }

        public List<string> GetAllTagWords()
        {
            List<string> tagWords = new List<string>();
            if (IsValid())
            {
                string bListMainData = GetAllTagString();
                bListMainData = bListMainData.Replace("[S]", "");
                var outputs = Utility.SplitByComaOrNewline(bListMainData);
                if (outputs != null && outputs.Length > 0)
                {
                    foreach (var b in outputs)
                    {
                        if (string.IsNullOrEmpty(b)) { continue; }
                        if (tagWords.HasAny_IgnoreCase(b) == false)
                        {
                            tagWords.Add(b);
                        }
                    }
                }
            }
            return tagWords;
        }
    }
}