using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatingPanels : MonoBehaviour
{
    public GameObject field;
    public Controller controller;

    public bool isFalling = false;

    public int countOfPanelsOX;
    public int countOfPanelsOY;
    public Vector3 startPosition;

    ///   Creating a field (countOfPanelsOX - number of columns, countOfPanelsOY - number of rows)   ///

    public void CreateField(int col, int row)
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

                if (rand != -1)
                {
                    GameObject obj = Instantiate(controller.panels[rand].obj, new Vector3(cp.x + j, cp.y - i, 0), Quaternion.identity, field.transform);
                    obj.GetComponent<Panels>().ID = rand;
                    previousAbove[j] = obj.GetComponent<Panels>().ID;
                    previousLeft = obj.GetComponent<Panels>().ID;
                }
            }
            previousLeft = -1;
            cp.x = startPosition.x;
        }
    }

    ///     creating a panel at a specific point     ///

    public void CreatePanel(int ID, Vector3 pos)
    {
        GameObject obj = Instantiate(controller.panels[ID].obj, pos, Quaternion.identity, field.transform);
        obj.GetComponent<Panels>().ID = ID;
    }

    ///     checking and populating a column with panels     ///

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
}
