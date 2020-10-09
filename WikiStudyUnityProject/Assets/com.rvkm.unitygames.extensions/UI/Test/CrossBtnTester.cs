using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.extensions.UI
{
    public class CrossBtnTester : MonoBehaviour
    {
        [SerializeField]
        TapHoldRepeaterCrossButton btn;
        // Start is called before the first frame update
        void Start()
        {
            btn.OnCross_RemoveAllListeners();
            btn.OnCross_AddListener(() => { Debug.Log("cross it!"); }, 0.1f);
        }
    }
}