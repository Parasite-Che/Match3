using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidPanels : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Panels>().falling++;
    }
}
