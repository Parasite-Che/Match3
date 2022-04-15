using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public string loadLevel;
    public GameObject LoadingPanel;
    public GameObject first;
    public GameObject second;
    public List<GameObject> panelFromMenu;

    public Slider bar;

    public void LoadSceneButton(){
        LoadingPanel.SetActive(true);
        StartCoroutine(LoadAsync());
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

    public void MenuNavigation()
    {
        GameObject obj;
        for (int i = 0; i < panelFromMenu.Count; i++)
        {
            if (panelFromMenu[i].activeSelf)
            {
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
