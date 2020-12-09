using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch.HtmlPrinter
{
    public static class CategoryHtmlFilePrintControl
    {
        /// <summary>
        /// https://stackoverflow.com/questions/31760560/collapsible-list-with-dynamically-loaded-content
        /// </summary>
        const string upperHtml = @"
<!DOCTYPE html>
<html>
<body>
";
        const string lowerHtml = @"

</body>
</html>
";

        public static bool printHtmlCategoryCompleted { get; private set; }
        public static float categoryHtmlPrintOperationProgress { get; private set; }
        static string htmlCode;
        static int videoCount = 1;
        static CategoryHtmlPrintDesc printDesc;

        public static void InitControl()
        {
            printHtmlCategoryCompleted = true;
            categoryHtmlPrintOperationProgress = 0f;
        }

        static IEnumerator WriteInnerHtmlForVideoCOR(YoutubeVideo video)
        {
            string title = "\"" + video.title + "\"";
            string url = "\"" + video.url + "\"";
            string thumbsUrl = "\"" + video.url + "\"";
            if (video.thumbUrls != null && video.thumbUrls.Length > 0)
            {
                string t_url = "";
                foreach (var u in video.thumbUrls)
                {
                    if (string.IsNullOrEmpty(u) == false)
                    {
                        t_url = u;
                    }
                }

                if (string.IsNullOrEmpty(t_url) == false)
                {
                    thumbsUrl = "\"" + t_url + "\"";
                }
            }

            string subHeading = "";
            if (printDesc.durationInHtml)
            {
                subHeading += "T: " + video.durationInMinutes + " minutes, ";
            }

            if (printDesc.publishedYearInHtml)
            {
                subHeading += "Y: " + video.publisdedDate.year + ", ";
            }

            if (printDesc.viewCountInHtml)
            {
                subHeading += "V/";
            }
            if (printDesc.likeCountInHtml)
            {
                subHeading += "L/";
            }
            if (printDesc.dislikeCountInHtml)
            {
                subHeading += "D/";
            }
            if (printDesc.commentCountInHtml)
            {
                subHeading += "C/";
            }

            subHeading += ": ";
            if (printDesc.viewCountInHtml)
            {
                subHeading += video.viewCount + "/";
            }
            if (printDesc.likeCountInHtml)
            {
                subHeading += video.likeCount + "/";
            }
            if (printDesc.dislikeCountInHtml)
            {
                subHeading += video.dislikeCount + "/";
            }
            if (printDesc.commentCountInHtml)
            {
                subHeading += video.commentCount + "/";
            }

            htmlCode += @"
<!-- #######  Video " + videoCount + @" #########-->
<p><a title=" + title + " href=" + url + ">" + title + @"</a></p>"

;

            if (string.IsNullOrEmpty(subHeading) == false)
            {
                htmlCode += @"
<p>[" + subHeading + @"] </p>";
            }


            if (printDesc.showThumbnailInHtml)
            {
                htmlCode +=
@"
<p><a href = " + url + @">
<img src = " + thumbsUrl + @" 
alt = " + title + @" width = ""480"" height = ""360""
/> 
</a>
</p>";
            }

            htmlCode += @"
<p>-----------------------------------------------------------------------------------------------------</p>
";
            yield return null;
        }

        public static void MakeCategoryWebPage(YoutubeCategory data, CategoryHtmlPrintDesc printDesc, KaiyumScriptableObjectEditor editor, Action<string> OnHandleError)
        {
            CategoryHtmlFilePrintControl.printDesc = printDesc;
            printHtmlCategoryCompleted = false;
            categoryHtmlPrintOperationProgress = 0f;
            htmlCode = upperHtml;
            string nl = Environment.NewLine;
            htmlCode += @"<h1 style=""color: #5e9ca0;"">Category: " + data.categoryName + "</h1>";
            
            if (data.videoData != null && data.videoData.allVideos != null && data.videoData.allVideos.Length > 0)
            {
                videoCount = 1;
                var cor = EditorCoroutineUtility.StartCoroutine(WriteToHtmlCOR_Single(data.videoData.allVideos, editor, () =>
                {
                    htmlCode += lowerHtml;

                    var htmlSavepath = EditorUtility.SaveFilePanel("Save html files", "", "web_" + data.categoryName + ".html", "html");
                    try
                    {
                        File.WriteAllText(htmlSavepath, htmlCode);
                    }
                    catch (Exception)
                    {
                        OnHandleError?.Invoke("can not write html code into html file!");
                        printHtmlCategoryCompleted = true;
                        categoryHtmlPrintOperationProgress = 1f;
                        return;
                    }
                    EditorUtility.OpenWithDefaultApp(htmlSavepath);
                }), editor);
                editor.AllCoroutines.Add(cor);
            }

            
        }
        
        static IEnumerator WriteToHtmlCOR_Single(YoutubeVideo[] videos, KaiyumScriptableObjectEditor editor, Action OnComplete)
        {
            videoCount = 1;
            foreach (var video in videos)
            {
                if (video == null) { continue; }
                categoryHtmlPrintOperationProgress = (float)((float)videoCount / (float)videos.Length);
                var cor = EditorCoroutineUtility.StartCoroutine(WriteInnerHtmlForVideoCOR(video), editor);
                editor.AllCoroutines.Add(cor);
                yield return cor;
                videoCount++;
            }
            printHtmlCategoryCompleted = true;
            categoryHtmlPrintOperationProgress = 1f;
            OnComplete?.Invoke();
        }

        static IEnumerator WriteToHtmlCOR_Multiple(YoutubeCategory[] dataList, KaiyumScriptableObjectEditor editor, Action OnComplete)
        {
            int maxVdCount = 0;
            if (dataList != null && dataList.Length > 0)
            {
                foreach (var d in dataList)
                {
                    if (d == null) { continue; }
                    if (d.videoData != null && d.videoData.allVideos != null && d.videoData.allVideos.Length > 0)
                    {
                        maxVdCount += d.videoData.allVideos.Length;
                    }
                }
            }

            foreach (var data in dataList)
            {
                if (data == null) { continue; }
                htmlCode += @"<h1 style=""color: #5e9ca0;"">Category: " + data.categoryName + "</h1>";
                //videoCount = 1;
                if (data.videoData != null && data.videoData.allVideos != null && data.videoData.allVideos.Length > 0)
                {
                    foreach (var video in data.videoData.allVideos)
                    {
                        if (video == null) { continue; }
                        categoryHtmlPrintOperationProgress = (float)((float)videoCount / (float)maxVdCount);
                        var cor = EditorCoroutineUtility.StartCoroutine(WriteInnerHtmlForVideoCOR(video), editor);
                        editor.AllCoroutines.Add(cor);
                        yield return cor;
                        videoCount++;
                    }
                }
            }
            printHtmlCategoryCompleted = true;
            categoryHtmlPrintOperationProgress = 1f;
            OnComplete?.Invoke();
        }
        
        public static void MakeCategoryWebPage(YoutubeCategory[] dataList, string searchName, CategoryHtmlPrintDesc printDesc, KaiyumScriptableObjectEditor editor, Action<string> OnHandleError)
        {
            CategoryHtmlFilePrintControl.printDesc = printDesc;
            printHtmlCategoryCompleted = false;
            categoryHtmlPrintOperationProgress = 0f;
            htmlCode = upperHtml;
            string nl = Environment.NewLine;
            if (dataList != null && dataList.Length > 0)
            {
                videoCount = 1;
                var cor = EditorCoroutineUtility.StartCoroutine(WriteToHtmlCOR_Multiple(dataList, editor, () =>
                {
                    htmlCode += lowerHtml;
                    var htmlSavepath = EditorUtility.SaveFilePanel("Save html files", "", "web_" + searchName + ".html", "html");
                    try
                    {
                        File.WriteAllText(htmlSavepath, htmlCode);
                    }
                    catch (Exception)
                    {
                        OnHandleError?.Invoke("can not write html code into html file!");
                        printHtmlCategoryCompleted = true;
                        categoryHtmlPrintOperationProgress = 1f;
                        return;
                    }
                    EditorUtility.OpenWithDefaultApp(htmlSavepath);
                }), editor);
                editor.AllCoroutines.Add(cor);
            }
        }
    }
}