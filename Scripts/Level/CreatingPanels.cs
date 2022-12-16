using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatingPanels : MonoBehaviour
{
    public GameObject field;
    public GameObject voidPanel;
    public Controller controller;

    public bool isFalling = false;
    public bool check = false;

    public int countOfPanelsOX;
    public int countOfPanelsOY;
    public Vector3 startPosition;

    ///   Creating a field (countOfPanelsOX - number of columns, countOfPanelsOY - number of rows)   ///

    public void CreateField()
    {
        Vector3 cp = startPosition;
        int rand = -1;
        int[] previousAbove = new int[countOfPanelsOX];
        int previousLeft = -1;

        for (int i = 0; i < controller.level.field.GetLength(0); i++)
        {
            for (int j = 0; j < controller.level.field.GetLength(1); j++)
            {
                if (controller.level.field[i,j] == 0)
                {
                    GameObject obj = Instantiate(voidPanel, new Vector3(cp.x + j, cp.y - i, 0), Quaternion.identity, field.transform);
                }
                else if (controller.level.field[i, j] == 1)
                {
                    while (rand == previousLeft || rand == previousAbove[j])
                    {
                        rand = Random.Range(0, 6);
                    }

                    if (rand != -1)
                    {
                        GameObject obj = Instantiate(controller.panels[rand].obj, new Vector3(cp.x + j, cp.y - i, 0), Quaternion.identity, field.transform);
                        obj.GetComponent<Panels>().ID = rand;
                        obj.GetComponent<Panels>().health = 1;
                        previousAbove[j] = obj.GetComponent<Panels>().ID;
                        previousLeft = obj.GetComponent<Panels>().ID;
                    }
                }
                else if (controller.level.field[i, j] == 2)
                {
                    while (rand == previousLeft || rand == previousAbove[j])
                    {
                        rand = Random.Range(0, 6);
                    }

                    if (rand != -1)
                    {
                        GameObject obj = Instantiate(controller.panels[rand].obj, new Vector3(cp.x + j, cp.y - i, 0), Quaternion.identity, field.transform);
                        obj.GetComponent<Panels>().ID = rand;
                        obj.GetComponent<Panels>().health = 2;
                        obj.GetComponent<SpriteRenderer>().color -= new Color32(40, 40, 40, 0);
                        previousAbove[j] = obj.GetComponent<Panels>().ID;
                        previousLeft = obj.GetComponent<Panels>().ID;
                    }
                }
            }
            previousLeft = -1;
            cp.x = startPosition.x;
        }

        List<GameObject> allObj = controller.AllPanels();
        rand = -1;
        Debug.Log(PlayerPrefs.GetInt("WinStreak"));
        switch (PlayerPrefs.GetInt("WinStreak"))
        {
            case 1:
                rand = Random.Range(0, allObj.Count);
                _ = new BonusControl<LineBonus4>(allObj[rand], new LineBonus4());
                do
                {
                    rand = Random.Range(0, allObj.Count);
                    if (allObj[rand].GetComponent<Panels>().ID < 300)
                    {
                        _ = new BonusControl<CubeBonus>(allObj[rand], new CubeBonus());
                        break;
                    }
                } while (allObj[rand].GetComponent<Panels>().ID > 300);
                break;
            case 2:

                rand = Random.Range(0, allObj.Count);
                _ = new BonusControl<LineBonus4>(allObj[rand], new LineBonus4());
                do
                {
                    rand = Random.Range(0, allObj.Count);
                    if (allObj[rand].GetComponent<Panels>().ID < 300)
                    {
                        _ = new BonusControl<CubeBonus>(allObj[rand], new CubeBonus());
                        break;
                    }
                } while (allObj[rand].GetComponent<Panels>().ID > 300);
                
                do
                {
                    rand = Random.Range(0, allObj.Count);
                    if (allObj[rand].GetComponent<Panels>().ID < 300)
                    {
                        _ = new BonusControl<LinesOf3Panels>(allObj[rand], new LinesOf3Panels());
                        break;
                    }
                } while (allObj[rand].GetComponent<Panels>().ID > 300);

                break;
            case 3:
                rand = Random.Range(0, allObj.Count);
                _ = new BonusControl<LineBonus4>(allObj[rand], new LineBonus4());
                do
                {
                    rand = Random.Range(0, allObj.Count);
                    if (allObj[rand].GetComponent<Panels>().ID < 300)
                    {
                        _ = new BonusControl<CubeBonus>(allObj[rand], new CubeBonus());
                        break;
                    }
                } while (allObj[rand].GetComponent<Panels>().ID > 300);
                
                do
                {
                    rand = Random.Range(0, allObj.Count);
                    if (allObj[rand].GetComponent<Panels>().ID < 300)
                    {
                        _ = new BonusControl<LinesOf3Panels>(allObj[rand], new LinesOf3Panels());
                        break;
                    }
                } while (allObj[rand].GetComponent<Panels>().ID > 300);
                
                do
                {
                    rand = Random.Range(0, allObj.Count);
                    if (allObj[rand].GetComponent<Panels>().ID < 300)
                    {
                        _ = new BonusControl<LineBonus5>(allObj[rand], new LineBonus5());
                        break;
                    }
                } while (allObj[rand].GetComponent<Panels>().ID > 300);
                break;
        }
        if (PlayerPrefs.GetInt("WinStreak") > 3)
        {
            rand = Random.Range(0, allObj.Count);
            _ = new BonusControl<LineBonus4>(allObj[rand], new LineBonus4());
            do
            {
                rand = Random.Range(0, allObj.Count);
                if (allObj[rand].GetComponent<Panels>().ID < 300)
                {
                    _ = new BonusControl<CubeBonus>(allObj[rand], new CubeBonus());
                    break;
                }
            } while (allObj[rand].GetComponent<Panels>().ID > 300);

            do
            {
                rand = Random.Range(0, allObj.Count);
                if (allObj[rand].GetComponent<Panels>().ID < 300)
                {
                    _ = new BonusControl<LinesOf3Panels>(allObj[rand], new LinesOf3Panels());
                    break;
                }
            } while (allObj[rand].GetComponent<Panels>().ID > 300);

            do
            {
                rand = Random.Range(0, allObj.Count);
                if (allObj[rand].GetComponent<Panels>().ID < 300)
                {
                    _ = new BonusControl<LineBonus5>(allObj[rand], new LineBonus5());
                    break;
                }
            } while (allObj[rand].GetComponent<Panels>().ID > 300);
        }
    }

    ///     creating a panel at a specific point    ///

    public void CreatePanel(int ID, Vector3 pos)
    {
        GameObject obj = Instantiate(controller.panels[ID].obj, pos, Quaternion.identity, field.transform);
        obj.GetComponent<Panels>().ID = ID;
    }

    ///     checking and populating a column with panels    ///

    public void FillingInEmptyFields(float posX)
    {
        int length = 0;
        RaycastHit2D[] panels = Physics2D.RaycastAll(new Vector3(posX, -startPosition.y - 1, 0), Vector2.up, 100f, LayerMask.GetMask("Panel"));
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].transform.gameObject.GetComponent<SpriteRenderer>().sprite != null)
            {
                length++;
            }
        }
        if (length < countOfPanelsOY)
        {
            for (int i = 0; i < (countOfPanelsOY - length); i++)
            {
               CreatePanel(Random.Range(0, 6), new Vector3(posX, startPosition.y + i + 1, 0));
            }
        }
    }

    public void CheckAllField()
    {
        if (check == false)
        {
            GameObject obj;
            int rand;
            for (int i = 0; i < countOfPanelsOX; i++)
            {
                int length = 0;
                RaycastHit2D[] panels = Physics2D.RaycastAll(new Vector3(startPosition.x + i, -startPosition.y - 1, 0), Vector2.up, 100f, LayerMask.GetMask("Panel"));
                for (int j = 0; j < panels.Length; j++)
                {
                    if (panels[j].transform.gameObject.GetComponent<SpriteRenderer>().sprite != null || panels[j].transform.gameObject.GetComponent<Panels>().health == 2)
                    {
                        length++;
                    }
                }
                RaycastHit2D[] voidPanels = Physics2D.RaycastAll(new Vector3(startPosition.x + i, -startPosition.y - 1, 0), Vector2.up, 100f, LayerMask.GetMask("Void"));
                length += voidPanels.Length;

                if (length < countOfPanelsOY)
                {
                    for (int j = 0; j < (countOfPanelsOY - length); j++)
                    {
                        rand = Random.Range(0, 6);
                        obj = Instantiate(controller.panels[rand].obj, new Vector3(panels[j].transform.position.x, startPosition.y + j + 1, 0), Quaternion.identity, field.transform);
                        obj.GetComponent<Panels>().health = 1;
                        obj.GetComponent<Panels>().ID = rand;
                    }
                }
            }
            check = true;
        }
    }
}
