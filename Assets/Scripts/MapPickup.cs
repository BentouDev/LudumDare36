using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPickup : GenericPickup
{
    public MiniMapController MiniMap;

    private void Start()
    {
        if (!MiniMap)
        {
            MiniMap = FindObjectOfType<MiniMapController>();
        }
    }

    protected override void OnPickup(Pawn pawn)
    {
        MiniMap.DisplayUndiscovered = true;
    }
}
