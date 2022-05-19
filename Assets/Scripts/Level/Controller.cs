using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public List<Panel> panels = new List<Panel>();

    public float ppX;
    public float ppY;
    public int[] panelGoal;
    public int countOfMoves;
    public int countOfScore = 0;
    public int scorePerPanel = 10;

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
    public List<GameObject> objInFalling = new List<GameObject>();


    public bool hold = false;
    public bool lockDirectX = false;
    public bool lockDirectY = false;
    public bool matchFound = false;
    public bool verticalBonus = false;
    bool setStar = false;

    public Text winText;
    public Text loseText;
    public Text moves;
    public Vector2 clickPos;
    public RaycastHit2D hitPanel;

    public Sprite rocket;
    public Sprite lighting;
    public Sprite explosiveBarrel;
    public Sprite eraser;

    public Level level = new Level();

    JsonControl JC;
    Save save;

    private void Awake()
    {
        JC = new JsonControl();
        save = new Save();
        level = JC.LoadFromRecurces(PlayerPrefs.GetInt("Level number").ToString());
        Application.targetFrameRate = 60;
        
        creatingPanel.CreateField();
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

        for (int i = 0; i < panelGoal.Length; i++)
        {
            panelGoal[i] = level.goals[0, i];
        }
        
        countOfMoves = level.countOfMoves;

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
                        GameObject obj = Instantiate(layout, new Vector3(0, creatingPanel.countOfPanelsOY - 1, 0), Quaternion.identity, GameObject.Find("GameField").transform);
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
                        GameObject obj = Instantiate(layout, new Vector3(-countGoals + 1 + j * 2, creatingPanel.countOfPanelsOY - 1 , 0), Quaternion.identity, GameObject.Find("GameField").transform);
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
                    GameObject obj = Instantiate(layout, new Vector3(-countGoals + 1 + j * 2, creatingPanel.countOfPanelsOY - 1, 0), Quaternion.identity, GameObject.Find("GameField").transform);
                    obj.GetComponent<Image>().color = panels[i].obj.GetComponent<SpriteRenderer>().color;
                    obj.transform.GetChild(0).GetComponent<Text>().text = panelGoal[i].ToString();
                    goalsList[i] = obj;
                    j++;
                }
            }
        }
    }

    public void DeployingTheBombWithAI(int width)
    {
        int countOfPanels = 0;
        int max = 0;
        int id = -1;
        GameObject centre = new GameObject();
        GameObject curentCentre = new GameObject();

        for (int i = 0; i < panelGoal.Length; i++)
        {
            if (panelGoal[i] > max)
            {
                max = panelGoal[i];
                id = i;
            }
        }
        max = 0;

        for (int i = 0; i < creatingPanel.countOfPanelsOY - width; i++)
        {
            for (int j = 0; j < creatingPanel.countOfPanelsOX - width; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    RaycastHit2D[] panels = Physics2D.RaycastAll(
                                                        new Vector3(creatingPanel.startPosition.x + j + k, creatingPanel.startPosition.y - width - i, 0),
                                                        Vector2.up,
                                                        width - 1,
                                                        LayerMask.GetMask("Panel"));
                    if (k == 2)
                    {
                        centre = panels[2].transform.gameObject;
                    }
                    for (int l = 0; l < panels.Length; l++) 
                    {
                        if (panels[l].transform.gameObject.GetComponent<Panels>().ID == id)
                        {
                            countOfPanels++;
                        }
                    }

                }
                if (countOfPanels > max)
                {
                    max = countOfPanels;
                    curentCentre = centre;
                }
                countOfPanels = 0;
            }
        }
        _ = new BonusControl<LinesOf3Panels>(curentCentre, new LinesOf3Panels());

        currentPanel.GetComponent<SpriteRenderer>().sprite = null;
        hitPanel.transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    public void ClearLineWithAI()
    {
        int max = 0;
        int countOfPanels = 0; 
        int id = -1;
        int line = 0;
        int currentLine = 0;
        bool vertical = true;

        for (int i = 0; i < panelGoal.Length; i++)
        {
            if (panelGoal[i] > max)
            {
                max = panelGoal[i];
                id = i;
            }
        }
        max = 0;

        for (int i = 0; i < creatingPanel.countOfPanelsOX; i++)
        {
            line++;
            RaycastHit2D[] panels = Physics2D.RaycastAll(new Vector3(creatingPanel.startPosition.x + i, -creatingPanel.startPosition.y, 0), Vector2.up, 100f, LayerMask.GetMask("Panel"));
            for (int j = 0; j < panels.Length; j++)
            {
                if (panels[j].transform.gameObject.GetComponent<Panels>().ID == id)
                {
                    countOfPanels++;
                }
                if (countOfPanels > max)
                {
                    max = countOfPanels;
                    currentLine = line;
                }
            }
            countOfPanels = 0;
        }
        line = 0;
        for (int i = 0; i < creatingPanel.countOfPanelsOY; i++)
        {
            line++;
            RaycastHit2D[] panels = Physics2D.RaycastAll(new Vector3(creatingPanel.startPosition.x, creatingPanel.startPosition.y - i, 0), Vector2.right, 100f, LayerMask.GetMask("Panel"));
            for (int j = 0; j < panels.Length; j++)
            {
                if (panels[j].transform.gameObject.GetComponent<Panels>().ID == id)
                {
                    countOfPanels++;
                }
                if (countOfPanels > max)
                {
                    max = countOfPanels;
                    vertical = false;
                    currentLine = line;
                }
            }
            countOfPanels = 0;
        }

        if (vertical)
        {
            RaycastHit2D[] panels = Physics2D.RaycastAll(new Vector3(creatingPanel.startPosition.x + currentLine - 1, -creatingPanel.startPosition.y, 0), Vector2.up, 100f, LayerMask.GetMask("Panel"));
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
                countOfScore += scorePerPanel;
            }
        }
        else
        {
            RaycastHit2D[] panels = Physics2D.RaycastAll(new Vector3(creatingPanel.startPosition.x, creatingPanel.startPosition.y - currentLine + 1, 0), Vector2.right, 100f, LayerMask.GetMask("Panel"));
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
                countOfScore += scorePerPanel;
            }
        }
        currentPanel.GetComponent<SpriteRenderer>().sprite = null;
        hitPanel.transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    public void ThreeRandomBonuses()
    {
        GameObject[] bonuses = new GameObject[3];
        List<GameObject> allObj = AllPanels();
        GameObject obj = new GameObject();
        int rand = -1;
        for (int i = 0; i < bonuses.Length; i++)
        {
            rand = UnityEngine.Random.Range(1, 5);
            obj = allObj[UnityEngine.Random.Range(0, allObj.Count)];
            if (obj != bonuses[0] && obj != bonuses[1] && obj != bonuses[2])
            {
                switch(rand)
                {
                    case 1:
                        _ = new BonusControl<LineBonus4>(obj, new LineBonus4());
                        break;
                    case 2:
                        _ = new BonusControl<CubeBonus>(obj, new CubeBonus());
                        break;
                    case 3:
                        _ = new BonusControl<LinesOf3Panels>(obj, new LinesOf3Panels());
                        break;
                    case 4:
                        _ = new BonusControl<LineBonus5>(obj, new LineBonus5());
                        break;
                }
                bonuses[i] = obj;
            }
            else
            {
                i--;
            }
        }

        for (int i = 0; i < bonuses.Length; i++)
        {
            if (bonuses[i] != null)
            {
                switch (bonuses[i].GetComponent<Panels>().ID)
                {
                    case 301:
                        if (bonuses[i].GetComponent<Panels>().deleteOY == true)
                        {
                            ClearLine(bonuses[i], true);
                        }
                        else if (bonuses[i].GetComponent<Panels>().deleteOY == false)
                        {
                            ClearLine(bonuses[i], false);
                        }
                        break;
                    case 302:
                        ClearPanelWithAI(bonuses[i]);
                        break;
                    case 303:
                        ClearPanelOnCube(bonuses[i], 5);
                        break;
                    case 304:
                        List<GameObject> panels = SingleClolorPanels();
                        bonuses[i].GetComponent<SpriteRenderer>().sprite = null;
                        for (int j = 0; j < panels.Count; j++)
                        {
                            panels[j].GetComponent<SpriteRenderer>().sprite = null;
                        }
                        break;
                }
            }
        }

        currentPanel.GetComponent<SpriteRenderer>().sprite = null;
        hitPanel.transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    public void FillingBonuses301(float perCent)
    {
        List<GameObject> allObj = AllPanels();
        List<GameObject> bonuses = new List<GameObject>();
        int countOfBonuses = 0;
        int maxBonuses = (int)(creatingPanel.countOfPanelsOX * creatingPanel.countOfPanelsOY * perCent * 0.01);
        Debug.Log(maxBonuses);
        for (int i = 0; i < allObj.Count; i++)
        {
            if (countOfBonuses < maxBonuses)
            {
                if (UnityEngine.Random.Range(1, 11) <= perCent / 10)
                {
                    _ = new BonusControl<LineBonus4>(allObj[i], new LineBonus4());
                    allObj[i].GetComponent<Panels>().deleteOY = UnityEngine.Random.Range(1, 3) == 1;
                    countOfBonuses++;
                    Debug.Log(countOfBonuses);
                    bonuses.Add(allObj[i]);
                }
            }
            else break;
        }
        
        for (int i = 0; i < bonuses.Count; i++)
        {
            if(bonuses[i].GetComponent<SpriteRenderer>().sprite != null)
            {
                if (bonuses[i].GetComponent<Panels>().deleteOY == true)
                {
                    ClearLine(bonuses[i], true);
                }
                else if (bonuses[i].GetComponent<Panels>().deleteOY == false)
                {
                    ClearLine(bonuses[i], false);
                }
            }
        }
        currentPanel.GetComponent<SpriteRenderer>().sprite = null;
        hitPanel.transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    public void FillingBonuses302(float perCent)
    {
        List<GameObject> allObj = AllPanels();
        List<GameObject> bonuses = new List<GameObject>();
        int countOfBonuses = 0;
        int maxBonuses = (int)(creatingPanel.countOfPanelsOX * creatingPanel.countOfPanelsOY * perCent * 0.01f);
        Debug.Log(maxBonuses);
        for (int i = 0; i < allObj.Count; i++)
        {
            if (countOfBonuses < maxBonuses)
            {
                if (UnityEngine.Random.Range(1, 11) <= perCent / 10)
                {
                    _ = new BonusControl<CubeBonus>(allObj[i], new CubeBonus());
                    allObj[i].GetComponent<Panels>().deleteOY = UnityEngine.Random.Range(1, 3) == 1;
                    countOfBonuses++;
                    Debug.Log(countOfBonuses);
                    bonuses.Add(allObj[i]);
                }
            }
            else break;
        }

        for (int i = 0; i < bonuses.Count; i++)
        {
            if(bonuses[i].GetComponent<SpriteRenderer>().sprite != null)
            {
                ClearPanelWithAI(bonuses[i]);
            }
                
        }
        currentPanel.GetComponent<SpriteRenderer>().sprite = null;
        hitPanel.transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    public void FillingBonuses303(float perCent)
    {
        List<GameObject> allObj = AllPanels();
        List<GameObject> bonuses = new List<GameObject>();
        int countOfBonuses = 0;
        int maxBonuses = (int)(creatingPanel.countOfPanelsOX * creatingPanel.countOfPanelsOY * perCent * 0.01f);
        Debug.Log(maxBonuses);
        int rand;
        for (int i = 0; i < allObj.Count; i++)
        {
            if (countOfBonuses < maxBonuses)
            {
                rand = UnityEngine.Random.Range(1, 11);
                if (rand <= perCent / 10)
                {
                    _ = new BonusControl<LinesOf3Panels>(allObj[i], new LinesOf3Panels());
                    allObj[i].GetComponent<Panels>().deleteOY = UnityEngine.Random.Range(1, 3) == 1;
                    countOfBonuses++;
                    Debug.Log(countOfBonuses);
                    bonuses.Add(allObj[i]);
                }
            }
            else break;
        }
        currentPanel.GetComponent<SpriteRenderer>().sprite = null;
        hitPanel.transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    public void DestroyAll()
    {
        List<GameObject> allObj = AllPanels();
        allObj.Add(currentPanel);
        allObj.Add(hitPanel.transform.gameObject);
        for (int i = 0; i < allObj.Count; i++)
        {
            allObj[i].GetComponent<SpriteRenderer>().sprite = null;
            countOfScore += scorePerPanel;
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
            if (currentPanel.GetComponent<Panels>().ID > 300)
            {
                UsingBonus(currentPanel);
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
        else
        {
            if (hitPanel)
            {
                if ((hitPanel.transform.gameObject.GetComponent<Panels>().ID > 300) && (currentPanel.GetComponent<Panels>().ID > 300))
                {
                    currentPanel.transform.position = hitPanel.transform.gameObject.transform.position;
                    hitPanel.transform.gameObject.transform.position = new Vector3(ppX, ppY, 0);
                    UsingDoubleBonus();
                    countOfMoves--;
                    moves.text = "Moves: " + countOfMoves.ToString();
                    LosingControl();
                    matchFound = false;
                }
                else if ((hitPanel.transform.gameObject.GetComponent<Panels>().ID > 300) || (currentPanel.GetComponent<Panels>().ID > 300))
                {
                    currentPanel.transform.position = hitPanel.transform.gameObject.transform.position;
                    hitPanel.transform.gameObject.transform.position = new Vector3(ppX, ppY, 0);
                    UsingBonus(currentPanel);
                    UsingBonus(hitPanel.transform.gameObject);
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
            DateTime time = JsonConvert.DeserializeObject<DateTime>(PlayerPrefs.GetString("timeToAddLife"));

            if (DateTime.Now < time)
            {
                PlayerPrefs.SetString("timeToAddLife", JsonConvert.SerializeObject(time.AddMinutes(24)));
            }
            else
            {
                PlayerPrefs.SetString("timeToAddLife", JsonConvert.SerializeObject(DateTime.Now.AddMinutes(24)));
            }

            PlayerPrefs.SetInt("WinStreak", 0);
            if (10 - Mathf.Ceil((float)(time - DateTime.Now).TotalMinutes / 24) <= 0)
            {
                loseScreen.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
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

    void UsingBonus(GameObject obj)
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

    void UsingDoubleBonus()
    {
        if(currentPanel.GetComponent<Panels>().bonusName == "LineBonus4" || hitPanel.transform.gameObject.GetComponent<Panels>().bonusName == "LineBonus4")
        {
            if (currentPanel.GetComponent<Panels>().bonusName == "LineBonus4")
            {
                _ = new BonusControl<LineBonus4>(new LineBonus4(), currentPanel);
            }
            else if (hitPanel.transform.gameObject.GetComponent<Panels>().bonusName == "LineBonus4")
            {
                _ = new BonusControl<LineBonus4>(new LineBonus4(), hitPanel.transform.gameObject);
            }
        }
        else if (currentPanel.GetComponent<Panels>().bonusName == "CubeBonus" || hitPanel.transform.gameObject.GetComponent<Panels>().bonusName == "CubeBonus")
        {
            if (currentPanel.GetComponent<Panels>().bonusName == "CubeBonus")
            {
                _ = new BonusControl<CubeBonus>(new CubeBonus(), currentPanel);
            }
            else if (hitPanel.transform.gameObject.GetComponent<Panels>().bonusName == "CubeBonus")
            {
                _ = new BonusControl<CubeBonus>(new CubeBonus(), hitPanel.transform.gameObject);
            }
        }
        else if (currentPanel.GetComponent<Panels>().bonusName == "LinesOf3Panels" || hitPanel.transform.gameObject.GetComponent<Panels>().bonusName == "LinesOf3Panels")
        {
            if (currentPanel.GetComponent<Panels>().bonusName == "LinesOf3Panels")
            {
                _ = new BonusControl<LinesOf3Panels>(new LinesOf3Panels(), currentPanel);
            }
            else if (hitPanel.transform.gameObject.GetComponent<Panels>().bonusName == "LinesOf3Panels")
            {
                _ = new BonusControl<LinesOf3Panels>(new LinesOf3Panels(), hitPanel.transform.gameObject);
            }
        }
        else if (currentPanel.GetComponent<Panels>().bonusName == "LineBonus5" || hitPanel.transform.gameObject.GetComponent<Panels>().bonusName == "LineBonus5")
        {
            if (currentPanel.GetComponent<Panels>().bonusName == "LineBonus5")
            {
                _ = new BonusControl<LineBonus5>(new LineBonus5(), currentPanel);
            }
            else if (hitPanel.transform.gameObject.GetComponent<Panels>().bonusName == "LineBonus5")
            {
                _ = new BonusControl<LineBonus5>(new LineBonus5(), hitPanel.transform.gameObject);
            }
        }
    }

    void ClearPanelONDirect(GameObject obj, Vector3 dir)
    {
        if (Physics2D.Raycast(new Vector3(obj.transform.position.x, obj.transform.position.y, 0), dir, 1f, LayerMask.GetMask("Panel")))
        {
            Physics2D.Raycast(new Vector3(obj.transform.position.x, obj.transform.position.y, 0), dir, 1f, LayerMask.GetMask("Panel")).transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
            countOfScore += scorePerPanel;
        }
    }

    public void ClearPanelWithAI(GameObject obj)
    {
        int minus = 1;
        if (hitPanel)
        {
            if (obj == currentPanel)
            {
                hitPanel.transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
                minus = -1;
            }
            else
            {
                currentPanel.GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        countOfScore += scorePerPanel;
        obj.GetComponent<BoxCollider2D>().enabled = false;
        if (Direction() == new Vector3(0, 1))
        {
            ClearPanelONDirect(obj, new Vector3(1, 0, 0));
            ClearPanelONDirect(obj, new Vector3(-1, 0, 0));
            ClearPanelONDirect(obj, new Vector3(0, -1 * minus, 0));
        }
        else if (Direction() == new Vector3(0, -1))
        {
            ClearPanelONDirect(obj, new Vector3(1, 0, 0));
            ClearPanelONDirect(obj, new Vector3(-1, 0, 0));
            ClearPanelONDirect(obj, new Vector3(0, 1 * minus, 0));
        }
        else if (Direction() == new Vector3(1, 0))
        {
            ClearPanelONDirect(obj, new Vector3(-1 * minus, 0, 0));
            ClearPanelONDirect(obj, new Vector3(0, -1, 0));
            ClearPanelONDirect(obj, new Vector3(0, 1, 0));
        }
        else if (Direction() == new Vector3(-1, 0))
        {
            ClearPanelONDirect(obj, new Vector3(1 * minus, 0, 0));
            ClearPanelONDirect(obj, new Vector3(0, -1, 0));
            ClearPanelONDirect(obj, new Vector3(0, 1, 0));
        }
        else if (Direction() == new Vector3(0, 0, 1))
        {
            ClearPanelONDirect(obj, new Vector3(-1, 0, 0));
            ClearPanelONDirect(obj, new Vector3(1, 0, 0));
            ClearPanelONDirect(obj, new Vector3(0, -1, 0));
            ClearPanelONDirect(obj, new Vector3(0, 1, 0));
        }
        obj.GetComponent<BoxCollider2D>().enabled = true;
        obj.GetComponent<SpriteRenderer>().sprite = null;
    }

    public void ClearPanelOnCube(GameObject obj, int width)
    {
        List<GameObject> panelList = new List<GameObject>() { currentPanel };
        if (hitPanel)
        {
            panelList.Add(hitPanel.transform.gameObject);
        }

        for (int i = 0; i < width; i++)
        {
            RaycastHit2D[] panels = Physics2D.RaycastAll(
                new Vector3(obj.transform.position.x - (width / 2) + i, obj.transform.position.y - (width / 2), 0),
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
            countOfScore += scorePerPanel;
        }
    }

    public void ClearLine(GameObject obj, bool verticalLine)
    {
        List<GameObject> tiles = new List<GameObject>();
        if (verticalLine)
        {
            RaycastHit2D[] panels = Physics2D.RaycastAll(new Vector3(obj.transform.position.x, -creatingPanel.startPosition.y, 0), Vector2.up, 100f, LayerMask.GetMask("Panel"));
            for (int i = 0; i < panels.Length; i++)
            {
                tiles.Add(panels[i].transform.gameObject);
            }
            if (Direction() == new Vector3(0, 1) || Direction() == new Vector3(0, -1))
            {
                if (obj == currentPanel)
                {
                    tiles.Add(hitPanel.transform.gameObject);
                }
                else
                    tiles.Add(currentPanel);
            }
        }
        else
        {
            RaycastHit2D[] panels = Physics2D.RaycastAll(new Vector3(creatingPanel.startPosition.x, obj.transform.position.y, 0), Vector2.right, 100f, LayerMask.GetMask("Panel"));
            for (int i = 0; i < panels.Length; i++)
            {
                tiles.Add(panels[i].transform.gameObject);
            }
            if (Direction() == new Vector3(1, 0) || Direction() == new Vector3(-1, 0))
            {
                if (obj == currentPanel)
                {
                    tiles.Add(hitPanel.transform.gameObject);
                }
                else
                    tiles.Add(currentPanel);
            }

        }
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].GetComponent<SpriteRenderer>().sprite = null;
            countOfScore += scorePerPanel;
        }
        obj.GetComponent<SpriteRenderer>().sprite = null;
    }

                                ///     General method for finding matches     ///

    public void AllMatches(bool oneObj, GameObject obj)
    {
        if (oneObj)
        {
            obj.GetComponent<BoxCollider2D>().enabled = false;
            ClearMatchOnCrossedLines(new Vector2[2] { Vector2.left, Vector2.up }, obj, obj.transform.position);
            ClearMatchOnCrossedLines(new Vector2[2] { Vector2.left, Vector2.down }, obj, obj.transform.position);
            ClearMatchOnCrossedLines(new Vector2[2] { Vector2.up, Vector2.right }, obj, obj.transform.position);
            ClearMatchOnCrossedLines(new Vector2[2] { Vector2.down, Vector2.right }, obj, obj.transform.position);

            ClearMatchOnLine(new Vector2[2] { Vector2.left, Vector2.right }, obj, obj.transform.position);
            ClearMatchOnLine(new Vector2[2] { Vector2.up, Vector2.down }, obj, obj.transform.position);

            ClearMatchOnCub(obj, new Vector3(1, 1, 0), obj.transform.position);
            ClearMatchOnCub(obj, new Vector3(-1, 1, 0), obj.transform.position);
            ClearMatchOnCub(obj, new Vector3(1, -1, 0), obj.transform.position);
            ClearMatchOnCub(obj, new Vector3(-1, -1, 0), obj.transform.position);
            obj.GetComponent<BoxCollider2D>().enabled = true;
            //creatingPanel.CheckAllField();
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

                if (matchFound)
                {
                    matchFound = false;
                    ClearMatchOnCrossedLines(new Vector2[2] { Vector2.left, Vector2.up }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                    ClearMatchOnCrossedLines(new Vector2[2] { Vector2.left, Vector2.down }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                    ClearMatchOnCrossedLines(new Vector2[2] { Vector2.up, Vector2.right }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                    ClearMatchOnCrossedLines(new Vector2[2] { Vector2.down, Vector2.right }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                    matchFound = true;
                }
                else
                {
                    ClearMatchOnCrossedLines(new Vector2[2] { Vector2.left, Vector2.up }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                    ClearMatchOnCrossedLines(new Vector2[2] { Vector2.left, Vector2.down }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                    ClearMatchOnCrossedLines(new Vector2[2] { Vector2.up, Vector2.right }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                    ClearMatchOnCrossedLines(new Vector2[2] { Vector2.down, Vector2.right }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                }

                ////        Clear match on cube         ////

                ClearMatchOnCub(currentPanel, new Vector3(1, 1, 0), hitPanel.transform.position);
                ClearMatchOnCub(currentPanel, new Vector3(-1, 1, 0), hitPanel.transform.position);
                ClearMatchOnCub(currentPanel, new Vector3(1, -1, 0), hitPanel.transform.position);
                ClearMatchOnCub(currentPanel, new Vector3(-1, -1, 0), hitPanel.transform.position);

                if (matchFound)
                {
                    matchFound = false;
                    ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(1, 1, 0), new Vector3(ppX, ppY, 0));
                    ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(-1, 1, 0), new Vector3(ppX, ppY, 0));
                    ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(1, -1, 0), new Vector3(ppX, ppY, 0));
                    ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(-1, -1, 0), new Vector3(ppX, ppY, 0));
                    matchFound = true;
                }
                else
                {
                    ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(1, 1, 0), new Vector3(ppX, ppY, 0));
                    ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(-1, 1, 0), new Vector3(ppX, ppY, 0));
                    ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(1, -1, 0), new Vector3(ppX, ppY, 0));
                    ClearMatchOnCub(hitPanel.transform.gameObject, new Vector3(-1, -1, 0), new Vector3(ppX, ppY, 0));
                }

                ////        Clear match on Axes         ////

                ClearMatchOnLine(new Vector2[2] { Vector2.left, Vector2.right }, currentPanel, hitPanel.transform.gameObject.transform.position);
                ClearMatchOnLine(new Vector2[2] { Vector2.up, Vector2.down }, currentPanel, hitPanel.transform.gameObject.transform.position);

                if (matchFound)
                {
                    matchFound = false;
                    ClearMatchOnLine(new Vector2[2] { Vector2.left, Vector2.right }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                    ClearMatchOnLine(new Vector2[2] { Vector2.up, Vector2.down }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                    matchFound = true;
                }
                else
                {
                    ClearMatchOnLine(new Vector2[2] { Vector2.left, Vector2.right }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                    ClearMatchOnLine(new Vector2[2] { Vector2.up, Vector2.down }, hitPanel.transform.gameObject, new Vector3(ppX, ppY, 0));
                }
                //creatingPanel.CheckAllField();
            }
        }
    }

    private void ClearMatchOnCrossedLines(Vector2[] paths, GameObject firstObj, Vector3 secObjPos)
    {
        if (!matchFound)
        {
            List<GameObject> matchingTiles = new List<GameObject> { firstObj };
            for (int i = 0; i < paths.Length; i++)
            {
                matchingTiles.AddRange(FindMatch(paths[i], firstObj, secObjPos));
            }
            if (matchingTiles.Count == 5)
            {
                matchingTiles[UnityEngine.Random.Range(0, matchingTiles.Count)].GetComponent<Panels>().bonusName = "_LinesOf3Panels";
                for (int i = 0; i < matchingTiles.Count; i++)
                {
                    matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
                    countOfScore += (int)(scorePerPanel * 1.7f);
                }
                GameObject.Find("Score").GetComponent<Text>().text = "Score: " + countOfScore.ToString();
                matchFound = true;
            }
        }
    }

    private void ClearMatchOnLine(Vector2[] paths, GameObject firstObj, Vector3 secObjPos)
    {
        if (!matchFound)
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
                    matchingTiles[UnityEngine.Random.Range(0, matchingTiles.Count)].GetComponent<Panels>().bonusName = "_4LineBonus";
                    for (int i = 0; i < matchingTiles.Count; i++)
                    {
                        matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
                        countOfScore += (int)(scorePerPanel * 1.3f);
                    }
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
                    matchingTiles[UnityEngine.Random.Range(0, matchingTiles.Count)].GetComponent<Panels>().bonusName = "_5LineBonus";
                    for (int i = 0; i < matchingTiles.Count; i++)
                    {
                        matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
                        countOfScore += scorePerPanel * 2;
                    }
                }
                GameObject.Find("Score").GetComponent<Text>().text = "Score: " + countOfScore.ToString();
                matchFound = true;
            }
        }
    }

    private void ClearMatchOnCub(GameObject Obj, Vector3 dir, Vector3 objPos)
    {
        if (!matchFound)
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
    }

    public void Win()
    {
        PlayerPrefs.SetInt("WinStreak", PlayerPrefs.GetInt("WinStreak") + 1);

        save = JC.LoadJson();
        int curentStars = save.levels[PlayerPrefs.GetInt("Level number") - 1, 1];
        save.levels[PlayerPrefs.GetInt("Level number") - 1, 1] = countOfScore;
        if (save.levels[PlayerPrefs.GetInt("Level number") - 1, 1] > (save.levels[PlayerPrefs.GetInt("Level number") - 1, 2]))
        {
            save.levels[PlayerPrefs.GetInt("Level number") - 1, 0] = 3;
        }
        else if (save.levels[PlayerPrefs.GetInt("Level number") - 1, 1] > ((save.levels[PlayerPrefs.GetInt("Level number") - 1, 2] * 2) / 3))
        {
            save.levels[PlayerPrefs.GetInt("Level number") - 1, 0] = 2;
        }
        else if (save.levels[PlayerPrefs.GetInt("Level number") - 1, 1] > (save.levels[PlayerPrefs.GetInt("Level number") - 1, 2] / 3))
        {
            save.levels[PlayerPrefs.GetInt("Level number") - 1, 0] = 1;    
        }

        if ((curentStars < save.levels[PlayerPrefs.GetInt("Level number") - 1, 1]) && !setStar)
        {
            save.stars += save.levels[PlayerPrefs.GetInt("Level number") - 1, 0] - curentStars;
            setStar = true;
        }

        JC.SaveJson(save);
        field.SetActive(false);
        WinScreen.SetActive(true);
        if (loseScreen.activeSelf)
        {
            loseScreen.SetActive(false);
        }

        winText.text = "Great!\nYour Score: " + countOfScore.ToString();
        for (int i = 0; i < goalsList.Length; i++)
        {
            if (goalsList[i] != null)
            {
                Destroy(goalsList[i]);
            }
        }
    }

    ///      Find Matches and delete sprite     ///
    
    public List<GameObject> SingleClolorPanels()
    {
        List<GameObject> allObj = AllPanels();
        List<GameObject> SingeClolorPanels = new List<GameObject>();
        int ID = -1;
        if (hitPanel)
        {
            if (currentPanel.GetComponent<Panels>().ID > 300 && hitPanel.transform.gameObject.GetComponent<Panels>().ID > 300)
            {
                ID = UnityEngine.Random.Range(0, 6);
            }
            else if (currentPanel.GetComponent<Panels>().ID > 300)
            {
                ID = hitPanel.transform.gameObject.GetComponent<Panels>().ID;
                allObj.Add(hitPanel.transform.gameObject);
            }
            else if (hitPanel.transform.gameObject.GetComponent<Panels>().ID > 300)
            {
                ID = currentPanel.GetComponent<Panels>().ID;
                allObj.Add(hitPanel.transform.gameObject);
            }
        }
        else
        {
            ID = UnityEngine.Random.Range(0, 6);
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

    public List<GameObject> AllPanels()
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
}

[System.Serializable]
public struct Panel
{
    public GameObject obj;
    public int ID;
}
