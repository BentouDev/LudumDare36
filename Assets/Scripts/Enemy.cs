using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IDamageable
{
    private Pawn Pawn;

    public int Health;
    public GameObject BulletPrefab;

    private float LastShootTime;
    public float ShootPerSec;
    public float BulletStartDistance;

    private float ShootDelay { get { return 1.0f / ShootPerSec; } }

    public bool IsAlive { get { return Health > 0; } }

    public void OnStart(Pawn pawn)
    {
        LastShootTime = Time.time;
        Pawn = pawn;
    }

    void Update()
    {
        if (Pawn && IsAlive)
        {
            var distance = transform.position - Pawn.transform.position;
            Shoot(-distance.normalized);
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

    void Shoot(Vector3 dir)
    {
        if (Time.time - LastShootTime > ShootDelay)
        {
            LastShootTime = Time.time;

            var go = Instantiate(BulletPrefab) as GameObject;
            go.transform.position = transform.position + dir * BulletStartDistance;

            var bullet = go.GetComponentInChildren<Bullet>();
            bullet.OnStart(dir);
        }
    }
}
