using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public string loadLevel;
    public GameObject LoadingPanel;
    public GameObject first;
    public GameObject second;
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
