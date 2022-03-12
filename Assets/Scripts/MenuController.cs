using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public string loadLevel;
    public GameObject LoadingPanel;
    public Slider bar;

    public void StartButton(){

        LoadingPanel.SetActive(true);
        StartCoroutine(LoadAsync());
    }

    public void RestartTheApp()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CloseTheApp()
    {
        Application.Quit();
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(1);
        
        while (!async.isDone)
        {
            bar.value = async.progress;
            Debug.Log("Load completed");
            yield return null;
        }

    }
}
