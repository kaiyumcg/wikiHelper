using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames
{
    public class GameDebug
    {
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