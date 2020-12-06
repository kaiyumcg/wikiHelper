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

        public static void InitControl()
        {
            printHtmlCategoryCompleted = true;
            categoryHtmlPrintOperationProgress = 0f;
        }

        static IEnumerator WriteInnerHtmlForVideoCOR(YoutubeVideo video, SearchDataYoutube mainData)
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
            if (mainData.durationInHtml)
            {
                subHeading += "T: " + video.durationInMinutes + " minutes, ";
            }

            if (mainData.publishedYearInHtml)
            {
                subHeading += "Y: " + video.publisdedDate.year + ", ";
            }

            if (mainData.viewCountInHtml)
            {
                subHeading += "V/";
            }
            if (mainData.likeCountInHtml)
            {
                subHeading += "L/";
            }
            if (mainData.dislikeCountInHtml)
            {
                subHeading += "D/";
            }
            if (mainData.commentCountInHtml)
            {
                subHeading += "C/";
            }

            subHeading += ": ";
            if (mainData.viewCountInHtml)
            {
                subHeading += video.viewCount + "/";
            }
            if (mainData.likeCountInHtml)
            {
                subHeading += video.likeCount + "/";
            }
            if (mainData.dislikeCountInHtml)
            {
                subHeading += video.dislikeCount + "/";
            }
            if (mainData.commentCountInHtml)
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


            if (mainData.showThumbnailInHtml)
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

        public static void MakeCategoryWebPage(YoutubeCategory data, SearchDataYoutube mainData, SearchDataEditor editor, Action<string> OnHandleError)
        {
            printHtmlCategoryCompleted = false;
            categoryHtmlPrintOperationProgress = 0f;
            htmlCode = upperHtml;
            string nl = Environment.NewLine;
            htmlCode += @"<h1 style=""color: #5e9ca0;"">Category: " + data.categoryName + "</h1>";
            
            if (data.videoData != null && data.videoData.allVideos != null && data.videoData.allVideos.Length > 0)
            {
                videoCount = 1;
                var cor = EditorCoroutineUtility.StartCoroutine(WriteToHtmlCOR_Single(data.videoData.allVideos, mainData, editor, () =>
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
        
        static IEnumerator WriteToHtmlCOR_Single(YoutubeVideo[] videos, SearchDataYoutube mainData, SearchDataEditor editor, Action OnComplete)
        {
            videoCount = 1;
            foreach (var video in videos)
            {
                if (video == null) { continue; }
                categoryHtmlPrintOperationProgress = (float)((float)videoCount / (float)videos.Length);
                var cor = EditorCoroutineUtility.StartCoroutine(WriteInnerHtmlForVideoCOR(video, mainData), editor);
                editor.AllCoroutines.Add(cor);
                yield return cor;
                videoCount++;
            }
            printHtmlCategoryCompleted = true;
            categoryHtmlPrintOperationProgress = 1f;
            OnComplete?.Invoke();
        }

        static IEnumerator WriteToHtmlCOR_Multiple(YoutubeCategory[] dataList, SearchDataYoutube mainData, SearchDataEditor editor, Action OnComplete)
        {
            int maxVdCount = 0;
            if (mainData != null && mainData.videoData != null && mainData.videoData.allVideos != null && mainData.videoData.allVideos.Length > 0)
            {
                maxVdCount = mainData.videoData.allVideos.Length;
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
                        var cor = EditorCoroutineUtility.StartCoroutine(WriteInnerHtmlForVideoCOR(video, mainData), editor);
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
        
        public static void MakeCategoryWebPage(YoutubeCategory[] dataList, string searchName, SearchDataYoutube mainData, SearchDataEditor editor, Action<string> OnHandleError)
        {
            printHtmlCategoryCompleted = false;
            categoryHtmlPrintOperationProgress = 0f;
            htmlCode = upperHtml;
            string nl = Environment.NewLine;
            if (dataList != null && dataList.Length > 0)
            {
                videoCount = 1;
                var cor = EditorCoroutineUtility.StartCoroutine(WriteToHtmlCOR_Multiple(dataList, mainData, editor, () =>
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