using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusControl<T> where T : IBonus
{
    public BonusControl(T value, GameObject obj)
    {
        value.BonusEffect(obj);
    }

    public BonusControl(GameObject gameObject, T value) 
    {
        value.GivingBonus(gameObject);
    }
}

public interface IBonus
{
    public void GivingBonus(GameObject Obj);

    public void BonusEffect(GameObject Obj);
}

public class LineBonus4 : IBonus
{
    public void GivingBonus(GameObject Obj)
    {
        Obj.GetComponent<SpriteRenderer>().color = new Color32(214, 231, 86, 255);
        Obj.GetComponent<Panels>().bonusName = "LineBonus4";
        Obj.GetComponent<Panels>().deleteOY = Obj.GetComponent<Panels>().Controller.verticalBonus;
        Obj.GetComponent<Panels>().ID = 301;
    }
    public void BonusEffect(GameObject Obj)
    {
        Controller controller = Obj.GetComponent<Panels>().Controller;

        if (controller.currentPanel.GetComponent<Panels>().ID == 301 &&
            controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 301)
        {
            controller.ClearLine(Obj, true);
            controller.ClearLine(Obj, false);
        }
        else if ((controller.currentPanel.GetComponent<Panels>().ID == 301 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 302) ||
                 (controller.currentPanel.GetComponent<Panels>().ID == 302 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 301))
        {
             
        }
        else if ((controller.currentPanel.GetComponent<Panels>().ID == 301 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 303) ||
                 (controller.currentPanel.GetComponent<Panels>().ID == 303 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 301))
        {
            for(int i = 0; i < 3; i++)
            {
                RaycastHit2D[] line = Physics2D.RaycastAll(new Vector3(controller.currentPanel.transform.position.x - 1 + i, controller.creatingPanel.startPosition.y, 0), Vector2.down, 100f, LayerMask.GetMask("Panel"));
                Debug.Log(line.Length);
                if (line.Length != 0)
                {
                    for(int j = 0; j < line.Length; j++)
                    {
                        line[j].transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
                    }
                }

                RaycastHit2D[] line2 = Physics2D.RaycastAll(new Vector3(controller.creatingPanel.startPosition.x, controller.currentPanel.transform.position.y - 1 + i, 0), Vector2.right, 100f, LayerMask.GetMask("Panel"));
                if (line.Length != 0)
                {
                    for (int j = 0; j < line2.Length; j++)
                    {
                        line2[j].transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
                    }
                }
            }
            controller.currentPanel.GetComponent<SpriteRenderer>().sprite = null;
            controller.hitPanel.transform.gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }
        else if ((controller.currentPanel.GetComponent<Panels>().ID == 301 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 304) ||
                 (controller.currentPanel.GetComponent<Panels>().ID == 304 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 301))
        {
            controller.FillingBonuses301();
        }
        else
        {
            if (Obj.GetComponent<Panels>().deleteOY == true)
            {
                controller.ClearLine(Obj, true);
            }
            else if (Obj.GetComponent<Panels>().deleteOY == false)
            {
                controller.ClearLine(Obj, false);
            }
        }
    }
        
    
}

public class CubeBonus : IBonus
{
    public void GivingBonus(GameObject Obj)
    {
        Obj.GetComponent<SpriteRenderer>().color = new Color32(185, 115, 0, 255);
        Obj.GetComponent<Panels>().bonusName = "CubeBonus";
        Obj.GetComponent<Panels>().ID = 302;
    }
    public void BonusEffect(GameObject Obj)
    {
        Controller controller = Obj.GetComponent<Panels>().Controller;


        if (controller.currentPanel.GetComponent<Panels>().ID == 302 &&
        controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 302)
        {
            controller.ThreeRandomBonuses();
        }
        else if ((controller.currentPanel.GetComponent<Panels>().ID == 302 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 303) ||
                 (controller.currentPanel.GetComponent<Panels>().ID == 303 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 302))
        {

        }
        else if ((controller.currentPanel.GetComponent<Panels>().ID == 302 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 304) ||
                 (controller.currentPanel.GetComponent<Panels>().ID == 304 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 302))
        {
            controller.FillingBonuses302();
        }
        else if ((controller.currentPanel.GetComponent<Panels>().ID < 300 ||
                controller.currentPanel.GetComponent<Panels>().ID > 302) ||
                (controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID < 300 ||
                controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID > 302))
        {

            controller.ClearPanelWithAI(Obj);
        }

    }
}

public class LinesOf3Panels : IBonus
{
    public void GivingBonus(GameObject Obj)
    {
        Obj.GetComponent<SpriteRenderer>().color = new Color32(151, 190, 51, 255);
        Obj.GetComponent<Panels>().bonusName = "LinesOf3Panels";
        Obj.GetComponent<Panels>().ID = 303;
    }
    public void BonusEffect(GameObject Obj)
    {
        Controller controller = Obj.GetComponent<Panels>().Controller;


        if (controller.currentPanel.GetComponent<Panels>().ID == 303 &&
        controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 303)
        {
            controller.ClearPanelOnCube(Obj, 8);
        }
        else if ((controller.currentPanel.GetComponent<Panels>().ID == 303 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 304) ||
                 (controller.currentPanel.GetComponent<Panels>().ID == 304 &&
                 controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 303))
        {
            controller.FillingBonuses303();
        }
        else 
        {
            controller.ClearPanelOnCube(Obj, 5);
        }

    }
}

public class LineBonus5 : IBonus
{
    public void GivingBonus(GameObject Obj)
    {
        Obj.GetComponent<SpriteRenderer>().color = new Color32(30, 31, 130, 255);
        Obj.GetComponent<Panels>().bonusName = "LineBonus5";
        Obj.GetComponent<Panels>().ID = 304;
    }

    public void BonusEffect(GameObject Obj)
    {
        Controller controller = Obj.GetComponent<Panels>().Controller;
        if (controller.currentPanel.GetComponent<Panels>().ID == 304 &&
            controller.hitPanel.transform.gameObject.GetComponent<Panels>().ID == 304)
        {
            controller.DestroyAll();
        }
        else 
        {
            List<GameObject> panels = controller.SingleClolorPanels();
            Obj.GetComponent<SpriteRenderer>().sprite = null;
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].GetComponent<SpriteRenderer>().sprite = null;
            }
        }
    }
}
