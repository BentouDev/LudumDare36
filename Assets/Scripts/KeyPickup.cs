using UnityEngine;
using System.Collections.Generic;

public class KeyPickup : GenericPickup
{
    public Door.Key KeyInstance { get; private set; }

    protected override void OnInit()
    {
        KeyInstance = ParentRoom.KeyPickup;

        var meshRenderer = GetComponentInChildren<MeshRenderer>();
            meshRenderer.material.color = KeyInstance.color;
            meshRenderer.material.SetColor("_EmissionColor", KeyInstance.color);
    }
    
    protected override void OnPickup(Pawn pawn)
    {
        ParentRoom.PickedUpKey = true;
    }
}
