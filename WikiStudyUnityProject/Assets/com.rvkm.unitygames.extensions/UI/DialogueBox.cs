using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.rvkm.unitygames.extensions.UI
{
    public enum BoxType { Ok, OkCancel, YesNo, Progress}
    public class DialogueBox : MonoBehaviour
    {
        static DialogueBox instance;
        const string dialogueBoxControllerPrefabFullPath = "rvkm/UI/dialogueCon";
        const string dialogueBoxPrefabFullPath = "rvkm/UI/dialogueBox";
        const float spawnRange = 0.03f;
        private void Awake()
        {
            InitScriptIfReq();
        }

        void InitScriptIfReq()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                if (DialogueBox.instance != this)
                {
                    DestroyImmediate(this);
                    return;
                }
            }
        }

        public static void ShowOk(string header, string message, Action OnOk = null)
        {
            var actions = new DialogueBoxActions
            {
                OnNo = null,
                OnOK = OnOk,
                OnOkCancel_Cancel = null,
                OnOkCancel_Ok = null,
                OnYes = null,
                OnProgress_cancel = null,
                OnProgress_Ok = null, OnProgress_Cycle = null, progressUI_Rate = 0f
            };
            DialogueBoxCore(BoxType.Ok, header, message, actions);
        }

        public static void ShowOkCancel(string header, string message, Action OnOk, Action OnCancel)
        {
            var actions = new DialogueBoxActions
            {
                OnNo = null,
                OnOK = null,
                OnOkCancel_Cancel = OnCancel,
                OnOkCancel_Ok = OnOk,
                OnYes = null,
                OnProgress_cancel = null,
                OnProgress_Ok = null, OnProgress_Cycle = null,
                progressUI_Rate = 0f
            };
            DialogueBoxCore(BoxType.OkCancel, header, message, actions);
        }

        public static void ShowYesNo(string header, string message, Action OnYes, Action OnNo = null)
        {
            var actions = new DialogueBoxActions
            {
                OnNo = OnNo,
                OnOK = null,
                OnOkCancel_Cancel = null,
                OnOkCancel_Ok = null,
                OnYes = OnYes,
                OnProgress_cancel = null,
                OnProgress_Ok = null, OnProgress_Cycle = null,
                progressUI_Rate = 0f
            };
            DialogueBoxCore(BoxType.YesNo, header, message, actions);
        }

        public static BoxUI ShowProgress(string header, string message, Action OnOk, Action OnCancel, Action OnProgressCycle = null, float updateRate = 0.4f)
        {
            var actions = new DialogueBoxActions
            {
                OnNo = null,
                OnOK = null,
                OnOkCancel_Cancel = null,
                OnOkCancel_Ok = null,
                OnYes = null,
                OnProgress_cancel = OnCancel,
                OnProgress_Ok = OnOk, OnProgress_Cycle = OnProgressCycle,
                progressUI_Rate = updateRate
            };
            return DialogueBoxCore(BoxType.Progress, header, message, actions);
        }

        static BoxUI DialogueBoxCore(BoxType boxType, string header, string message, DialogueBoxActions actions) 
        {
            GenEvSystemIfReq();
            CloneMainControlIfReq();
            GameObject box = Instantiate(Resources.Load(dialogueBoxPrefabFullPath, typeof(GameObject))) as GameObject;
            box.gameObject.SetActive(true);



            var dialogueBox = box.GetComponent<BoxUI>();
            UpdateBoxCanvasSortingOrder(dialogueBox.canvasUI);
            dialogueBox.Install(boxType, header, message, actions, spawnRange);

            return dialogueBox;
        }

        static void GenEvSystemIfReq()
        {
            EventSystem ev = FindObjectOfType<EventSystem>();
            if (ev == null)
            {
                //constrcut event system because it one of the dependency
                GameObject evObj = new GameObject("_Event_System_Generated");
                ev = evObj.AddComponent<EventSystem>();
            }

            StandaloneInputModule inpModule = FindObjectOfType<StandaloneInputModule>();
            if (inpModule == null)
            {
                //construct standard input module because it 'probably' is one of the dependency.
                //to be fully sure that we are all good, we will create it. 
                //We assume that creating one if not existed does not harm anything else
                inpModule = ev.gameObject.AddComponent<StandaloneInputModule>();
                inpModule.horizontalAxis = "Horizontal";
                inpModule.verticalAxis = "Vertical";
                inpModule.submitButton = "Submit";
                inpModule.cancelButton = "Cancel";
                inpModule.inputActionsPerSecond = 10f;
                inpModule.repeatDelay = 0.5f;
                inpModule.forceModuleActive = false;
            }
        }

        static void CloneMainControlIfReq()
        {
            if (DialogueBox.instance == null)
            {
                GameObject boxCon = Instantiate(Resources.Load(dialogueBoxControllerPrefabFullPath, typeof(GameObject))) as GameObject;
                DialogueBox scr = boxCon.GetComponent<DialogueBox>();
                scr.InitScriptIfReq();
            }
        }

        static void UpdateBoxCanvasSortingOrder(Canvas thisBoxCanvas)
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            int sortOrder = -1;
            foreach (var c in allCanvases)
            {
                if (c == thisBoxCanvas) { continue; }
                if (c.sortingOrder > sortOrder)
                {
                    sortOrder = c.sortingOrder;
                }
            }
            thisBoxCanvas.sortingOrder = sortOrder + 1;
        }
    }
}