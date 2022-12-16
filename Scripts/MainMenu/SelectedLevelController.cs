using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedLevelController : MonoBehaviour
{
    public GameObject[] goalsList = new GameObject[6];
    UIBonusBar g;

    private void Awake()
    {
        g = gameObject.GetComponent<UIBonusBar>();
    }

    public void OnEnable()
    {
        JsonControl JC = new JsonControl();
        Level level = JC.LoadFromRecurces(PlayerPrefs.GetInt("Level number").ToString());
        

        for (int i = 0; goalsList.Length < i; i++)
        {
            goalsList[i].transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = level.goals[0, i].ToString();
            //Debug.Log(goalsList[i].transform.GetChild(0));
            //Debug.Log(goalsList[i]);
            //Debug.Log(level.goals[0, i]);
        }
    }

    public void Update()
    {
        g.SetValue(PlayerPrefs.GetInt("WinStreak") / 3.0f);
        JsonControl JC = new JsonControl();
        Level level = JC.LoadFromRecurces(PlayerPrefs.GetInt("Level number").ToString());
        

        for (int i = 0; goalsList.Length < i; i++)
        {
            goalsList[i].transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = level.goals[0, i].ToString();
           // Debug.Log(goalsList[i].transform.GetChild(0));
           // Debug.Log(level.goals[0, i]);
        }
    }

}
