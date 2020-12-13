using com.rvkm.unitygames.YouTubeSearch.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeSearch
{
    public static class Utility
    {
        /// <summary>
        /// https://social.msdn.microsoft.com/Forums/azure/en-US/d9bdc9fa-1cfa-4349-b9af-77436f102045/c-list-to-find-the-mode-and-median?forum=csharpgeneral
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal GetMedian(this IEnumerable<int> source)
        {
            // Create a copy of the input, and sort the copy
            int[] temp = source.ToArray();
            Array.Sort(temp);
            int count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                int a = temp[count / 2 - 1];
                int b = temp[count / 2];
                return (a + b) / 2m;
            }
            else
            {
                // count is odd, return the middle element
                return temp[count / 2];
            }
        }

        public static int GetMode(this IEnumerable<int> list)
        {
            // Initialize the return value
            int mode = default(int);
            // Test for a null reference and an empty list
            if (list != null && list.Count() > 0)
            {
                // Store the number of occurences for each element
                Dictionary<int, int> counts = new Dictionary<int, int>();
                // Add one to the count for the occurence of a character
                foreach (int element in list)
                {
                    if (counts.ContainsKey(element))
                        counts[element]++;
                    else
                        counts.Add(element, 1);
                }
                // Loop through the counts of each element and find the 
                // element that occurred most often
                int max = 0;
                foreach (KeyValuePair<int, int> count in counts)
                {
                    if (count.Value > max)
                    {
                        // Update the mode
                        mode = count.Key;
                        max = count.Value;
                    }
                }
            }
            return mode;
        }


        public static string GetStringIfNullOrEmpty(string str, ref bool success)
        {
            string result = "";
            if (string.IsNullOrEmpty(str) == false)
            {
                result = str;
                success = true;
            }
            return result;
        }

        public static string[] SplitByComaOrNewline(string inputString)
        {
            return Regex.Split(Regex.Replace(inputString, "^[,\r\n]+|[,\r\n]+$", ""), "[,\r\n]+");
        }

        static bool IsExact(string s)
        {
            string str = s.Replace("[", "");
            str = str.Replace("]", "");
            str = str.Replace("^", "");
            str = str.Replace("-", "");
            str = str.Replace("*", "");
            return str.StartsWith("\"") && str.StartsWith("\"");
        }

        static bool IsIgnore(string s)
        {
            string str = s.Replace("[", "");
            str = str.Replace("]", "");
            str = str.Replace("^", "");
            str = str.Replace("\"", "");
            str = str.Replace("*", "");
            return str.StartsWith("-") || str.EndsWith("-");
        }

        static bool IsCaseSensitive(string s)
        {
            string str = s.Replace("[", "");
            str = str.Replace("]", "");
            str = str.Replace("\"", "");
            str = str.Replace("-", "");
            str = str.Replace("*", "");
            return str.StartsWith("^");
        }

        static bool UseDeepSearch(string s)
        {
            return s.Contains("*");
        }

        static bool IsSecondary(string s)
        {
            return s.StartsWith("[") && s.EndsWith("]");
        }

        static string GetWord(string s)
        {
            string str = s.Replace("[", "");
            str = str.Replace("]", "");
            str = str.Replace("\"", "");
            str = str.Replace("-", "");
            str = str.Replace("^", "");
            str = str.Replace("*", "");
            return str;
        }

        static bool IsSeperateWord(string str, string word)
        {
            if (string.IsNullOrEmpty(word) || word.Length == 0)
            {
                return false;
            }
            else
            {
                int startID = -1;
                startID = str.IndexOf(word, 0);
                if (startID < 0)
                {
                    return false;
                }
                else
                {
                    int endID = -1;
                    endID = startID + word.Length - 1;
                    char c1 = str[startID];
                    char c2 = str[endID];
                    startID--;
                    endID++;

                    bool whiteSpaceAtStartCondition = false, whiteSpaceAtEndCondition = false;
                    if (startID >= 0)
                    {
                        whiteSpaceAtStartCondition = str[startID] == ' ' || str[startID] == ':' || str[startID] == '\"' || str[startID] == '[';
                    }
                    else
                    {
                        whiteSpaceAtStartCondition = true;
                    }

                    if (endID < str.Length)
                    {
                        whiteSpaceAtEndCondition = str[endID] == ' ' || str[endID] == ':' || str[endID] == '\"' || str[endID] == ']';
                    }
                    else
                    {
                        whiteSpaceAtEndCondition = true;
                    }
                    return whiteSpaceAtStartCondition && whiteSpaceAtEndCondition;
                }
            }
        }

        public static bool IsRelatableWith(this string thisString, string compareStringFromBlackOrWhiteList,
            bool caseSensitive, bool exactMatch, bool useDeepSearch)
        {
            bool relatable = false;
            if (useDeepSearch)
            {
                var comp = compareStringFromBlackOrWhiteList.Trim();
                if (thisString.Contains(comp) || thisString.Contains_IgnoreCase(comp))
                {
                    //var str = thisString.ToLower();
                    //var comp2 = comp.ToLower();
                    //relatable = IsSeperateWord(str, comp2);
                    if (caseSensitive && thisString.Contains(comp))
                    {
                        relatable = true;
                    }
                    else if (!caseSensitive && thisString.Contains_IgnoreCase(comp))
                    {
                        relatable = true;
                    }
                }
            }
            else
            {
                if (exactMatch)
                {
                    var comp = compareStringFromBlackOrWhiteList.Trim();
                    if (caseSensitive && thisString.Contains(comp))
                    {
                        var str = thisString.ToLower();
                        var comp2 = comp.ToLower();
                        relatable = IsSeperateWord(str, comp2);
                    }
                    else if (!caseSensitive && thisString.Contains_IgnoreCase(comp))
                    {
                        var str = thisString.ToLower();
                        var comp2 = comp.ToLower();
                        relatable = IsSeperateWord(str, comp2);
                    }
                }
                else
                {
                    string[] allCompWords = compareStringFromBlackOrWhiteList.Split(' ');
                    if (allCompWords != null && allCompWords.Length > 0)
                    {
                        foreach (var comp in allCompWords)
                        {
                            if (string.IsNullOrEmpty(comp) || comp == " ") { continue; }
                            if (caseSensitive && thisString.Contains(comp))
                            {
                                var str = thisString.ToLower();
                                var comp2 = comp.ToLower();
                                relatable = IsSeperateWord(str, comp2);
                            }
                            else if (!caseSensitive && thisString.Contains_IgnoreCase(comp))
                            {
                                var str = thisString.ToLower();
                                var comp2 = comp.ToLower();
                                relatable = IsSeperateWord(str, comp2);
                            }
                            if (relatable)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            
            return relatable;
        }

        public static void GetBlackWhitelist(string inputString, ref StringAndCaseDesc[] blackList, ref StringAndCaseDesc[] whiteList,
            ref StringAndCaseDesc[] secondaryblackList, ref StringAndCaseDesc[] secondarywhiteList)
        {
            var sps = Regex.Split(Regex.Replace(inputString, "^[,\r\n]+|[,\r\n]+$", ""), "[,\r\n]+");
            List<StringAndCaseDesc> bList = new List<StringAndCaseDesc>();
            List<StringAndCaseDesc> wList = new List<StringAndCaseDesc>();
            List<StringAndCaseDesc> sbList = new List<StringAndCaseDesc>();
            List<StringAndCaseDesc> swList = new List<StringAndCaseDesc>();
            if (sps != null && sps.Length > 0)
            {
                foreach (var s in sps)
                {
                    var exactMatch = IsExact(s);
                    var word = GetWord(s);
                    var caseSensitive = IsCaseSensitive(s);
                    var isSecondary = IsSecondary(s);
                    if (IsIgnore(s))
                    {  
                        var desc = new StringAndCaseDesc { caseSensitive = caseSensitive,
                            str = word, matchExactPhraseOrSentence = exactMatch,
                            useDeepSearch = UseDeepSearch(s) };
                        if (isSecondary)
                        {
                            sbList.Add(desc);
                        }
                        else
                        {
                            bList.Add(desc);
                        }
                    }
                    else
                    {
                        var desc = new StringAndCaseDesc { caseSensitive = caseSensitive,
                            str = word, matchExactPhraseOrSentence = exactMatch,
                            useDeepSearch = UseDeepSearch(s)
                        };
                        if (isSecondary)
                        {
                            swList.Add(desc);
                        }
                        else
                        {
                            wList.Add(desc);
                        }
                    }
                }
            }
            blackList = bList.ToArray();
            whiteList = wList.ToArray();
            secondaryblackList = sbList.ToArray();
            secondarywhiteList = swList.ToArray();
        }

        public static int GetIntFromString(string str, ref bool success)
        {
            int result = 0;
            if (string.IsNullOrEmpty(str) == false)
            {
                int count = 0;
                var perseSuccess = int.TryParse(str, out count);
                if (perseSuccess)
                {
                    result = count;
                    success = true;
                }
            }
            return result;
        }

        public static string GetWWWResponse(string Url)
        {
            string result = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (String.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                result = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
            return result;
        }

        /// <summary>
        /// https://codesnippets.fesslersoft.de/get-the-youtube-videoid-from-url/
        /// </summary>
        const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
        public static string GetVideoId(string url)
        {
            var regex = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
            foreach (Match match in regex.Matches(url))
            {
                foreach (var groupdata in match.Groups.Cast<Group>().Where(groupdata => !groupdata.ToString().StartsWith("http://") && !groupdata.ToString().StartsWith("https://") && !groupdata.ToString().StartsWith("youtu") && !groupdata.ToString().StartsWith("www.")))
                {
                    return groupdata.ToString();
                }
            }
            return string.Empty;
        }
    }
}