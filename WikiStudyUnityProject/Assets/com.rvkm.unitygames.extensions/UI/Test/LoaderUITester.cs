using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.rvkm.unitygames.extensions.UI;

public class LoaderUITester : MonoBehaviour
{
    [SerializeField] float testValue;
    [SerializeField] bool willShowSubHeader, willShowProgress;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (willShowSubHeader)
            {
                if (willShowProgress)
                {
                    FullScreenLoadingUI.Show("Time Management", "printing current delta time stuffs delta: " + Time.deltaTime, Time.deltaTime);
                }
                else
                {
                    FullScreenLoadingUI.Show("Time Management", "printing current delta time stuffs delta: " + Time.deltaTime);
                }
            }
            else
            {
                if (willShowProgress)
                {
                    FullScreenLoadingUI.Show("Time Management", "", Time.deltaTime);
                }
                else
                {
                    FullScreenLoadingUI.Show("Time Management");
                }
            }

            
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            FullScreenLoadingUI.HideIfAny();
        }
    }
}
