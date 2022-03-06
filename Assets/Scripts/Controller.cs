using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public List<Panel> panels = new List<Panel>();

    public float ppX;
    public float ppY;
    int countOfScore = 0;
    int scorePerPanel = 10;

    public CreatingPanels creatingPanel;
    public GameObject currentPanel;
    public GameObject secondPanel;
    public GameObject marker;
    public GameObject UIbonusBar;
    readonly GameObject[] objList = new GameObject[4];


    public bool hold = false;
    public bool lockDirectX = false;
    public bool lockDirectY = false;
    public bool matchFound = false;

    public Text upgradeTimeUI;
    public float upgradeTimeDecrease;
    public float upgradeTimeMax;
    public Vector2 clickPos;
    public RaycastHit2D hitPanel;

    private void Start()
    {
        Application.targetFrameRate = 60;
        creatingPanel.CreateField(creatingPanel.countOfPanelsOY, creatingPanel.countOfPanelsOX);
        UIbonusBar.transform.parent.gameObject.SetActive(false);
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

    /// Blocks one of the axes so that the tile can only be moved along the X or Y axis  ///
    
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

    public void OnBeginDrag(PointerEventData eventData) { }

                                ///     Determines the final direction     ///
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

                                ///     Hitmarker control     ///
                                
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
            if ((hitPanel && matchFound) || 
                (hitPanel.transform.gameObject.GetComponent<Panels>().ID > 300) ||
                (currentPanel.GetComponent<Panels>().ID > 300))
            {
                currentPanel.transform.position = hitPanel.transform.gameObject.transform.position;
                hitPanel.transform.gameObject.transform.position = new Vector3(ppX, ppY, 0);
                UseBonus(currentPanel);
                UseBonus(hitPanel.transform.gameObject);
                matchFound = false;
            }
            else
            {
                currentPanel.transform.position = new Vector3(ppX, ppY, 0);
            }
        }
    }

    public void UseBonus(GameObject obj)
    {
        if (obj.GetComponent<Panels>().ID > 300)
        {
            if (obj.GetComponent<Panels>().bonusName == "CubeBonus")
            {
                _ = new BonusControl<CubeBonus>(new CubeBonus());
            }
            else if (obj.GetComponent<Panels>().bonusName == "LineBonus4")
            {
                _ = new BonusControl<LineBonus4>(new LineBonus4());
            }
            else if (obj.GetComponent<Panels>().bonusName == "LineBonus5")
            {
                _ = new BonusControl<LineBonus5>(new LineBonus5());
            }
            else if (obj.GetComponent<Panels>().bonusName == "LinesOf3Panels")
            {
                _ = new BonusControl<LinesOf3Panels>(new LinesOf3Panels());
            }
        }
    }


                                ///     General method for finding matches     ///

    public void AllMatches(bool oneObj, GameObject obj)
    {
        if (oneObj)
        {
            obj.GetComponent<BoxCollider2D>().enabled = false;
            ClearMatchOnLine(new Vector2[2] { Vector2.left, Vector2.right }, obj, obj.transform.position);
            ClearMatchOnLine(new Vector2[2] { Vector2.up, Vector2.down }, obj, obj.transform.position);

            ClearMatchOnCub(obj, new Vector3(1, 1, 0), obj.transform.position);
            ClearMatchOnCub(obj, new Vector3(-1, 1, 0), obj.transform.position);
            ClearMatchOnCub(obj, new Vector3(1, -1, 0), obj.transform.position);
            ClearMatchOnCub(obj, new Vector3(-1, -1, 0), obj.transform.position);
            obj.GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            if (hitPanel)
            {

                ////        Clear match on Axes         ////

                ClearMatchOnLine(new Vector2[2] { Vector2.left, Vector2.right }, currentPanel, hitPanel.transform.gameObject.transform.position);
                ClearMatchOnLine(new Vector2[2] { Vector2.up, Vector2.down }, currentPanel, hitPanel.transform.gameObject.transform.position);
                ClearMatchOnLine(new Vector2[2] { Vector2.left, Vector2.right }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                ClearMatchOnLine(new Vector2[2] { Vector2.up, Vector2.down }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));

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
    }

                            ///      Find Matches and delete sprite     ///

    private List<GameObject> FindMatch(Vector2 castDir, GameObject firstObj, Vector3 secObjPos)

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

    void ClearMatchOnLine(Vector2[] paths, GameObject firstObj, Vector3 secObjPos)
    {

        List<GameObject> matchingTiles = new List<GameObject> { firstObj };
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i], firstObj,  secObjPos));
        }
        if (matchingTiles.Count >= 3)
        {
            if (matchingTiles.Count == 3)
            {
                for (int i = 0; i < matchingTiles.Count; i++)
                {
                    matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
                    countOfScore += scorePerPanel;
                }
            }
            else if (matchingTiles.Count == 4)
            {
                for (int i = 0; i < matchingTiles.Count; i++)
                {
                    matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
                    countOfScore += (int)(scorePerPanel * 1.3f);
                }
                matchingTiles[UnityEngine.Random.Range(0, matchingTiles.Count)].GetComponent<Panels>().bonusName = "_4LineBonus";
            }
            else
            {
                for (int i = 0; i < matchingTiles.Count; i++)
                {
                    matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
                    countOfScore += scorePerPanel * 2;
                }
                matchingTiles[UnityEngine.Random.Range(0, matchingTiles.Count)].GetComponent<Panels>().bonusName = "_5LineBonus";
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
                objList[UnityEngine.Random.Range(0, 4)].gameObject.GetComponent<Panels>().bonusName = "_CubeBonus";
                for (int i = 0; i < 4; i++)
                {
                    objList[i].GetComponent<SpriteRenderer>().sprite = null;
                    countOfScore += (int)(scorePerPanel * 1.5f);
                }
                matchFound = true;
            }

        }
    }
    // _ = new BonusControl<Bonus1>(hits[rand].transform.gameObject, new Bonus1());

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
public struct Panel
{
    public GameObject obj;
    public int ID;
}
