using UnityEngine;
using System.Collections.Generic;

public class DoorUnlocker : MonoBehaviour
{
    public Door Door;

    void OnTriggerEnter(Collider collider)
    {
        var pawn = collider.gameObject.GetComponentInParent<Pawn>();
        if (pawn && Door.KeyInstance != null && pawn.HasKey(Door.KeyInstance))
        {
            Door.UnlockWithKey(Door.KeyInstance);
        }
    }
}
