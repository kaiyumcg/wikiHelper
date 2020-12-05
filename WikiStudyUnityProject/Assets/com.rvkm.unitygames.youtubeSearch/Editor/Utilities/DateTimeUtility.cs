using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class DateTimeUtility
    {
        static bool GetNumberBeforeACharacter(char c, string inputString, ref int number)
        {
            int startIdx = -1;
            startIdx = inputString.IndexOf(c);
            if (startIdx < 0)
            {
                return false;
            }
            startIdx = startIdx - 1;
            if (startIdx < 0)
            {
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

            success = true;
            if (durationStr.Contains("H") && hSuccess == false) { success = false; }
            if (durationStr.Contains("M") && mSuccess == false) { success = false; }
            if (durationStr.Contains("S") && sSuccess == false) { success = false; }
            if (durationStr.Contains("D") && dSuccess == false) { success = false; }
            return min;
        }

        public static bool GetDate(string publishedDateStr, ref DateTime publishedDate)
        {
            publishedDate = new DateTime(1980, 1, 1, 1, 1, 1, 1);
            var success = DateTime.TryParse(publishedDateStr, out publishedDate);
            return success;
        }
    }
}