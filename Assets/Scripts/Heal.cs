using UnityEngine;
using System.Collections.Generic;

public class Heal : MonoBehaviour
{
    public int heal = 1;

    void OnTriggerEnter(Collider collider)
    {
        var pawn = collider.gameObject.GetComponentInParent<Pawn>();
        if (pawn)
        {
            pawn.Heal(heal);
        }
    }
}
