using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.extensions.UI
{
    public class ErrorUI_Con : MonoBehaviour
    {
        [SerializeField] GameObject dialogueBoxPrefab;
        [SerializeField] Transform spawnParent;
        static ErrorUI_Con instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                if (ErrorUI_Con.instance != this)
                {
                    DestroyImmediate(this);
                    return;
                }
            }
        }

        public static void ShowDiaglogue(string message, Action OnComplete)
        {
            var box = Instantiate(instance.dialogueBoxPrefab, instance.spawnParent) as GameObject;
            var dialogueBox = box.GetComponent<DialogueBox>();
            if (dialogueBox == null)
            {
                throw new System.ApplicationException("Not dialogue box referenced!");
            }
            dialogueBox.Setup(message, OnComplete);
            box.gameObject.SetActive(true);
        }
    }
}