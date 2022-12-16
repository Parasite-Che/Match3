using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Newtonsoft.Json;

public class LiveController : MonoBehaviour
{
    DateTime currentTime;
    DateTime timeToAddLife;

    public Text timeText;
    public Text livesText;

    private void Update()
    {
        currentTime = DateTime.Now;
        timeToAddLife = JsonConvert.DeserializeObject<DateTime>(PlayerPrefs.GetString("timeToAddLife"));
        
        if (currentTime < timeToAddLife)
        {
            timeText.text = ((int)(timeToAddLife - currentTime).TotalMinutes % 24).ToString() + ":" + (timeToAddLife - currentTime).Seconds.ToString();
            livesText.text = (10 - Mathf.Ceil((float)(timeToAddLife - currentTime).TotalMinutes / 24)).ToString();
        }
        else
        {
            livesText.text = "10";
            timeText.text = "";
        }
    }
}
