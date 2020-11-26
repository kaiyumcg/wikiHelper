using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace com.rvkm.unitygames.YouTube
{
    [System.Serializable]
    public class ChannelVideoInfoRaw
    {
        public string link;
        public int hours, minutes, seconds;
        public long PublishedAt_Tick;
        public string Title;
        public string Description;
        public string[] thumbsUrl;
        public string[] Tags;
        public string[] comments;
        public int viewCount;
    }

    public enum StringOpCondition { Exists, NotExists};
    public enum NumberOpCondition { GreaterThan, GreaThanOrEqual, Equal, LessThan, LessThanOrEqual}
    public class StringOperation
    {
        public string strValue;
        public StringOpCondition condition;
    }

    public class NumberOperation
    {
        public int numValue;
        public NumberOpCondition condition;
    }

    public class TimeOperation
    {
        public NumberOperation year, month, day, hour, minute, second;
    }

    public class SearchOperationValidator
    {
        public bool title, uploadTime, duration, viewCount;
    }

    public class SearchOperation
    {
        public SearchOperationValidator validator;
        public StringOperation titleOp;
        public TimeOperation uploadTimeOp, durationOp;
        public NumberOperation viewCountOp;
    }
}