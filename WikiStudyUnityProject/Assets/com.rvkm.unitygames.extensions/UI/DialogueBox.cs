using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.rvkm.unitygames.extensions.UI
{
    public class DialogueBox : MonoBehaviour
    {
        [SerializeField] Button okBtn;
        [SerializeField] Text errorMsgText;
        public void Setup(string errorMessage, Action OnComplete)
        {
            errorMsgText.text = errorMessage;
            okBtn.onClick.RemoveAllListeners();
            okBtn.onClick.AddListener(() =>
            {
                OnComplete?.Invoke();
            });
        }
    }
}