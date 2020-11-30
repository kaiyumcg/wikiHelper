using com.rvkm.unitygames.YouTubeSearch.Extensions;
using com.rvkm.unitygames.YouTubeSearchInternal;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class ChannelDataEditorUtility
    {
        public static string GetStringIfNullOrEmpty(string str, ref bool success)
        {
            string result = "";
            if (string.IsNullOrEmpty(str) == false)
            {
                result = str;
                success = true;
            }
            return result;
        }

        public static int GetIntFromString(string str, ref bool success)
        {
            int result = 0;
            if (string.IsNullOrEmpty(str) == false)
            {
                int count = 0;
                var perseSuccess = int.TryParse(str, out count);
                if (perseSuccess)
                {
                    result = count;
                    success = true;
                }
            }
            return result;
        }

        public static void SaveObjectToDiskJson(UnityEngine.Object obj)
        {
            var data = EditorJsonUtility.ToJson(obj, true);
            string assetPath = AssetDatabase.GetAssetPath(obj);
            FileInfo nfo = new FileInfo(assetPath);

            string savePath = Path.Combine(nfo.Directory.FullName, obj.name + "_json.txt");
            File.WriteAllText(savePath, data);
        }

        public static void SaveYoutubeDataToDisk(SerializedObject editorSerializedObject, SearchDataYoutube searchData)
        {
            EditorUtility.SetDirty(searchData);
            EditorUtility.SetDirty(searchData.videoData);
            EditorUtility.SetDirty(searchData.tagData);
            editorSerializedObject.ApplyModifiedProperties();
            ChannelDataEditorUtility.SaveObjectToDiskJson(searchData.videoData);
            ChannelDataEditorUtility.SaveObjectToDiskJson(searchData.tagData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static string GetWWWResponse(string Url)
        {
            string result = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (String.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                result = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
            return result;
        }

        /// <summary>
        /// https://codesnippets.fesslersoft.de/get-the-youtube-videoid-from-url/
        /// </summary>
        const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
        public static string GetVideoId(string url)
        {
            var regex = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
            foreach (Match match in regex.Matches(url))
            {
                foreach (var groupdata in match.Groups.Cast<Group>().Where(groupdata => !groupdata.ToString().StartsWith("http://") && !groupdata.ToString().StartsWith("https://") && !groupdata.ToString().StartsWith("youtu") && !groupdata.ToString().StartsWith("www.")))
                {
                    return groupdata.ToString();
                }
            }
            return string.Empty;
        }
    }
}