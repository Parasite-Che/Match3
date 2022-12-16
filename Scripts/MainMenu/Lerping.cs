using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerping : MonoBehaviour
{
    GameObject obj;
    GameObject backObj;
    bool done = true;
    bool backDone = true;
    bool lerped = false;
    float time;
    Vector3 firstPos = new Vector3(0, 0, 0);
    Vector3 secondPos = new Vector3(0, 130, 0);
    Vector3 firstScale = new Vector3(1, 1, 0);
    Vector3 secondScale = new Vector3(1.5f, 1.5f, 0);

    // Update is called once per frame
    void Update()
    {
        if (!done && !lerped)
        {
            time += Time.deltaTime * 3;
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
                lerped = true;
            }
        }
        if (!backDone && lerped)
        {
            time += Time.deltaTime * 3;
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
                lerped = false;
            }
        }

    }

    public void Lerp(GameObject Obj)
    {
        done = false;
        backDone = true;
        lerped = false;
        obj = Obj;
    }

    public void BackLerp(GameObject Obj)
    {
        backDone = false;
        done = true;
        lerped = true;
        backObj = Obj;
    }
}
