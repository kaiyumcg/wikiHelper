using com.rvkm.unitygames.YouTubeSearch.HtmlPrinter;
using com.rvkm.unitygames.YouTubeSearch.IMGUI_Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class BusyControl
    {
        static float progress;
        static string nl;
        public static bool GetBusyFlagAndContinuePrintingRelatedIMGUIIfAny(KaiyumScriptableObjectEditor editor)
        {
            bool busy = false;
            string status = "";
            if (string.IsNullOrEmpty(nl)) { nl = Environment.NewLine; }
            if (TagControl.TagFetchOperationHasCompleted == false || YouTubeControl.YoutubeAPIOperationHasCompleted == false
                || CategoryControl.categoryOperationHasCompleted == false || CategoryHtmlFilePrintControl.printHtmlCategoryCompleted == false)
            {
                busy = true;
                if (TagControl.TagFetchOperationHasCompleted == false)
                {
                    status = "Updating tags. Please wait...";
                    progress = TagControl.TagFetchOperationProgress;
                }
                else if (YouTubeControl.YoutubeAPIOperationHasCompleted == false)
                {
                    status = "Processing video information from youtube. Please wait...";
                    progress = YouTubeControl.YoutubeAPIOperationProgress;
                }
                else if (CategoryControl.categoryOperationHasCompleted == false)
                {
                    status = "Processing category information from youtube. Please wait...";
                    progress = CategoryControl.categoryOperationProgress;
                }
                else if (CategoryHtmlFilePrintControl.printHtmlCategoryCompleted == false)
                {
                    status = "Processing Html page for view. Please wait...";
                    progress = CategoryHtmlFilePrintControl.categoryHtmlPrintOperationProgress;
                }

                GUILayout.Label(status);
                GUILayout.BeginHorizontal("box");
                progress = (float)Math.Round((double)progress, 4);

                GUILayout.Label("Progress: ");
                GUILayout.Button("" + (100 * progress) + "% ");
                if (editor.anyButtonClicked == false)
                {
                    GUILayout.EndHorizontal();
                }


                GUILayout.Space(10);
                if (GUILayout.Button(nl + "Cancel" + nl))
                {
                    editor.StopAllEditorCoroutines();
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Error!", "Operation aborted by user!", "Ok");
                    TagControl.InitControl();
                    YouTubeControl.InitControl();
                    editor.anyButtonClicked = true;
                    busy = false;
                }
            }

            return busy;
        }
    }
}