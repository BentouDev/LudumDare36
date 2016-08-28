using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public abstract class GenericPickup : MonoBehaviour
{
    protected abstract void OnPickup(Pawn pawn);

	void OnTriggerEnter(Collider collider)
    {
        var pawn = collider.GetComponentInParent<Pawn>();
        if(pawn)
        {
            OnPickup(pawn);
            DestroyObject(gameObject);
        }
    }
}
