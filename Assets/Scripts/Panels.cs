using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panels : MonoBehaviour
{
    public Controller Controller;
    CreatingPanels creatingPanel;

    public int health;
    public int ID;
    public float falling = 0;
    public bool deleteOY;
    bool inList = false;
    int posInList = -1;

    public string bonusName = "";
    bool fallen = false;

    JsonControl JC = new JsonControl();
    Save save;

    private void Awake()
    {
        Controller = GameObject.Find("Controller").GetComponent<Controller>();
        creatingPanel = GameObject.Find("Controller").GetComponent<CreatingPanels>();
    }

    private void FixedUpdate()
    {
        if (falling > 0)
        {
            if (!inList)
            {
                Controller.objInFalling.Add(gameObject);
                posInList = Controller.objInFalling.Count - 1;
                //Debug.Log(posInList);
                inList = true;
            }
            transform.position += new Vector3(0, -0.15f, 0);
            falling -= 0.15f;
            creatingPanel.isFalling = true;
            if (falling < 0)
            {
                transform.position = new Vector3(transform.position.x, Mathf.Floor(transform.position.y + 0.31f));
                if (inList)
                {
                    Controller.objInFalling.RemoveAt(Controller.objInFalling.Count - 1);
                    //Debug.Log(Controller.objInFalling.Count);
                    inList = false;
                }
                if (Controller.objInFalling.Count == 0)
                {
                    creatingPanel.isFalling = false;
                    creatingPanel.check = false;
                }
                fallen = true;
                falling = 0;
                Controller.matchFound = false;
            }
        }
        
        DeletePanel();
        if (fallen && !creatingPanel.isFalling)
        {
            Controller.AllMatches(true, gameObject);
            fallen = false;
            Debug.Log(gameObject);
        }
    }

    private void OnMouseDown()
    {
        if (!creatingPanel.isFalling)
        {
            Controller.currentPanel = gameObject;
            if (Controller.currentPanel != null)
            {
                Controller.ppX = transform.position.x;
                Controller.ppY = transform.position.y;
                Controller.clickPos = Input.mousePosition;
                Controller.currentPanel.GetComponent<BoxCollider2D>().enabled = false;
                Vector3 vec = Camera.main.ScreenToWorldPoint(Controller.clickPos) + new Vector3(0, 0, 8);
                Controller.HitMarker(vec, true);
                Controller.hold = true;
            }
        }
    }

    private void OnMouseUp()
    {
        if (Controller != null)
        {
            if (Controller.hold)
            {

                Controller.hitPanel = Physics2D.Raycast(new Vector3(Controller.ppX, Controller.ppY, 0), Controller.Direction(), 1f, LayerMask.GetMask("Panel"));
                if (Controller.hitPanel)
                    Controller.hitPanel.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false;

                
                Controller.AllMatches(false, null);
                Controller.Transposition();

                Controller.HitMarker(new Vector3(), false);
                if (Controller.hitPanel)
                    Controller.hitPanel.transform.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                Controller.currentPanel.GetComponent<BoxCollider2D>().enabled = true;
                Controller.hitPanel = new RaycastHit2D();
                Controller.hold = false;
                Controller.currentPanel = null;
                Controller.lockDirectX = false;
                Controller.lockDirectY = false;
            }
        }
    }

    private void DeletePanel()
    {
        if (gameObject.GetComponent<SpriteRenderer>().sprite == null)
        {
            for (int i = 0; i < Controller.objInFalling.Count; i++)
            {
                if (Controller.objInFalling[i] == gameObject)
                {
                    Controller.objInFalling.RemoveAt(i);
                    Debug.Log(Controller.objInFalling.Count);
                }
            }

            if (ID < 300)
            {
                if (Controller.panelGoal[ID] > 0)
                {
                    Controller.panelGoal[ID]--;
                    Controller.goalsList[ID].transform.GetChild(0).GetComponent<Text>().text = Controller.panelGoal[ID].ToString();
                    if (Controller.panelGoal[ID] <= 0)
                    {
                        bool goals = true;
                        for (int i = 0; i < Controller.panelGoal.Length; i++ )
                        {
                            if (Controller.panelGoal[i] > 0)
                            {
                                goals = false;
                                break;
                            }
                        }
                        if (goals)
                        {
                            JC = new JsonControl();
                            save = JC.LoadJson();
                            save.levels[PlayerPrefs.GetInt("Level number") - 1, 1] = Controller.countOfScore;
                            SetStars();
                            JC.SaveJson(save);

                            Controller.field.SetActive(false);
                            Controller.WinScreen.SetActive(true);
                            if (Controller.loseScreen.activeSelf)
                            {
                                Controller.loseScreen.SetActive(true);
                            }
                            Controller.winText.text = "Great!\nYour Score: " + Controller.countOfScore.ToString();
                            for (int i = 0; i < Controller.goalsList.Length; i++)
                            {
                                if (Controller.goalsList[i] != null)
                                {
                                    Destroy(Controller.goalsList[i]);
                                }
                            }
                        }
                    }
                }
            }

            creatingPanel.CheckAllField();
            //creatingPanel.FillingInEmptyFields(gameObject.transform.position.x);
            RaycastHit2D[] hits = Physics2D.RaycastAll(gameObject.transform.position, Vector2.up, 100.0F, LayerMask.GetMask("Panel"));
            if (hits != null)
            {
                for (int j = 0; j < hits.Length; j++)
                {
                    hits[j].transform.gameObject.GetComponent<Panels>().falling++;
                }

                int rand = -1;
                do
                {
                    rand = Random.Range(0, hits.Length);
                    if (hits[rand].transform.gameObject.GetComponent<SpriteRenderer>().sprite != null)
                    {
                        if (bonusName == "_CubeBonus")
                        {
                            _ = new BonusControl<CubeBonus>(hits[rand].transform.gameObject, new CubeBonus());
                        }
                        else if (bonusName == "_4LineBonus")
                        {
                            _ = new BonusControl<LineBonus4>(hits[rand].transform.gameObject, new LineBonus4());
                        }
                        else if (bonusName == "_5LineBonus")
                        {
                            _ = new BonusControl<LineBonus5>(hits[rand].transform.gameObject, new LineBonus5());
                        }
                        else if (bonusName == "_LinesOf3Panels")
                        {
                            _ = new BonusControl<LinesOf3Panels>(hits[rand].transform.gameObject, new LinesOf3Panels());
                        }

                    }
                } while (hits[rand].transform.gameObject.GetComponent<SpriteRenderer>().sprite == null);

                    Destroy(gameObject);
                creatingPanel.isFalling = false;
            }
        }
    }

    public void SetStars()
    {
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
    }
}
