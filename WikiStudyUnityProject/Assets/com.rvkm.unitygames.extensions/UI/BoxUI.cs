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
           OnProgress_Ok, OnProgress_cancel, OnProgress_Cycle;
        public float progressUI_Rate;
    }

    public class BoxUI : MonoBehaviour
    {
        [SerializeField] Text headerTxt, mainMessageTxt;
        [SerializeField] Button okBtn, okCancel_okBtn, okCancel_cancelBtn, yesBtn, noBtn, progress_okBtn, progress_cancelBtn;
        [SerializeField] Image progressImg;

        [SerializeField] Canvas canvas;
        [SerializeField] Transform mainPanel;
        public Canvas canvasUI { get { return canvas; } }
        DialogueBoxActions boxActions;
        BoxType bType;
        bool installed = false;
        bool completed = false;

        public void Install(BoxType boxType, string header, string message, DialogueBoxActions actions, float spawnRange)
        {
            if (installed) { return; }
            installed = true;
            completed = false;
            timer = 0f;
            //todo range
            Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
            spawnRange = Screen.width * spawnRange;
            center.x += UnityEngine.Random.Range(-spawnRange, spawnRange);
            center.y += UnityEngine.Random.Range(-spawnRange, spawnRange);
            mainPanel.position = center;

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

        float timer;
        private void Update()
        {
            if (installed == false || bType != BoxType.Progress || completed) { return; }
            timer += Time.deltaTime;
            if (timer > boxActions.progressUI_Rate)
            {
                timer = 0f;
                boxActions.OnProgress_Cycle?.Invoke();
            }
        }

        public void SignalComplete()
        {
            if (bType != BoxType.Progress) { return; }
            mainMessageTxt.text = "Completed!";
            TurnOffAllDialogueUI();
            progress_okBtn.gameObject.SetActive(true);
            completed = true;
        }

        public void SetMessage(string msg)
        {
            mainMessageTxt.text = msg;
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