using UnityEngine;
using UnityEngine.UI;

public class SetLevelNumber : MonoBehaviour
{
    public void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(SetNum);
    }

    public void SetNum()
    {
        PlayerPrefs.SetInt("Level number", int.Parse(gameObject.name));
        Debug.Log(gameObject.name);
    } 
}
