using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDelete : MonoBehaviour
{
    [Range(2012, 2060)]
    public int year;
    [Range(1,12)]
    public int month;
    [Range(1, 31)]
    public int day;
    [Range(1,24)]
    public int hour;
    [Range(1,60)]
    public int minute;
    [Range(1,60)]
    public int second;
    public float addDays; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DateTime d = new DateTime(year, month, day, hour, minute, second);
            int days = (int)addDays;
            float fracDays = addDays - days;
            Debug.Log("let us add days: " + addDays + " frac part: " + fracDays);
            d = d.AddDays(addDays);

            year = d.Year; month = d.Month; day = d.Day;
            hour = d.Hour; minute = d.Minute; second = d.Second;
        }
    }
}
