using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;

namespace com.rvkm.unitygames.gameplayFramework
{
    public class GameTagManager : IController
    {
        static TagData data;

        public static bool HaveSameParent(GameTag tag1, GameTag tag2)
        {
            bool found1 = false, found2 = false;
            var parent1 = GetParentTag(tag1, ref found1);
            var parent2 = GetParentTag(tag2, ref found2);
            return found1 && found2 && parent1 == parent2;
        }

        public static GameTag GetParentTag(GameTag tag)
        {
            bool success = false;
            return GetParentTag(tag, ref success);
        }

        public static GameTag GetParentTag(GameTag tag, ref bool found)
        {
            TagNode parentResult = null;
            found = false;
            if (tag == GameTag.rootTag)
            {
                //nobody should request parent of root tag since there can be no parent of root itself
                //so if some code indeed does this, we should LOG BUG
                Debug.Log("LOG BUG");
                return GameTag.UnAssigned;
            }
            else
            {
                if (data == null || data.rootNodeRuntime == null)
                {
                    //by design we must have a valid tag asset loaded
                    //if we do not have any, LOG BUG
                    Debug.Log("LOG BUG");
                    return GameTag.UnAssigned;
                }
                else
                {
                    TravelThroughTheTreeForParent(tag, data.rootNodeRuntime, ref parentResult, ref found);
                    if (found == false)
                    {
                        //by design, we must have found parent tag of any tag other than root tag.
                        //so if we do not find one, we should log bug!
                        Debug.Log("LOG BUG");
                        return GameTag.UnAssigned;
                    }
                    else
                    {
                        return parentResult.tag;
                    }
                }
            }
        }

        static void TravelThroughTheTreeForParent(GameTag tag, TagNode tagData,
            ref TagNode matchedOutput, ref bool found)
        {
            if ( tagData != null && tagData.childTags != null && tagData.childTags.Length > 0)
            {
                foreach (var c in tagData.childTags)
                {
                    if (c == null) 
                    {
                        //by design, we shall never end up here. Log bug
                        //but if we do then handle bug by just simply skip this iteration
                        Debug.Log("LOG BUG");
                        continue; 
                    }

                    if (c.tag == tag)
                    {
                        matchedOutput = c.parentTag;
                        found = true;
                        return;
                    }
                    TravelThroughTheTreeForParent(tag, c, ref matchedOutput, ref found);
                }
            }
        }

        public void InitSystem()
        {
            data = Resources.Load(ConstantManager.tagAssetPathResourceFolder) as TagData;
            if (data == null)
            {
                //by design, user will generate the data in proper directory or we do it for user
                //upon build or editor play and we refresh assetdatabase so that it can know our data
                //If we find this data to be null we should LOG BUG
                Debug.Log("LOG BUG");
            }
            Debug.Log("yes tag man initialised");
        }
    }
}