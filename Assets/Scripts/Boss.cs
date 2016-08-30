﻿using UnityEngine;
using System.Collections.Generic;

public class Boss : Enemy
{
    public int ShootsAtOnce = 6;

    void Update()
    {
        if (CanMove && Pawn && IsAlive)
        {
            var distance = transform.position - Pawn.transform.position;
            if (Time.time - LastShootTime > ShootDelay)
            {
                float angle = 360.0f/(float) ShootsAtOnce;

                for (int i = 0; i < ShootsAtOnce; i++)
                {
                    var dir = Quaternion.AngleAxis(angle * i, Vector3.up)*Vector3.forward;
                    Shoot(dir);
                }
            }

            transform.forward = distance.normalized;
            Rigidbody.velocity = -distance.normalized * Speed;
        }

        if (!IsAlive && WasAlive)
        {
            StopAllCoroutines();

            var renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.material.SetColor("_BlinkColor", Color.grey);
                StartCoroutine(Blink(renderer, 0.5f));
            }

            StartCoroutine(WaitDisable());
        }

        WasAlive = IsAlive;
    }
}
