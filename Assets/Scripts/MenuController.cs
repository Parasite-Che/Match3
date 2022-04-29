using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;
using Newtonsoft.Json;

public class MenuController : MonoBehaviour
{
    public string loadLevel;
    public GameObject LoadingPanel;
    public GameObject first;
    public GameObject second;
    public GameObject mapButton;
    public LiveController LC;

    public List<GameObject> panelFromMenu;
    public List<GameObject> buttonFromMenu;

    public Slider bar;

    public void LoadSceneButton(){
        LoadingPanel.SetActive(true);
        StartCoroutine(LoadAsync());
    }

    public void LoadLevelButton()
    {
        if (int.Parse(LC.livesText.text) > 0)
        {
            LoadingPanel.SetActive(true);
            StartCoroutine(LoadAsync());
        }
    }

    public void LoadUIButton()
    {
        first.SetActive(false);
        second.SetActive(true);
    }

    public void RestartTheApp()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetAllProgrress()
    {
        PlayerPrefs.DeleteAll();
        LoadSceneButton();
    }

    public void PopUpActiv()
    {
        first.SetActive(true);
    }

    public void PopUpDeactiv()
    {
        first.SetActive(false);
    }

    public void MenuNavigation()
    {
        GameObject obj;
        Color col = buttonFromMenu[0].GetComponent<Image>().color;
        for (int i = 0; i < panelFromMenu.Count; i++)
        {
            if (panelFromMenu[i].activeSelf)
            {
                if (mapButton.GetComponent<Image>().color == new Color32(47, 162, 133, 255))
                {
                    mapButton.GetComponent<Image>().color = new Color32(87, 202, 173, 255);
                }
                for (int j = 0; j < buttonFromMenu.Count; j++)
                {
                    if (buttonFromMenu[j].GetComponent<Image>().color != col)
                    {
                        Debug.Log(buttonFromMenu[j].GetComponent<Image>().color);
                        buttonFromMenu[j].GetComponent<Image>().color = col;
                        Debug.Log(buttonFromMenu[j]);
                        Debug.Log(buttonFromMenu[j].GetComponent<Image>().color);
                        break;
                    }
                }
                gameObject.GetComponent<Image>().color -= new Color32(40, 40, 40, 0);

                obj = panelFromMenu[i];
                obj.SetActive(false);
                first.SetActive(true);
                break;
            }
        }
    }

    public void CloseTheApp()
    {
        Application.Quit();
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(loadLevel);
        while (!async.isDone)
        {
            bar.value = async.progress;
            yield return null;
        }
    }
}
