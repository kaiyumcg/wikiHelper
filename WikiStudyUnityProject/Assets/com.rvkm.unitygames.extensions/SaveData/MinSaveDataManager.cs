using com.rvkm.unitygames.debug;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.rvkm.unitygames.extensions.saveData
{
    public static class MinSaveDataManager
    {
        public static void WriteDataToDevice<T>(T data, string fileExtensionWithDot, ref bool error, ref string errorMsg)
        {
            if (data == null)
            {
                DebugRVKM.LogRed("invalid data. Write to device operation failed!");
                error = true;
                errorMsg += "invalid data. Write to device operation failed!";
                return;
            }
            //data.ticksAtSaveTime = DateTime.Now.Ticks;
            var json = JsonUtility.ToJson(data);

            string fPath = "";

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
            fPath = Path.Combine(Application.persistentDataPath, "" + DateTime.Now.Ticks + "_dt" + fileExtensionWithDot);
#elif UNITY_STANDALONE
        fPath = Path.Combine(Application.dataPath, "" + DateTime.Now.Ticks + "_dt"+fileExtensionWithDot);
#endif

            File.WriteAllText(fPath, json);
        }

        public static List<T> GetDataFromDeviceFiles<T>(string fileExtensionWithDot, ref bool error, ref string errorMsg)
        {
            List<T> result = new List<T>();
            string dir = "";
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
            dir = Application.persistentDataPath;
#elif UNITY_STANDALONE
        dir = Application.dataPath;
#endif
            DebugRVKM.LogBlack("app dir: " + dir);
            DirectoryInfo nfo = new DirectoryInfo(dir);
            if (nfo.Exists == false) { return result; }
            FileInfo[] jsonFiles = nfo.GetFiles("*" + fileExtensionWithDot, SearchOption.AllDirectories);
            if (jsonFiles == null || jsonFiles.Length < 1) { return result; }

            foreach (var j in jsonFiles)
            {
                string jsonStr = "";
                try
                {
                    jsonStr = File.ReadAllText(j.FullName);
                }
                catch (System.Exception ex)
                {
                    DebugRVKM.LogRed("error while reading " + fileExtensionWithDot + " files----will return empty data" +
                        "------file path: " + j.FullName + "-----error: " + ex.Message);
                    result = new List<T>();

                    error = true;
                    errorMsg += Environment.NewLine + "error while reading " + fileExtensionWithDot + " files----will return empty data" +
                        "------file path: " + j.FullName + "-----error: " + ex.Message;
                    return result;
                    throw;
                }

                T dataFromJson = default(T);
                try
                {
                    dataFromJson = JsonUtility.FromJson<T>(jsonStr);
                }
                catch (Exception ex)
                {
                    DebugRVKM.LogRed("error while reading .wrm json content----will return empty data" +
                        "-----file path: " + j.FullName + "-----error: " + ex.Message);
                    result = new List<T>();

                    error = true;
                    errorMsg += Environment.NewLine + "error while reading .wrm json content----will return empty data" +
                        "-----file path: " + j.FullName + "-----error: " + ex.Message;

                    return result;
                    throw;
                }
                DebugRVKM.LogGreen("json obtained from path: " + j.FullName);
                result.Add(dataFromJson);
            }
            return result;
        }
    }
}