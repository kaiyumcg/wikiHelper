using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace com.rvkm.unitygames.gameplayFramework
{
    [System.Serializable]
    public class TagEnumDesc
    {
        string enumName;
        public string EnumName { get { return enumName; } }
        int enumValue;
        public int EnumValue { get { return enumValue; } }

        public TagEnumDesc(string enumName, int enumValue)
        {
            this.enumName = enumName;
            this.enumValue = enumValue;
        }
    }

    [System.Serializable]
    public class AllTagEnumDescription
    {
        [SerializeField] List<TagEnumDesc> allTagDesc = new List<TagEnumDesc>();
        public List<TagEnumDesc> AllTagDesc { get { return allTagDesc; } }

        public AllTagEnumDescription()
        {
            allTagDesc = new List<TagEnumDesc>();
            var t1 = new TagEnumDesc(EditorConstantManager.rootTagName, 0);
            var t2 = new TagEnumDesc(EditorConstantManager.unassignedTagName, 1);
            allTagDesc.Add(t1);
            allTagDesc.Add(t2);

            /*
            foreach (var g in Enum.GetValues(typeof(GameTag)))
            {
                var t = new TagEnumDesc(g.ToString(), (int)g);
                allTagDesc.Add(t);
            }
            */
        }

        public void AddEnumIfNotPresent(string enumName)
        {
            if (string.IsNullOrEmpty(enumName)) { return; }
            enumName = RemoveSpecialChars(enumName);

            bool exists = allTagDesc.Exists((pred) => { return string.Equals(enumName, pred.EnumName); });
            if (exists) { return; }

            int enumValue = -1;
            foreach (var a in AllTagDesc)//first check which one is the greatest
            {
                if (a.EnumValue > enumValue)
                {
                    enumValue = a.EnumValue;
                }
            }
            enumValue = enumValue + 1;
            allTagDesc.Add(new TagEnumDesc(enumName, enumValue));
        }

        string RemoveSpecialChars(string input)
        {
            var res = Regex.Replace(input, @"[^0-9a-zA-Z\._]", string.Empty);
            return res.Replace(".", string.Empty);
        }


    }
}