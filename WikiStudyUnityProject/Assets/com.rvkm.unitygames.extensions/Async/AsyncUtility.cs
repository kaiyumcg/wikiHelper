using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: MD. Al Kaiyum Rumman. GitHub: https://github.com/rummanKaiyum
/// Async library to read/write files and do tasks asynchronously
/// </summary>

namespace com.rvkm.unitygames.extensions.async
{
    public class AsyncUtility : MonoBehaviour
    {
        static AsyncUtility instance;
        public delegate void OnWriteComplete(bool success);
        Dictionary<string, byte[]> dataPool = new Dictionary<string, byte[]>();
        Dictionary<string, Thread> threadPool = new Dictionary<string, Thread>();
        Dictionary<string, bool> completedFlagPool = new Dictionary<string, bool>();
        Dictionary<string, OnWriteComplete> callbackPool = new Dictionary<string, OnWriteComplete>();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                if (AsyncUtility.instance != this)
                {
                    DestroyImmediate(this);
                    return;
                }
            }
        }

        static void InitIfReq()
        {
            if (AsyncUtility.instance == null)
            {
                var gObj = new GameObject("_AsyncUtilityCon");
                var inst = gObj.GetComponent<AsyncUtility>();
                if (inst == null)
                {
                    inst = gObj.AddComponent<AsyncUtility>();
                    AsyncUtility.instance = inst;
                }
            }
        }

        public static void WaitOneFrame(Action OnComplete)
        {
            InitIfReq();
            instance.StartCoroutine(instance.WaitFrameCOR(OnComplete));
        }

        public static void WaitXSeconds(float seconds, Action OnComplete, bool unscaledTime = false)
        {
            InitIfReq();
            instance.StartCoroutine(instance.WaitSecondsCOR(seconds, OnComplete, unscaledTime));
        }

        IEnumerator WaitFrameCOR(Action OnComplete)
        {
            yield return null;
            OnComplete?.Invoke();
        }

        IEnumerator WaitSecondsCOR(float seconds, Action OnComplete, bool unscaledTime)
        {
            if (unscaledTime)
            {
                yield return new WaitForSecondsRealtime(seconds);
            }
            else
            {
                yield return new WaitForSeconds(seconds);
            }
            
            OnComplete?.Invoke();
        }

        public static void WriteAllBytesAsync(byte[] data, string savePath, OnWriteComplete callback = null)
        {
            InitIfReq();
            Debug.LogWarning("save path API start: " + savePath);
            if (instance.dataPool.ContainsKey(savePath))
            {
                instance.dataPool[savePath] = data;
            }
            else
            {
                instance.dataPool.Add(savePath, data);
            }

            instance.SaveDataThreaded(savePath);

            if (instance.completedFlagPool.ContainsKey(savePath))
            {
                instance.completedFlagPool[savePath] = false;
            }
            else
            {
                instance.completedFlagPool.Add(savePath, false);
            }

            if (instance.callbackPool.ContainsKey(savePath))
            {
                instance.callbackPool[savePath] = callback;
            }
            else
            {
                instance.callbackPool.Add(savePath, callback);
            }
            instance.StartCoroutine(instance.NowCheckForResourceUnload(savePath));
        }

        public static void ReadAllBytesAsync(string readPath, Action<bool, byte[]> OnRead)
        {
            InitIfReq();
            //need extra care on iOS for plist management and file url format. On Android, similar stuffs must be done.
            throw new System.NotImplementedException("Async read is yet to be implemented. " +
                "Visit: 'https://github.com/rummanKaiyum' or mail me at 'kaiyumce06rumman@gmail.com'");
        }

        IEnumerator NowCheckForResourceUnload(string savePath)
        {
            yield return null;
            while (true)
            {
                bool flag = false;
                if (completedFlagPool.ContainsKey(savePath))
                {
                    flag = completedFlagPool[savePath];
                }
                else
                {
                    throw new Exception("could not find the flag in the pool!");
                }
                if (flag)
                {
                    break;
                }
                yield return null;
            }
            Resources.UnloadUnusedAssets();
            if (completedFlagPool.ContainsKey(savePath))
            {
                completedFlagPool.Remove(savePath);
            }

            if (completedFlagPool.ContainsKey(savePath))
            {
                completedFlagPool.Remove(savePath);
            }
        }

        void SaveDataThreaded(string savePath)
        {
            Thread thread = new Thread(() => SaveDataTaskThreaded(savePath));
            if (threadPool.ContainsKey(savePath))
            {
                threadPool[savePath] = thread;
            }
            else
            {
                threadPool.Add(savePath, thread);
            }
            thread.Start();
        }

        void SaveDataTaskThreaded(string savePath)
        {
            byte[] data = null;
            if (dataPool.ContainsKey(savePath))
            {
                data = dataPool[savePath];
            }
            else
            {
                CallCB(savePath, false);
                throw new Exception("could not find the data to write on dataPool!");
            }
            File.WriteAllBytes(savePath, data);
            Debug.LogWarning("data saved!- used threading for path: " + savePath);
            data = null;

            CallCB(savePath, true);

            if (dataPool.ContainsKey(savePath))
            {
                dataPool.Remove(savePath);
            }
            if (threadPool.ContainsKey(savePath))
            {
                threadPool.Remove(savePath);
            }


            if (callbackPool.ContainsKey(savePath))
            {
                callbackPool.Remove(savePath);
            }
        }

        void CallCB(string savePath, bool flag)
        {
            OnWriteComplete cb = null;
            if (callbackPool.ContainsKey(savePath))
            {
                cb = callbackPool[savePath];
            }

            if (cb != null)
            {
                cb(flag);
            }
        }
    }
}