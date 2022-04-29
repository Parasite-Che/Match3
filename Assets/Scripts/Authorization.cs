using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Newtonsoft.Json;


public class Authorization : MonoBehaviour
{
    public GameObject menu;
    public GameObject author;
    public string playerName = "";
    public InputField field;
    JsonControl JC;

    //int allStars = 0;

    private void Awake()
    {
        
        playerName = PlayerPrefs.GetString("PlayerName");
        JC = new JsonControl();
        if (playerName == "")
        {
            PlayerPrefs.SetString("timeToAddLife", JsonConvert.SerializeObject(DateTime.Now));
            JC.SaveJson(JC.LoadSavesFromRecurces());

            menu.SetActive(false);
            author.SetActive(true);
        }

        /*
        Save lvl = JC.LoadJson();
        for (int i = 0; i < lvl.levels.GetLength(0); i++)
        {
            allStars += lvl.levels[i, 0];
        }
        */
    }

    public void AuthorButton()
    {
        if (field.text != "")
        {
            playerName = field.text;
            field.text = "";
            PlayerPrefs.SetString("PlayerName", playerName);
            author.SetActive(false);
            menu.SetActive(true);
        }
    }
}
