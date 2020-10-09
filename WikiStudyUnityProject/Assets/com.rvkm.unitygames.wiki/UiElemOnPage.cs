using com.rvkm.unitygames.extensions.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.rvkm.unitygames.wiki
{
    public class UiElemOnPage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        string full_url;
        public string Url { get { return full_url; } }//future
        [SerializeField] Button openWeb, minusBtn, plusBtn;
        [SerializeField] TapHoldRepeaterCrossButton minusBtnAdvanced;
        [SerializeField] Text urlText;
        [SerializeField] Color selectedColorTint;
        [SerializeField] MaskableGraphic[] allUI_Sel_Tween;
        Color[] allUI_initCol;
        Action OnMinus, OnPlus;

        private void Start()
        {
            SetupElem("https://en.wikipedia.org/wiki/.NET_Core", "dot net core", () => { Debug.Log("added!"); }, () => { Debug.Log("removed!"); });
        }

        public void SetupElem(string url, string url_name, Action OnPlus, Action OnMinus)
        {
            this.full_url = Utility.GetFullWikiUrlIfReq(url);
            this.urlText.text = url_name;
            this.OnMinus = OnMinus;
            this.OnPlus = OnPlus;
            if (IsSelTweenDataValid())
            {
                allUI_initCol = new Color[allUI_Sel_Tween.Length];
                for (int i = 0; i < allUI_initCol.Length; i++)
                {
                    allUI_initCol[i] = allUI_Sel_Tween[i].color;
                }
            }

            if (plusBtn != null && this.OnPlus != null)
            {
                plusBtn.onClick.RemoveAllListeners();
                plusBtn.onClick.AddListener(() => { this.OnPlus?.Invoke(); });
            }

            if (minusBtn != null && this.OnMinus != null)
            {
                minusBtn.onClick.RemoveAllListeners();
                minusBtn.onClick.AddListener(() => { this.OnMinus?.Invoke(); });
            }

            if (minusBtnAdvanced != null && this.OnMinus != null)
            {
                minusBtnAdvanced.OnCross_RemoveAllListeners();
                minusBtnAdvanced.OnCross_AddListener(() => { this.OnMinus?.Invoke(); }, 0.1f);
            }

            if (openWeb != null)
            {
                openWeb.onClick.AddListener(() =>
                {
                    Application.OpenURL(this.full_url);
                });
            }
        }

        bool IsSelTweenDataValid()
        {
            bool valid = true;
            if (allUI_Sel_Tween != null && allUI_Sel_Tween.Length > 0)
            {
                for (int i = 0; i < allUI_Sel_Tween.Length; i++)
                {
                    if (allUI_Sel_Tween[i] == null) { valid = false; break; }
                }
            }
            else
            {
                valid = false;
            }

            return valid;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (IsSelTweenDataValid())
            {
                for (int i = 0; i < allUI_Sel_Tween.Length; i++)
                {
                    allUI_Sel_Tween[i].color = selectedColorTint;
                }
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            StartCoroutine(ButtonExitCOR());
        }

        IEnumerator ButtonExitCOR()
        {
            yield return new WaitForSeconds(0.1f);
            if (IsSelTweenDataValid())
            {
                for (int i = 0; i < allUI_Sel_Tween.Length; i++)
                {
                    allUI_Sel_Tween[i].color = allUI_initCol[i];
                    yield return new WaitForSeconds(0.06f);
                }
            }
        }
    }
}