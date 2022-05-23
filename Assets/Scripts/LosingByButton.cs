using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LosingByButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Loosing);
    }

    void Loosing()
    {
        PlayerPrefs.SetInt("WinStreak", 0);
        DateTime time = JsonConvert.DeserializeObject<DateTime>(PlayerPrefs.GetString("timeToAddLife"));

        if (DateTime.Now < time)
        {
            PlayerPrefs.SetString("timeToAddLife", JsonConvert.SerializeObject(time.AddMinutes(24)));
        }
        else
        {
            PlayerPrefs.SetString("timeToAddLife", JsonConvert.SerializeObject(DateTime.Now.AddMinutes(24)));
        }
    }
}
