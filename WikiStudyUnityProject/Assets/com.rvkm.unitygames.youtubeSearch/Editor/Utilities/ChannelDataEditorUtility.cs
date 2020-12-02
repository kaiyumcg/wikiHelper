using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class ChannelDataEditorUtility
    {
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
    }
}