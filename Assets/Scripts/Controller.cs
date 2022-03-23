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
    public int[] panelGoal;
    public int countOfMoves;
    public int countOfScore = 0;
    int scorePerPanel = 10;

    public CreatingPanels creatingPanel;
    public GameObject currentPanel;
    public GameObject secondPanel;
    public GameObject marker;
    public GameObject layout;
    public GameObject field;
    public GameObject loseScreen;
    public GameObject WinScreen;
    public GameObject[] goalsList;
    readonly GameObject[] objList = new GameObject[4];


    public bool hold = false;
    public bool lockDirectX = false;
    public bool lockDirectY = false;
    public bool matchFound = false;
    public bool verticalBonus = false;

    public Text winText;
    public Text loseText;
    public Text moves;
    public Vector2 clickPos;
    public RaycastHit2D hitPanel;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        creatingPanel.CreateField(creatingPanel.countOfPanelsOY, creatingPanel.countOfPanelsOX);
        CreatingGoal();
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
            return new Vector3(0, 0, 1);
    }

    public void CreatingGoal()
    {
        panelGoal = new int[panels.Count];
        goalsList = new GameObject[panels.Count];
        //panelGoal[0] = 40;
        panelGoal[1] = 40; 
        //panelGoal[2] = 40;
        panelGoal[3] = 40;
        //panelGoal[4] = 40;
        countOfMoves = 40;
        int countGoals = 0;
        moves.text = "Moves: " + countOfMoves.ToString();
        for (int i = 0; i < panelGoal.Length; i++)
        {
            if (panelGoal[i] > 0)
            {
                countGoals++;
            }
        }

        if(countGoals % 2 == 1)
        {
            if (countGoals == 1)
            {
                for (int i = 0; i < panelGoal.Length; i++)
                {       
                    if (panelGoal[i] > 0)
                    {
                        GameObject obj = Instantiate(layout, new Vector3(0, creatingPanel.countOfPanelsOY, 0), Quaternion.identity, GameObject.Find("Canvas").transform);
                        obj.GetComponent<Image>().color = panels[i].obj.GetComponent<SpriteRenderer>().color;
                        obj.transform.GetChild(0).GetComponent<Text>().text = panelGoal[i].ToString();
                        goalsList[i] = obj;
                        break;
                    }
                }
            }
            else
            {
                int j = 0;
                for (int i = 0; i < panelGoal.Length; i++)
                {
                    if (panelGoal[i] > 0)
                    {
                        GameObject obj = Instantiate(layout, new Vector3(-countGoals + 1 + j * 2, creatingPanel.countOfPanelsOY , 0), Quaternion.identity, GameObject.Find("Canvas").transform);
                        obj.GetComponent<Image>().color = panels[i].obj.GetComponent<SpriteRenderer>().color;
                        obj.transform.GetChild(0).GetComponent<Text>().text = panelGoal[i].ToString();
                        goalsList[i] = obj;
                        j++;
                    }
                }
            }
        }
        else
        {
            int j = 0;
            for (int i = 0; i < panelGoal.Length; i++)
            {
                if (panelGoal[i] > 0)
                {
                    GameObject obj = Instantiate(layout, new Vector3(-countGoals + 1 + j * 2, creatingPanel.countOfPanelsOY , 0), Quaternion.identity, GameObject.Find("Canvas").transform);
                    obj.GetComponent<Image>().color = panels[i].obj.GetComponent<SpriteRenderer>().color;
                    obj.transform.GetChild(0).GetComponent<Text>().text = panelGoal[i].ToString();
                    goalsList[i] = obj;
                    j++;
                }
            }
        }
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
        if (Direction() == new Vector3(0, 0, 1))
        {
            currentPanel.transform.position = new Vector3(ppX, ppY, 0);
        }
        else
        {
            if (hitPanel)
            {
                if ((hitPanel.transform.gameObject.GetComponent<Panels>().ID > 300) || (currentPanel.GetComponent<Panels>().ID > 300))
                {
                    currentPanel.transform.position = hitPanel.transform.gameObject.transform.position;
                    hitPanel.transform.gameObject.transform.position = new Vector3(ppX, ppY, 0);
                    UseBonus(currentPanel);
                    UseBonus(hitPanel.transform.gameObject);
                    countOfMoves--;
                    moves.text = "Moves: " + countOfMoves.ToString();
                    LosingControl();
                    matchFound = false;

                }
                else
                {
                    if (matchFound)
                    {
                        currentPanel.transform.position = hitPanel.transform.gameObject.transform.position;
                        hitPanel.transform.gameObject.transform.position = new Vector3(ppX, ppY, 0);
                        countOfMoves--;
                        moves.text = "Moves: " + countOfMoves.ToString();
                        LosingControl();
                        matchFound = false;
                    }
                    else
                    {
                        currentPanel.transform.position = new Vector3(ppX, ppY, 0);
                    }

                }
            }
            else
            {
                currentPanel.transform.position = new Vector3(ppX, ppY, 0);
            }
        }
    }

    public void LosingControl()
    {
        bool goals = true;
        for (int i = 0; i < panelGoal.Length; i++)
        {
            if (panelGoal[i] > 0)
            {
                goals = false;
                break;
            }
        }
        if (countOfMoves == 0 && !goals)
        {
            field.SetActive(false);
            loseScreen.SetActive(true);
            loseText.text = "You lose!\nYour Score: " + countOfScore.ToString();
            for (int i = 0; i < goalsList.Length; i++)
            {
                if (goalsList[i] != null)
                {
                    Destroy(goalsList[i]);
                }
            }
        }
    }

    public void UseBonus(GameObject obj)
    {
        if (obj.GetComponent<Panels>().ID > 300)
        {
            if (obj.GetComponent<Panels>().bonusName == "CubeBonus")
            {
                _ = new BonusControl<CubeBonus>(new CubeBonus(), obj);
            }
            else if (obj.GetComponent<Panels>().bonusName == "LineBonus4")
            {
                _ = new BonusControl<LineBonus4>(new LineBonus4(), obj);
            }
            else if (obj.GetComponent<Panels>().bonusName == "LineBonus5")
            {
                _ = new BonusControl<LineBonus5>(new LineBonus5(), obj);
            }
            else if (obj.GetComponent<Panels>().bonusName == "LinesOf3Panels")
            {
                _ = new BonusControl<LinesOf3Panels>(new LinesOf3Panels(), obj);
            }
        }
    }

    public void ClearPanelWithAI()
    {
        /*
        List<GameObject> allObj = AllPanels();
        int max = 0;
        int id = -1;
        for (int i = 0; i < panelGoal.Length; i++)
        {
            if (panelGoal[i] > max)
            {
                max = panelGoal[i];
                id = i;
            }
        }
        for (int i = 0; i < allObj.Count; i++)
        {
            if (allObj[i].GetComponent<Panels>().ID == id)
            {
                allObj[i].GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        if(currentPanel.GetComponent<Panels>().ID == 301 || currentPanel.GetComponent<Panels>().ID == id)
        {
            currentPanel.GetComponent<SpriteRenderer>().sprite = null;
        }
        else if (hitPanel.transform.gameObject.GetComponent<Panels>().ID == 301 || hitPanel.transform.gameObject.GetComponent<Panels>().ID == id)
        {
            hitPanel.transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }

        */
    }

    public void ClearPanelOnCube(GameObject obj, int width)
    {
        List<GameObject> panelList = new List<GameObject>() { currentPanel, hitPanel.transform.gameObject };

        for (int i = 0; i < width; i++)
        {
            RaycastHit2D[] panels = Physics2D.RaycastAll(
                new Vector3(obj.transform.position.x - (width / 2) + i, obj.transform.position.y - (width/2), 0),
                Vector2.up,
                width - 1,
                LayerMask.GetMask("Panel"));

            for (int j = 0; j < panels.Length; j++)
            {
                panelList.Add(panels[j].transform.gameObject);
            }
        }

        for (int i = 0; i < panelList.Count; i++)
        {
            panelList[i].GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    public void ClearLine(GameObject obj, bool verticalLine)
    {
        List<GameObject> tiles;
        if (verticalLine)
        {
            if (Direction() == new Vector3(0, 1))
            {
                tiles = Line(Vector2.up, currentPanel);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
                tiles = Line(Vector2.down, hitPanel.transform.gameObject);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
            }
            else if (Direction() == new Vector3(0, -1))
            {
                tiles = Line(Vector2.down, currentPanel);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
                tiles = Line(Vector2.up, hitPanel.transform.gameObject);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
            }
            else
            {
                tiles = Line(Vector2.up, obj);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
                tiles = Line(Vector2.down, obj);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
            }
        }
        else
        {
            if (Direction() == new Vector3(1, 0))
            {
                tiles = Line(Vector2.right, currentPanel);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
                tiles = Line(Vector2.left, hitPanel.transform.gameObject);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
            }
            else if (Direction() == new Vector3(-1, 0))
            {
                tiles = Line(Vector2.left, currentPanel);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
                tiles = Line(Vector2.right, hitPanel.transform.gameObject);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
            }
            else
            {
                tiles = Line(Vector2.left, obj);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
                tiles = Line(Vector2.right, obj);
                for (int i = 0; i < tiles.Count; i++)
                {
                    tiles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
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
                ////        Clear match on crossed lines         ////

                ClearMatchOnCrossedLines(new Vector2[2] { Vector2.left, Vector2.up }, currentPanel, hitPanel.transform.gameObject.transform.position);
                ClearMatchOnCrossedLines(new Vector2[2] { Vector2.left, Vector2.down }, currentPanel, hitPanel.transform.gameObject.transform.position);
                ClearMatchOnCrossedLines(new Vector2[2] { Vector2.up, Vector2.right }, currentPanel, hitPanel.transform.gameObject.transform.position);
                ClearMatchOnCrossedLines(new Vector2[2] { Vector2.down, Vector2.right }, currentPanel, hitPanel.transform.gameObject.transform.position);

                ClearMatchOnCrossedLines(new Vector2[2] { Vector2.left, Vector2.up }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                ClearMatchOnCrossedLines(new Vector2[2] { Vector2.left, Vector2.down }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                ClearMatchOnCrossedLines(new Vector2[2] { Vector2.up, Vector2.right }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                ClearMatchOnCrossedLines(new Vector2[2] { Vector2.down, Vector2.right }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));

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

    private void ClearMatchOnCrossedLines(Vector2[] paths, GameObject firstObj, Vector3 secObjPos)
    {
        List<GameObject> matchingTiles = new List<GameObject> { firstObj };
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i], firstObj, secObjPos));
        }
        if (matchingTiles.Count == 5)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
                countOfScore += (int)(scorePerPanel * 1.7f);
            }
            matchingTiles[UnityEngine.Random.Range(0, matchingTiles.Count)].GetComponent<Panels>().bonusName = "_LinesOf3Panels";
            Debug.Log("Clear Match On Crossed Lines");
            GameObject.Find("Score").GetComponent<Text>().text = "Score: " + countOfScore.ToString();
            matchFound = true;
        }
    }

    private void ClearMatchOnLine(Vector2[] paths, GameObject firstObj, Vector3 secObjPos)
    {
        List<GameObject> matchingTiles = new List<GameObject> { firstObj };
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i], firstObj, secObjPos));
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
                if (matchingTiles[1].transform.position.x - matchingTiles[2].transform.position.x == 0)
                {
                    verticalBonus = false;
                }
                else
                {
                    verticalBonus = true;
                }
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

    private void ClearMatchOnCub(GameObject Obj, Vector3 dir, Vector3 objPos)
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
                objList[UnityEngine.Random.Range(0, 4)].GetComponent<Panels>().bonusName = "_CubeBonus";
                for (int i = 0; i < 4; i++)
                {
                    objList[i].GetComponent<SpriteRenderer>().sprite = null;
                    countOfScore += (int)(scorePerPanel * 1.5f);
                }
                matchFound = true;
            }
        }
    }

    private List<GameObject> Line(Vector2 castDir, GameObject obj)
    {
        List<GameObject> tiles = new List<GameObject> { obj };
        RaycastHit2D hit = Physics2D.Raycast(obj.transform.position, castDir, 1f, LayerMask.GetMask("Panel"));
        if (hit.collider != null)
        {
            hit.transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }
        while (hit.collider != null)
        {
            tiles.Add(hit.collider.gameObject);
            hit.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            RaycastHit2D hit2 = Physics2D.Raycast(hit.transform.gameObject.transform.position, castDir, 1f, LayerMask.GetMask("Panel"));
            hit.transform.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            hit = hit2;
        }
        return tiles;

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

    private List<GameObject> AllPanels()
    {
        List<GameObject> allObj = new List<GameObject>();
        for (int i = 0; i < creatingPanel.countOfPanelsOX; i++)
        {
            RaycastHit2D[] panels = Physics2D.RaycastAll(new Vector3(creatingPanel.startPosition.x + i, -creatingPanel.startPosition.y - 1, 0), Vector2.up, 100f, LayerMask.GetMask("Panel"));
            for (int j = 0; j < panels.Length; j++)
            {
                allObj.Add(panels[j].transform.gameObject);
            }
        }
        return allObj;
    }

    public List<GameObject> SingleClolorPanels()
    {
        List<GameObject> allObj = AllPanels();
        List<GameObject> SingeClolorPanels = new List<GameObject>();
        int ID = -1;

        if (currentPanel.GetComponent<Panels>().ID > 300)
        {
            ID = hitPanel.transform.gameObject.GetComponent<Panels>().ID;
            allObj.Add(hitPanel.transform.gameObject);
        }
        else if (hitPanel.transform.gameObject.GetComponent<Panels>().ID > 300)
        {
            ID = currentPanel.GetComponent<Panels>().ID;
            allObj.Add(hitPanel.transform.gameObject);
        }

        for (int i = 0; i < allObj.Count; i++)
        {
            if (allObj[i].GetComponent<Panels>().ID == ID)
            {
                SingeClolorPanels.Add(allObj[i]);
            }
        }
        return SingeClolorPanels;
    }
}

[System.Serializable]
public struct Panel
{
    public GameObject obj;
    public int ID;
}
