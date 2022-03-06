using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusControl<T> where T : IBonus
{
    public BonusControl( T value)
    {
        //value.BonusEffect(gameObject);
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

public class CubeBonus : IBonus
{
    public void GivingBonus(GameObject Obj)
    {
        Obj.GetComponent<SpriteRenderer>().color = new Color32(185, 115, 0, 255);
        Obj.GetComponent<Panels>().bonusName = "CubeBonus";
        Obj.GetComponent<Panels>().ID = 301;
    }
    public void BonusEffect(GameObject Obj)
    {

    }
}

public class LineBonus4 : IBonus
{
    public void GivingBonus(GameObject Obj)
    {
        Obj.GetComponent<SpriteRenderer>().color = new Color32(214, 231, 86, 255);
        Obj.GetComponent<Panels>().bonusName = "LineBonus4";
        Obj.GetComponent<Panels>().ID = 302;
    }
    public void BonusEffect(GameObject Obj)
    {

    }
}

public class LineBonus5 : IBonus
{
    public void GivingBonus(GameObject Obj)
    {
        Obj.GetComponent<SpriteRenderer>().color = new Color32(30, 31, 130, 255);
        Obj.GetComponent<Panels>().bonusName = "LineBonus5";
        Obj.GetComponent<Panels>().ID = 303;
    }
    public void BonusEffect(GameObject Obj)
    {

    }
}

public class LinesOf3Panels: IBonus
{
    public void GivingBonus(GameObject Obj)
    {
        Obj.GetComponent<SpriteRenderer>().color = new Color32(151, 190, 51, 255);
        Obj.GetComponent<Panels>().bonusName = "LinesOf3Panels";
        Obj.GetComponent<Panels>().ID = 304;
    }
    public void BonusEffect(GameObject Obj)
    {

    }
}
