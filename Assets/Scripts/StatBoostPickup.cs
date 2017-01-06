using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBoostPickup : GenericPickup
{
    public float SpeedBonus;
    public float ShootPerSecBonus;
    public float BulletSizeBonus;

    public GameObject BulletReplacement;

    protected override void OnPickup(Pawn pawn)
    {
        if (BulletReplacement)
        {
            pawn.Data.BulletPrefab = BulletReplacement;
        }

        pawn.Data.ShootPerSec += ShootPerSecBonus;
        pawn.Data.Speed += SpeedBonus;
        pawn.Data.BulletSize += BulletSizeBonus;
    }
}
