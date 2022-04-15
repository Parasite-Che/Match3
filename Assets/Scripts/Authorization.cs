using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Authorization : MonoBehaviour
{
    public GameObject menu;
    public GameObject author;
    public string playerName = "";
    public InputField field;

    private void Awake()
    {
        playerName = PlayerPrefs.GetString("PlayerName");
        if (playerName == "")
        {
            menu.SetActive(false);
            author.SetActive(true);
        }
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
