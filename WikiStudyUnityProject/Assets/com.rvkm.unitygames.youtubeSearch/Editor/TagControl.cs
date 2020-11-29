using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using System.Linq;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class TagControl
    {
        static List<string> procList = new List<string>();
        static List<TagDesc> tagDescList = new List<TagDesc>();
        public static bool TagFetchOperationHasCompleted { get { return completed; } }
        public static float TagFetchOperationProgress { get { return progress; } }
        static bool completed;
        static float progress;
        public static void InitControl()
        {
            completed = true;
            progress = 0f;
        }

        public static void UpdateTags(SerializedObject editorSerializedObject, SearchDataYoutube searchData, SearchDataEditor editor)
        {
            completed = false;
            progress = 0f;
            procList = new List<string>();
            tagDescList = new List<TagDesc>();
            List<string> blacklistedTags = new List<string>();
            blacklistedTags.GetAllTags(searchData.ignoreTags);
            List<string> allTags = new List<string>();
            allTags.GetAllTags(searchData.allVideos);
            if (allTags.Count > 0)
            {
                allTags.RemoveIfContains(blacklistedTags);
            }
            allTags = allTags.OrderBy(x => x).ToList();
            var cor = EditorCoroutineUtility.StartCoroutine(UpdateTag(editorSerializedObject, editor, allTags, (ser, tagDescList) =>
            {
                if (tagDescList.Count > 0)
                {
                    searchData.allTags = tagDescList.ToArray();
                }
                else
                {
                    searchData.allTags = null;
                }
                ChannelDataEditorUtility.SaveYoutubeDataToDisk(editorSerializedObject, searchData);
                EditorUtility.DisplayDialog("Success!", "Tags updated", "Ok");
                if (searchData.printTagsInHtml)
                {
                    string errorMsgIfAny = "";
                    HtmlFilePrintUtility.UpdateTagHtmlfileAndOpenIt(searchData.allTags, ref errorMsgIfAny, () =>
                    {
                        editor.StopAllEditorCoroutines();
                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog("Error!", "Tag Operation Error!", "Ok");
                        completed = true;
                    });
                }
            }), editor);
            editor.AllCoroutines.Add(cor);
        }

        static IEnumerator UpdateTag(SerializedObject ser, SearchDataEditor editor, List<string> allTags, Action<SerializedObject, List<TagDesc>> OnFinish)
        {
            if (allTags.Count > 0)
            {
                for (int i = 0; i < allTags.Count; i++)
                {
                    progress = (float)(i + 1) / (float)allTags.Count;
                    if (string.IsNullOrEmpty(allTags[i]) || procList.HasAny(allTags[i])
                        || allTags[i].IsItNeumeric_YT()) { continue; }
                    procList.Add(allTags[i]);
                    var cor = EditorCoroutineUtility.StartCoroutine(SingleTagProcCOR(allTags, allTags[i],
                     (thisDesc) =>
                     {
                         tagDescList.Add(thisDesc);
                     }), editor);
                    editor.AllCoroutines.Add(cor);
                    yield return cor;
                }
            }
            completed = true;
            OnFinish?.Invoke(ser, tagDescList);
            EditorUtility.ClearProgressBar();
        }

        static IEnumerator SingleTagProcCOR(List<string> allTags, string thisTag, Action<TagDesc> OnFinish)
        {
            SingleTagProc(allTags, thisTag, OnFinish);
            yield return null;
        }

        static void SingleTagProc(List<string> allTags, string thisTag, Action<TagDesc> OnFinish)
        {
            var tagDesc = new TagDesc();
            tagDesc.mainTag = thisTag;
            List<string> relList = new List<string>();
            allTags.ForEach((pred) =>
            {
                if (string.IsNullOrEmpty(pred) == false
                && string.Equals(pred, thisTag, StringComparison.OrdinalIgnoreCase) == false
                && procList.HasAny(pred) == false)
                {
                    if (pred.Contains_IgnoreCase(thisTag))
                    {
                        relList.Add(pred);
                        procList.Add(pred);
                    }
                }
            });

            if (relList.Count > 0)
            {
                tagDesc.relatedWords = relList.ToArray();
            }
            else
            {
                tagDesc.relatedWords = null;
            }

            var pList = new List<string>();
            pList.CopyUniqueFrom(procList);
            OnFinish?.Invoke(tagDesc);
        }
    }
}