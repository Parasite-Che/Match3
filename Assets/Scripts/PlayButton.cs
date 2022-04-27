using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public Text lives;

    public void Update()
    {

        if (gameObject.GetComponent<Image>().color != new Color32(77, 173, 138, 255) && int.Parse(lives.text) > 0)
        {
            gameObject.GetComponent<Image>().color = new Color32(77, 173, 138, 255);
        }
        else if (gameObject.GetComponent<Image>().color != new Color32(37, 133, 98, 255) && int.Parse(lives.text) <= 0)
        {
            gameObject.GetComponent<Image>().color = new Color32(37, 133, 98, 255);
        }
    }
}
