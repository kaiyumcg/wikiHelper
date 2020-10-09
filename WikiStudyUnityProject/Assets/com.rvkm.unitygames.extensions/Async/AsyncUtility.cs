using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.extensions.async
{
    public class AsyncUtility : MonoBehaviour
    {
        static AsyncUtility instance;
        private void Awake()
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

        public static void WaitOneFrame(Action OnComplete)
        {
            instance.StartCoroutine(instance.WaitFrameCOR(OnComplete));
        }

        public static void WaitXSeconds(float seconds, Action OnComplete)
        {
            instance.StartCoroutine(instance.WaitSecondsCOR(seconds, OnComplete));
        }

        IEnumerator WaitFrameCOR(Action OnComplete)
        {
            yield return null;
            OnComplete?.Invoke();
        }

        IEnumerator WaitSecondsCOR(float seconds, Action OnComplete)
        {
            yield return new WaitForSeconds(seconds);
            OnComplete?.Invoke();
        }
    }
}