using UnityEngine;
using System.Collections.Generic;

public class HealthPickup : GenericPickup
{
    public int Amount;

    protected override void OnPickup(Pawn pawn)
    {
        pawn.Heal(Amount);
    }
}
