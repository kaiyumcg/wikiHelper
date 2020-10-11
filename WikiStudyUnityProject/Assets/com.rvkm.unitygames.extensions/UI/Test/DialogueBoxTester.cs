using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.rvkm.unitygames.extensions.UI;

public class DialogueBoxTester : MonoBehaviour
{
    [SerializeField] float testValue;
    [SerializeField] BoxType testType;
    //BoxUI data;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (testType == BoxType.Ok)
            {
                DialogueBox.ShowOk("Test UI", "Test message", () =>
                {
                    Debug.Log("Ok only UI tested at time: " + Time.time);
                });
            }
            else if (testType == BoxType.OkCancel)
            {
                DialogueBox.ShowOkCancel("Test UI OK CANCEL", "test message for ok cancel", () =>
                {
                    Debug.Log("ok button pressed on test ok cancel");
                }, () =>
                {

                    Debug.Log("cancel button pressed on test ok cancel");
                });
            }
            else if (testType == BoxType.YesNo)
            {
                DialogueBox.ShowYesNo("Test UI YESNO", "test message for YESNO", () =>
                {
                    Debug.Log("YES button pressed on test YESNO");
                }, () =>
                {

                    Debug.Log("NO button pressed on test YESNO");
                });
            }
            else if (testType == BoxType.Progress)
            {
                BoxUI data = null;
                data = DialogueBox.ShowProgress("Downloading", "DLC file is being downloaded....", () =>
                {
                    //ok button will appear because somebody will call 'data.SignalComplete();'. 
                    //We do not know who but we know that this code block will be executed.
                    Debug.Log("<color='green'>DLC download completed</color>");
                }, 
                () =>
                {
                    //we pressed cancel button and this code block will be executed
                    //ask for confirmation we really want to end this progress UI or not
                    DialogueBox.ShowYesNo("Confirmation!!", "Do you want to cancel the download?", () =>
                    {

                        //yes, we want to end it
                        Debug.Log("Yes destroy the progress window!");
                        if (data != null)
                        {
                            data.DestroyUI();
                        }
                        else
                        {
                            throw new System.Exception("data is null on progress stuffs");
                        }

                    }, () =>
                    {
                        //ok we do not want to end download process but we we want to force end?

                        DialogueBox.ShowYesNo("Confirmation!!", "will complete the test download process?", () =>
                        {

                            //yes end.
                            Debug.Log("Yes end the progress to 100%!");
                            if (data != null)
                            {
                                data.SignalComplete();
                            }
                            else
                            {
                                throw new System.Exception("data is null on progress stuffs");
                            }

                        }, () =>
                        {

                            //no do not end
                        });
                    });
                }, () =>
                {

                    data.SetProgress(Time.deltaTime);
                    data.SetMessage("current time is: " + Time.time);
                }, 0.1f);
            }

        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            
        }
    }
}
