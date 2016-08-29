using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, IDamageable
{
    public AudioRandomizer Sound;

    protected Pawn Pawn;
    protected Rigidbody Rigidbody;

    public float Speed;
    public int Health;
    public GameObject BulletPrefab;

    protected float LastShootTime;
    public float ShootPerSec;
    public float BulletStartDistance;

    public float BlinkTime;

    protected float ShootDelay { get { return 1.0f / ShootPerSec; } }

    public bool IsAlive { get { return Health > 0; } }

    protected bool WasAlive;

    void OnDestroy()
    {
        Sound.Audio.Stop();
    }

    public void OnStart(Pawn pawn)
    {
        Sound.Play();

        Rigidbody = GetComponent<Rigidbody>();
        LastShootTime = Time.time;
        Pawn = pawn;
    }

    IEnumerator WaitDisable()
    {
        yield return new WaitForSeconds(BlinkTime);

        gameObject.SetActive(false);
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

    public void TakeDamage(int damage)
    {
        Health -= damage;

        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            StartCoroutine(Blink(renderer));
        }
    }

    public IEnumerator Blink(MeshRenderer renderer, float coefficient = 1)
    {
        float elapsed = 0;
        while (elapsed < BlinkTime)
        {
            elapsed += Time.deltaTime;

            renderer.material.SetFloat("_Blink", Mathf.Sin(coefficient * (elapsed / BlinkTime)));

            yield return null;
        }
        
        renderer.material.SetFloat("_Blink", 0);
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
