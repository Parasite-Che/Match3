using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerping : MonoBehaviour
{
    GameObject obj;
    GameObject backObj;
    bool done = true;
    bool backDone = true;
    float time;
    Vector3 firstPos = new Vector3(0, 0, 0);
    Vector3 secondPos = new Vector3(0, 130, 0);
    Vector3 firstScale = new Vector3(1, 1, 0);
    Vector3 secondScale = new Vector3(1.5f, 1.5f, 0);

    // Update is called once per frame
    void Update()
    {
        if (!done)
        {
            time += Time.deltaTime;
            if (time > 1)
            {
                time = 1;
            }
            obj.transform.localPosition = Vector3.Lerp(firstPos, secondPos, time);
            obj.transform.localScale = Vector3.Lerp(firstScale, secondScale, time);
            if (time == 1)
            {
                time = 0;
                done = true;
                backDone = true;
            }
        }
        else if (!backDone)
        {
            time += Time.deltaTime;
            if (time > 1)
            {
                time = 1;
            }
            backObj.transform.localPosition = Vector3.Lerp(secondPos, firstPos, time);
            backObj.transform.localScale = Vector3.Lerp(secondScale, firstScale, time);
            if (time == 1)
            {
                time = 0;
                done = true;
                backDone = true;
            }
        }
    }

    public void Lerp(GameObject Obj)
    {
        done = false;
        obj = Obj;
    }

    public void BackLerp(GameObject Obj)
    {
        backDone = false;
        backObj = Obj;
    }
}
