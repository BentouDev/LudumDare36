using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, IDamageable
{
    protected Pawn Pawn;
    protected Rigidbody Rigidbody;

    public float Speed;
    public int Health;
    public GameObject BulletPrefab;

    protected float LastShootTime;
    public float ShootPerSec;
    public float BulletStartDistance;

    protected float ShootDelay { get { return 1.0f / ShootPerSec; } }

    public bool IsAlive { get { return Health > 0; } }

    public void OnStart(Pawn pawn)
    {
        Rigidbody = GetComponent<Rigidbody>();
        LastShootTime = Time.time;
        Pawn = pawn;
    }

    void Update()
    {
        if (Pawn && IsAlive)
        {
            var distance = transform.position - Pawn.transform.position;
            if (Time.time - LastShootTime > ShootDelay)
            {
                Shoot(-distance.normalized);
            }

            transform.forward = distance.normalized;
            Rigidbody.velocity = -distance.normalized * Speed;
        }
        else
        {
            Rigidbody.velocity = Vector3.zero;
        }

        if (!IsAlive)
        {
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    protected void Shoot(Vector3 dir)
    {
        LastShootTime = Time.time;

        var go = Instantiate(BulletPrefab) as GameObject;
        go.transform.position = transform.position + dir * BulletStartDistance;

        var bullet = go.GetComponentInChildren<Bullet>();
        bullet.OnStart(dir);
    }
}
