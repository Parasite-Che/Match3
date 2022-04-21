using UnityEngine;
using UnityEngine.UI;
using System;

public class SetLevelNumber : MonoBehaviour
{
    public Text lvl;
    public GameObject[] stars;

    JsonControl JC;
    Save save;

    public void Start()
    {
        JC = new JsonControl();
        save = JC.LoadJson();
        int num = int.Parse(gameObject.name) - 1;
        if (save.levels[num, 0] != 0)
        {
            for (int i = 0; i < save.levels[num, 0]; i++)
            {
                stars[i].SetActive(true);
            }
        }

        gameObject.GetComponent<Button>().onClick.AddListener(SetNum);
    }

    public void SetNum()
    {
        PlayerPrefs.SetInt("Level number", int.Parse(gameObject.name));
        lvl.text = "Level " + PlayerPrefs.GetInt("Level number").ToString();
        Debug.Log(gameObject.name);
    } 
}
