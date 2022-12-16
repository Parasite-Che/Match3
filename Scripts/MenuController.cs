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
        GameObject _obj;
        Color _col = buttonFromMenu[0].GetComponent<Image>().color;
        bool _sameButton = true;
        for (int i = 0; i < panelFromMenu.Count; i++)
        {
            if (panelFromMenu[i].activeSelf)
            {
                for (int j = 0; j < buttonFromMenu.Count; j++)
                {
                    if (buttonFromMenu[j].GetComponent<Image>().color != _col && buttonFromMenu[j] != gameObject)
                    {
                        buttonFromMenu[j].GetComponent<Lerping>().BackLerp(buttonFromMenu[j].transform.GetChild(1).gameObject);
                        buttonFromMenu[j].transform.GetChild(0).gameObject.SetActive(false);
                        buttonFromMenu[j].GetComponent<Image>().color = _col;
                        _sameButton = false;
                        break;
                    }
                }
                if (!_sameButton)
                {
                    gameObject.GetComponent<Image>().color = new Color32(87, 202, 173, 255);
                    gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    gameObject.GetComponent<Lerping>().Lerp(gameObject.transform.GetChild(1).gameObject);
                    _obj = panelFromMenu[i];
                    _obj.SetActive(false);
                    first.SetActive(true);
                }
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
