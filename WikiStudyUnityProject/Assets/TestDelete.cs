﻿using com.rvkm.unitygames.YouTubeSearch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDelete : MonoBehaviour
{
    //[SerializeField] string inputStr;
    // public float totalMins;
    // [SerializeField] bool success;
    // [SerializeField] string urlTest = "https://www.youtube.com/watch?v=yqCHiZrgKzs&amp;t=1622s";
    //[SerializeField] string inputStr, patternToDelete, outputStr;
    //[SerializeField] string inputStr, compStr;
    //[SerializeField] bool contains;
    [Multiline]
    [SerializeField] string inputStr, wordComp;
    [SerializeField] bool isWord;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    static bool GetNumberBeforeACharacter(char c, string inputString, ref int number)
    {
        int startIdx = -1;
        startIdx = inputString.IndexOf(c);
        if (startIdx < 0)
        {
            Debug.Log("idx less than zero 1  for char: " + c);
            return false;
        }
        startIdx = startIdx - 1;
        if (startIdx < 0)
        {
            Debug.Log("idx less than zero 2  for char: " + c);
            return false;
        }
        List<char> cList = new List<char>();
        for (int i = startIdx; i > -1; i--)
        {
            if (char.IsDigit(inputString[i]))
            {
                cList.Add(inputString[i]);
            }
            else
            {
                break;
            }
        }
        cList.Reverse();
        if (cList.Count <= 0)
        {
            Debug.Log("cList cound is zero or lesser  for char: " + c);
            return false;
        }

        string NumStr = "";
        for (int i = 0; i < cList.Count; i++)
        {
            NumStr += cList[i];
        }

        var convSuccess = int.TryParse(NumStr, out number);
        if (!convSuccess)
        {
            Debug.Log("conversion failed for numStr:" + NumStr + "  for char: " + c);
            return false;
        }

        return true;
    }

    public static int GetDurationInMinute(string durationStr, ref bool success)
    {
        int hour = 0, minute = 0, second = 0, day = 0;
        var hSuccess = GetNumberBeforeACharacter('H', durationStr, ref hour);
        var mSuccess = GetNumberBeforeACharacter('M', durationStr, ref minute);
        var sSuccess = GetNumberBeforeACharacter('S', durationStr, ref second);
        var dSuccess = GetNumberBeforeACharacter('D', durationStr, ref day);
        var min = day * 24 * 60 + hour * 60 + minute + (int)((float)second / (float)60);
        Debug.Log("h success? " + hSuccess + " and m success? " + mSuccess + " and s success? " + sSuccess + " and d success? " + dSuccess);
        Debug.Log("hour: " + hour + " and minute: " + minute + " and second: " + second + " and day: " + day);

        success = true;
        if (durationStr.Contains("H") && hSuccess == false) { success = false; }
        if (durationStr.Contains("M") && mSuccess == false) { success = false; }
        if (durationStr.Contains("S") && sSuccess == false) { success = false; }
        if (durationStr.Contains("D") && dSuccess == false) { success = false; }
        return min;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("start!");
            //totalMins = GetDurationInMinute(inputStr, ref success);
            isWord = IsSeperateWord(inputStr, wordComp);

            //string videoID = Utility.GetVideoId(urlTest);
           // Debug.Log("video ID: " + videoID);
        }
    }

    bool IsSeperateWord(string str, string word)
    {
        if (string.IsNullOrEmpty(word) || word.Length == 0)
        {
            return false;
        }
        else
        {
            int startID = -1;
            startID = str.IndexOf(word, 0);
            Debug.Log("start ID: " + startID);
            if (startID < 0)
            {
                return false;
            }
            else
            {
                int endID = -1;
                endID = startID + word.Length - 1;
                Debug.Log("start id: " + startID + " and end id: " + endID);

                char c1 = str[startID];
                char c2 = str[endID];
                Debug.Log("char in start: " + c1 + " and in end: " + c2);
                startID--;
                endID++;

                bool whiteSpaceAtStartCondition = false, whiteSpaceAtEndCondition = false;
                if (startID >= 0)
                {
                    whiteSpaceAtStartCondition = str[startID] == ' ';
                }
                else
                {
                    whiteSpaceAtStartCondition = true;
                }

                if (endID < str.Length)
                {
                    whiteSpaceAtEndCondition = str[endID] == ' ';
                }
                else
                {
                    whiteSpaceAtEndCondition = true;
                }
                return whiteSpaceAtStartCondition && whiteSpaceAtEndCondition;
            }
        }
    }
}
