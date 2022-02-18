using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public List<Panel> panels = new List<Panel>();
    public int countOfPanelsOX;
    public int countOfPanelsOY;
    public Vector3 startPosition;

    GameObject[] objList = new GameObject[4];
    public GameObject currentPanel;
    public GameObject secondPanel;
    public GameObject marker;

    public bool hold = false;
    public bool lockDirectX = false;
    public bool lockDirectY = false;
    public bool matchFound = false;
    public bool isFalling = false;
    public Vector2 clickPos;
    public RaycastHit2D hitPanel;

    public float ppX;
    public float ppY;
    int countOfScore = 0;
    


    private void Start()
    {
        Application.targetFrameRate = 60;
        CreateField(countOfPanelsOY, countOfPanelsOX);
    }

    private void Update()
    {
        if (currentPanel != null) //moves the panel along one of the axes
        {
            if (lockDirectX == true)
            {
                currentPanel.transform.position = new Vector3(ppX + ((Input.mousePosition.x - clickPos.x) / Screen.width), ppY, -1);
            }
            else if (lockDirectY == true)
            {
                currentPanel.transform.position = new Vector3(ppX, ppY + ((Input.mousePosition.y - clickPos.y) / Screen.height), -1);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (lockDirectX == false && lockDirectY == false)
        {
            if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
            {
                lockDirectX = true;
            }
            else
            {
                lockDirectY = true;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }


    void CreateField(int col, int row) 
    {
        Vector3 cp = startPosition;
        int rand = -1;
        int[] previousAbove = new int[countOfPanelsOX];
        int previousLeft = -1;

        for (int i = 0; i < col; i++)
        {
            for (int j = 0; j < row; j++)
            {

                while (rand == previousLeft || rand == previousAbove[j])
                {
                    rand = Random.Range(0, 6);
                }

                if (rand != -1) {
                    GameObject obj = Instantiate<GameObject>(panels[rand].obj, new Vector3(cp.x + j, cp.y - i, 0), Quaternion.identity);
                    obj.GetComponent<Panels>().ID = rand;
                    previousAbove[j] = obj.GetComponent<Panels>().ID;
                    previousLeft = obj.GetComponent<Panels>().ID;
                }
            }
            previousLeft = -1;
            cp.x = startPosition.x;
        }
    }

    public void CreatePanel(int ID ,Vector3 pos)
    {
        GameObject obj = Instantiate<GameObject>(panels[ID].obj, pos, Quaternion.identity);
        obj.GetComponent<Panels>().ID = ID;
    }

    public void FillingInEmptyFields(float posX)
    {
        RaycastHit2D[] panels = Physics2D.RaycastAll(new Vector3(posX, -startPosition.y - 1, 0), Vector2.up, 100f, LayerMask.GetMask("Panel"));
        if (panels.Length < countOfPanelsOY)
        {
            for (int i = 0; i < (countOfPanelsOY - panels.Length); i++)
            {
                CreatePanel(Random.Range(0, 6), new Vector3(posX, startPosition.y  - i, 0));
            }
        }
        isFalling = false;
    }

    public Vector3 Direction()
    {
        if (lockDirectX == true)
        {
            if ((Input.mousePosition.x - clickPos.x) / Screen.width > 0.02f)
                return new Vector3(1, 0);
            else if ((Input.mousePosition.x - clickPos.x) / Screen.width < -0.02f)
                return new Vector3(-1, 0);
            else
                return new Vector3(0, 0);
        }
        else if (lockDirectY == true)
        {
            if ((Input.mousePosition.y - clickPos.y) / Screen.height > 0.04f)
                return new Vector3(0, 1);
            else if ((Input.mousePosition.y - clickPos.y) / Screen.height < -0.04f)
                return new Vector3(0, -1);
            else
                return new Vector3(0, 0);
        }
        else 
            return new Vector3(1, 1);
    }

    public void HitMarker(Vector3 pos, bool condition)
    {
        if (pos != null)
        {
            marker.SetActive(condition);
            marker.transform.position = pos;
        }
        else
            marker.SetActive(condition);
    }

                                ///    Rearrangement of tiles     ///

    public void Transposition()
    {
        if (Direction() == new Vector3(0, 0))
        {
            currentPanel.transform.position = new Vector3(ppX, ppY, 0);
        }
        else
        {
            if (hitPanel && matchFound)
            {
                currentPanel.transform.position = hitPanel.transform.gameObject.transform.position;
                hitPanel.transform.gameObject.transform.position = new Vector3(ppX, ppY, 0);
                matchFound = false;
            }
            else
            {
                currentPanel.transform.position = new Vector3(ppX, ppY, 0);
            }
        }
    }

                                ///     General method for finding matches     ///

    public void AllMatches()
    {
        if (hitPanel)
        {
                                      ////        Clear match on Axes         ////

            ClearMatchOnLine(new Vector2[2] { Vector2.left, Vector2.right }, currentPanel, hitPanel.transform.gameObject, hitPanel.transform.gameObject.transform.position);
            ClearMatchOnLine(new Vector2[2] { Vector2.up, Vector2.down }, currentPanel, hitPanel.transform.gameObject, hitPanel.transform.gameObject.transform.position);
            ClearMatchOnLine(new Vector2[2] { Vector2.left, Vector2.right }, hitPanel.transform.gameObject, currentPanel, new Vector3(ppX, ppY, 0));
            ClearMatchOnLine(new Vector2[2] { Vector2.up, Vector2.down }, hitPanel.transform.gameObject, currentPanel, new Vector3(ppX, ppY, 0));

                                      ////        Clear match on cube         ////

            ClearMatchOnCub(currentPanel, new Vector3(1, 1, 0), hitPanel.transform.position);
            ClearMatchOnCub(currentPanel, new Vector3(-1, 1, 0), hitPanel.transform.position);
            ClearMatchOnCub(currentPanel, new Vector3(1, -1, 0), hitPanel.transform.position);
            ClearMatchOnCub(currentPanel, new Vector3(-1, -1, 0), hitPanel.transform.position);

            ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(1, 1, 0), new Vector3(ppX, ppY, 0));
            ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(-1, 1, 0), new Vector3(ppX, ppY, 0));
            ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(1, -1, 0), new Vector3(ppX, ppY, 0));
            ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(-1, -1, 0), new Vector3(ppX, ppY, 0));
        }
    }
    
                                ///      Find Matches      ///
    
    private List<GameObject> FindMatch(Vector2 castDir, GameObject firstObj, GameObject secondObj, Vector3 secObjPos)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(secObjPos, castDir, 1f, LayerMask.GetMask("Panel"));
        while (hit.collider != null && hit.transform.gameObject.GetComponent<Panels>().ID == firstObj.GetComponent<Panels>().ID)
        {
            matchingTiles.Add(hit.collider.gameObject);
            hit.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            RaycastHit2D hit2 = Physics2D.Raycast(hit.transform.gameObject.transform.position, castDir, 1f, LayerMask.GetMask("Panel"));
            hit.transform.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            hit = hit2;
        }
        return matchingTiles;

    }

    void ClearMatchOnLine(Vector2[] paths, GameObject firstObj, GameObject secondObj, Vector3 secObjPos)
    {

        List<GameObject> matchingTiles = new List<GameObject>();
        matchingTiles.Add(firstObj);
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i], firstObj, secondObj,  secObjPos));
        }
        if (matchingTiles.Count >= 3)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                //Debug.Log(matchingTiles[i]);
                matchingTiles[i].GetComponent<Panels>().Controller = gameObject.GetComponent<Controller>();
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
                countOfScore += 100;
            }
            GameObject.Find("Score").GetComponent<Text>().text = "Score: " + countOfScore.ToString();
            matchFound = true;
        }
    }


    void ClearMatchOnCub(GameObject Obj, Vector3 dir, Vector3 objPos)
    {
        objList[0] = Obj;
        if (Physics2D.Raycast(objPos, dir, 1f, LayerMask.GetMask("Panel")) &&
            Physics2D.Raycast(objPos, new Vector3(dir.x, 0, 0), 1f, LayerMask.GetMask("Panel")) &&
            Physics2D.Raycast(objPos, new Vector3(0, dir.y, 0), 1f, LayerMask.GetMask("Panel")))
        {
            objList[1] = Physics2D.Raycast(objPos, dir, 1f, LayerMask.GetMask("Panel")).transform.gameObject;
            objList[2] = Physics2D.Raycast(objPos, new Vector3(dir.x, 0, 0), 1f, LayerMask.GetMask("Panel")).transform.gameObject;
            objList[3] = Physics2D.Raycast(objPos, new Vector3(0, dir.y, 0), 1f, LayerMask.GetMask("Panel")).transform.gameObject;

            if (objList[0].GetComponent<Panels>().ID == objList[1].GetComponent<Panels>().ID &&
                objList[0].GetComponent<Panels>().ID == objList[2].GetComponent<Panels>().ID &&
                objList[0].GetComponent<Panels>().ID == objList[3].GetComponent<Panels>().ID)
            {
                for (int i = 0; i < 4; i++)
                {
                    objList[i].GetComponent<Panels>().Controller = gameObject.GetComponent<Controller>();
                    objList[i].GetComponent<SpriteRenderer>().sprite = null;
                }
                countOfScore += 400;
                matchFound = true;
            }

        }
    }

                                ///      Button controllers      ///

    public void RestartTheApp()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CloseTheApp()
    {
        Application.Quit();
    }
}

[System.Serializable]
public struct Panel{
    public GameObject obj;
    public int ID;
}