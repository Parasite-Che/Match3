using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panels : MonoBehaviour
{
    public Controller Controller;
    CreatingPanels creatingPanel;
    public int ID;
    public float falling = 0;
    public bool deleteOY;

    public string bonusName = "";
    bool topPanels = true;

    private void Awake()
    {
        Controller = GameObject.Find("Controller").GetComponent<Controller>();
        creatingPanel = GameObject.Find("Controller").GetComponent<CreatingPanels>();
    }

    private void Update()
    {
        if (falling > 0)
        {
            transform.position += new Vector3(0, -0.1f, 0);
            falling -= 0.1f;
            creatingPanel.isFalling = true;
            if (falling < 0)
            {
                transform.position = new Vector3(transform.position.x, Mathf.Floor(transform.position.y + 0.15f));
                falling = 0;
                creatingPanel.isFalling = false;
                Controller.matchFound = false;
            }
        }
        if (gameObject.GetComponent<SpriteRenderer>().sprite == null && falling == 0)
        {
            creatingPanel.FillingInEmptyFields(gameObject.transform.position.x);
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(gameObject.transform.position, Vector2.up, 100.0F, LayerMask.GetMask("Panel"));
            if (hits != null)
            {
                for (int j = 0; j < hits.Length; j++)
                {
                    hits[j].transform.gameObject.GetComponent<Panels>().falling += 1;
                    if (hits[j].transform.GetComponent<SpriteRenderer>().sprite != null)
                    {
                        topPanels = false;
                    }
                }
                int rand = Random.Range(0, hits.Length);
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

                if (topPanels)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                    creatingPanel.isFalling = false;
                }
            }            
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
                Controller.UseBonus(Controller.currentPanel);
                if (Controller.hitPanel)
                    Controller.UseBonus(Controller.hitPanel.transform.gameObject);

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
}
