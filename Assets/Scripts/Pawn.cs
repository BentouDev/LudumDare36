using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Pawn : MonoBehaviour, IDamageable
{
    public bool drawDebug;

    public Transform BodyBone;
    public Transform HeadBone;

    public AudioRandomizer ShootEffect;
    public AudioRandomizer ScreamEffect;

    public GameObject BulletPrefab;

    private PlayerController.InputData CurrentInput;

    public Rigidbody Rigidbody;

    public float Speed;

    public int MaxHealth;
    public int Health;

    private Vector3 Velocity;
    private Vector3 AppliedVelocity;

    private float LastShootTime;
    public float ShootPerSec;
    public float BulletStartDistance;

    private float ShootDelay { get { return 1.0f / ShootPerSec; } }

    public bool IsAlive { get; private set; }

    private List<Door.Key> Keys = new List<Door.Key>();

    public bool HasKey(Door.Key key)
    {
        return Keys.Contains(key);
    }

    public void PickupKey(Door.Key keyPickup)
    {
        Keys.Add(keyPickup);
    }

    void Start()
    {
        IsAlive = true;
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void OnInput(PlayerController.InputData currentInput)
    {
        if (Health <= 0)
        {
            IsAlive = false;
        }

        CurrentInput = IsAlive ? currentInput : new PlayerController.InputData();

        var flatVelocity = new Vector3(CurrentInput.Move.x, 0, CurrentInput.Move.y);
        Velocity = flatVelocity * Speed;

        if(Velocity.magnitude > Mathf.Epsilon)
            BodyBone.forward = Velocity.normalized;
        
        var shootDir = new Vector3(currentInput.Shoot.x, 0, currentInput.Shoot.y);
        if (shootDir.magnitude > Mathf.Epsilon)
        {
            HeadBone.rotation = Quaternion.LookRotation(Vector3.up, shootDir.normalized);
            Shoot(shootDir.normalized);
        }
    }

    public void Heal(int heal)
    {
        Health += heal;
    }

    public void TakeDamage(int damage)
    {
        ScreamEffect.Play();
        Health -= damage;
    }

    void Shoot(Vector3 dir)
    {
        if (Time.time - LastShootTime > ShootDelay)
        {
            ShootEffect.Play();
            LastShootTime = Time.time;

            var vel = Velocity.normalized * 0.125f;
            
            var go = Instantiate(BulletPrefab) as GameObject;
                go.transform.position = transform.position + dir * BulletStartDistance;

            var bullet = go.GetComponentInChildren<Bullet>();
                bullet.OnStart(dir - vel);
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void ApplyMovement()
    {
        AppliedVelocity = Velocity * Time.fixedDeltaTime;
        Rigidbody.velocity = Velocity;
    }

    void OnGUI()
    {
        if (!drawDebug)
            return;

        GUI.Label(new Rect(10,10,200,30), "Input " + CurrentInput.Move);
        GUI.Label(new Rect(10,30,200,30), "Vel " + Velocity);
        GUI.Label(new Rect(10,50,200,30), "Appleid " + AppliedVelocity);
        GUI.Label(new Rect(10,70,200,30), "HP " + Health);
    }
}
