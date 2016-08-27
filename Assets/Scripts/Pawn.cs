using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Pawn : MonoBehaviour
{
    public GameObject BulletPrefab;

    public bool drawDebug;

    private PlayerController.InputData CurrentInput;

    public bool IsAlive { get; private set; }

    public Rigidbody Rigidbody;

    public float Speed;

    private Vector3 Velocity;
    private Vector3 AppliedVelocity;

    private float LastShootTime;
    public float ShootPerSec;
    public float BulletStartDistance;

    private float ShootDelay { get { return 1.0f / ShootPerSec; } }

    void Start()
    {
        IsAlive = true;
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void OnInput(PlayerController.InputData currentInput)
    {
        CurrentInput = IsAlive ? currentInput : new PlayerController.InputData();

        var flatVelocity = new Vector3(CurrentInput.Move.x, 0, CurrentInput.Move.y);
        Velocity = flatVelocity * Speed;

        var shootDir = new Vector3(currentInput.Shoot.x, 0, currentInput.Shoot.y);
        if (shootDir.magnitude > Mathf.Epsilon)
        {
            Shoot(shootDir.normalized);
        }
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
    }
}
