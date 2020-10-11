using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.rvkm.unitygames.extensions.UI
{
    public class DialogueBoxActions
    {
        public Action OnOK, OnOkCancel_Ok, OnOkCancel_Cancel, OnYes, OnNo,
           OnProgress_Ok, OnProgress_cancel;
    }

    public class BoxUI : MonoBehaviour
    {
        [SerializeField] Text headerTxt, mainMessageTxt;
        [SerializeField] Button okBtn, okCancel_okBtn, okCancel_cancelBtn, yesBtn, noBtn, progress_okBtn, progress_cancelBtn;
        [SerializeField] Image progressImg;

        [SerializeField] Canvas canvas;
        public Canvas canvasUI { get { return canvas; } }
        DialogueBoxActions boxActions;
        BoxType bType;

        public void Install(BoxType boxType, string header, string message, DialogueBoxActions actions)
        {
            //setup
            bType = boxType;
            headerTxt.text = header;
            mainMessageTxt.text = message;
            boxActions = actions;
            BindToButtonThenBindDestroyCommand(okBtn, boxActions.OnOK);
            BindToButtonThenBindDestroyCommand(okCancel_okBtn, boxActions.OnOkCancel_Ok);
            BindToButtonThenBindDestroyCommand(okCancel_cancelBtn, boxActions.OnOkCancel_Cancel);
            BindToButtonThenBindDestroyCommand(yesBtn, boxActions.OnYes);
            BindToButtonThenBindDestroyCommand(noBtn, boxActions.OnNo);
            BindToButtonThenBindDestroyCommand(progress_okBtn, boxActions.OnProgress_Ok);
            BindToButton(progress_cancelBtn, boxActions.OnProgress_cancel);
            PaintUI_Accordingly();
        }

        public void SignalComplete()
        {
            if (bType != BoxType.Progress) { return; }
            mainMessageTxt.text = "Completed!";
            TurnOffAllDialogueUI();
            progress_okBtn.gameObject.SetActive(true);
        }

        public void DestroyUI()
        {
            DestroyImmediate(gameObject);
        }

        public void SetProgress(float progres)
        {
            progressImg.fillAmount = progres;
        }

        void PaintUI_Accordingly()
        {
            TurnOffAllDialogueUI();
            if (bType == BoxType.Ok)
            {
                okBtn.gameObject.SetActive(true);
            }
            else if (bType == BoxType.OkCancel)
            {
                okCancel_okBtn.gameObject.SetActive(true);
                okCancel_cancelBtn.gameObject.SetActive(true);
            }
            else if (bType == BoxType.Progress)
            {
                progress_okBtn.gameObject.SetActive(false);
                progress_cancelBtn.gameObject.SetActive(true);
                progressImg.gameObject.SetActive(true);
                progressImg.fillAmount = 0.1f;
            }
            else if (bType == BoxType.YesNo)
            {
                yesBtn.gameObject.SetActive(true);
                noBtn.gameObject.SetActive(true);
            }
        }

        void BindToButton(Button b, Action action)
        {
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => { action?.Invoke(); });
        }

        void BindToButtonThenBindDestroyCommand(Button b, Action action)
        {
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => { action?.Invoke(); DestroyUI(); });
        }

        void TurnOffAllDialogueUI()
        {
            progressImg.gameObject.SetActive(false);
            okBtn.gameObject.SetActive(false); 
            okCancel_okBtn.gameObject.SetActive(false);
            okCancel_cancelBtn.gameObject.SetActive(false); 
            yesBtn.gameObject.SetActive(false); 
            noBtn.gameObject.SetActive(false); 
            progress_okBtn.gameObject.SetActive(false); 
            progress_cancelBtn.gameObject.SetActive(false);
        }
    }
}