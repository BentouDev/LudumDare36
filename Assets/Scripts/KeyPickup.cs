using UnityEngine;
using System.Collections.Generic;

public class KeyPickup : GenericPickup
{
    protected Door.Key KeyInstance;

    public void Init(Door.Key key)
    {
        KeyInstance = key;
        var renderer = GetComponentInChildren<MeshRenderer>();
            renderer.material.color = KeyInstance.color;
    }

    protected override void OnPickup(Pawn pawn)
    {
        pawn.PickupKey(KeyInstance);
    }
}
