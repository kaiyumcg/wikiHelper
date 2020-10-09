using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace com.rvkm.unitygames.extensions.UI
{
    public class TapHoldRepeaterCrossButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public class RepeatedAction
        {
            public Action action;
            public float repeatRate, timer;
        }

        List<RepeatedAction> onCrossList;
        bool inside;
        [SerializeField] bool unscaledTime = false;
        public void OnCross_AddListener(Action OnCross, float repeatInterval = 0.2f)
        {
            if (onCrossList == null) { onCrossList = new List<RepeatedAction>(); }
            var ra = new RepeatedAction { action = OnCross, timer = 0f, repeatRate = repeatInterval };
            onCrossList.Add(ra);
        }

        void OnDestroy()
        {
            onCrossList = new List<RepeatedAction>();
        }

        public void OnCross_RemoveAllListeners()
        {
            onCrossList = new List<RepeatedAction>();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            inside = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            inside = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (inside == false) { return; }
            bool mouseStateOK = false;
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButton(0) || Input.GetMouseButton(1)) { mouseStateOK = true; }
            if (mouseStateOK == false) { return; }

            if (onCrossList == null || onCrossList.Count < 1) { return; }
            for (int i = 0; i < onCrossList.Count; i++)
            {
                if (unscaledTime)
                {
                    onCrossList[i].timer += Time.unscaledDeltaTime;
                }
                else
                {
                    onCrossList[i].timer += Time.deltaTime;
                }

                if (onCrossList[i].timer > onCrossList[i].repeatRate)
                {
                    onCrossList[i].timer = 0f;
                    onCrossList[i].action?.Invoke();
                }
            }
        }
    }
}