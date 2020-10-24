using com.rvkm.unitygames.extensions.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.debug
{
    public class DebugRVKM
    {
        public static void CheckNull<T>(T t, string additionalMsg = "")
        {
            //restrict to reference type only 
            //param dia array neya jay ki na so that we don't have to do check(d)-->check(d.something)-->check(d.something.something)
            //lina komano
            //any way to hold execution to a line in runtime. like cin() c++
            if (t == null)
            {
                DialogueBox.LogError("Object: " + t.ToString() + " is null! additional message: " + additionalMsg);
            }
        }

        public static void CheckNullEmpty<T>(UnityEngine.Object obj, string additionalMsg = "")
        {
            if (obj == null)
            {
                DialogueBox.LogError("Object: " + obj.name + " is null! additional message: " + additionalMsg);
            }
        }

        public static void LogRed(string msg, bool enabled = true)
        {
            if (enabled)
            {
                Debug.Log("<color='red'>" + msg + "</color>");
            }
        }

        public static void LogGreen(string msg, bool enabled = true)
        {
            if (enabled)
            {
                Debug.Log("<color='green'>" + msg + "</color>");
            }
        }

        public static void LogBlue(string msg, bool enabled = true)
        {
            if (enabled)
            {
                Debug.Log("<color='blue'>" + msg + "</color>");
            }
        }

        public static void LogBlack(string msg, bool enabled = true)
        {
            if (enabled)
            {
                Debug.Log("<color='black'>" + msg + "</color>");
            }
        }

        public static void LogWhite(string msg, bool enabled = true)
        {
            if (enabled)
            {
                Debug.Log("<color='white'>" + msg + "</color>");
            }
        }

        public static void LogMagenta(string msg, bool enabled = true)
        {
            if (enabled)
            {
                Debug.Log("<color='magenta'>" + msg + "</color>");
            }
        }

        public static void LogYellow(string msg, bool enabled = true)
        {
            if (enabled)
            {
                Debug.Log("<color='yellow'>" + msg + "</color>");
            }
        }
    }
}