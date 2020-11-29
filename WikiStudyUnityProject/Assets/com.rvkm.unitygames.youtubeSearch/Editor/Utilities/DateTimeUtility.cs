using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class DateTimeUtility
    {
        public static float GetDurationInMinute(string durationStr)
        {
            string hStr = "", mStr = "", sStr = "";
            if (durationStr.Contains("PT"))
            {
                durationStr = durationStr.Replace("PT", "");
            }

            var cArray = durationStr.ToCharArray();
            int h_id_start = 0, m_id_start = 0, s_id_start = 0;
            for (int i = 0; i < cArray.Length; i++)
            {
                if (cArray[i] == 'H')
                {
                    h_id_start = i;
                }
                if (cArray[i] == 'M')
                {
                    m_id_start = i;
                }
                if (cArray[i] == 'S')
                {
                    s_id_start = i;
                }
            }

            if (durationStr.Contains("H"))
            {
                for (int i = 0; i < h_id_start; i++)
                {
                    if (char.IsDigit(cArray[i]) || cArray[i] == '.')
                    {
                        if (hStr.Contains("."))
                        {
                            if (char.IsDigit(cArray[i]))
                            {
                                hStr += cArray[i];
                            }
                        }
                        else
                        {
                            hStr += cArray[i];
                        }

                    }
                }
            }

            if (durationStr.Contains("M"))
            {
                int m_end = 0;
                for (int i = m_id_start - 1; i >= 0; i--)
                {
                    if (char.IsDigit(cArray[i]) == false && cArray[i] != '.')
                    {
                        m_end = i;
                        break;
                    }
                }

                if (m_end > 0)
                {
                    for (int i = m_end + 1; i < m_id_start; i++)
                    {
                        if (char.IsDigit(cArray[i]) || cArray[i] == '.')
                        {
                            if (mStr.Contains("."))
                            {
                                if (char.IsDigit(cArray[i]))
                                {
                                    mStr += cArray[i];
                                }
                            }
                            else
                            {
                                mStr += cArray[i];
                            }
                        }
                    }
                }
            }

            if (durationStr.Contains("S"))
            {
                int s_end = 0;
                for (int i = s_id_start - 1; i >= 0; i--)
                {
                    if (char.IsDigit(cArray[i]) == false && cArray[i] != '.')
                    {
                        s_end = i;
                        break;
                    }
                }

                if (s_end > 0)
                {
                    for (int i = s_end + 1; i < s_id_start; i++)
                    {
                        if (char.IsDigit(cArray[i]) || cArray[i] == '.')
                        {
                            if (sStr.Contains("."))
                            {
                                if (char.IsDigit(cArray[i]))
                                {
                                    sStr += cArray[i];
                                }
                            }
                            else
                            {
                                sStr += cArray[i];
                            }
                        }
                    }
                }
            }
            float hour = 0f;
            float minute = 0f;
            float second = 0f;
            float.TryParse(hStr, out hour);
            float.TryParse(mStr, out minute);
            float.TryParse(sStr, out second);

            return hour * 60f + minute + (second / 60f);
        }

        public static bool GetDate(string publishedDateStr, ref DateTime publishedDate)
        {
            publishedDate = new DateTime(1980, 1, 1, 1, 1, 1, 1);
            var success = DateTime.TryParse(publishedDateStr, out publishedDate);
            return success;
        }
    }
}