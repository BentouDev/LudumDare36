using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class Pawn : MonoBehaviour, IDamageable, ILevelDependable
{
    [Header("Debug")]
    public bool drawDebug;

    [Header("Mesh")]
    public Transform BodyBone;
    public Transform HeadBone;
    public Rigidbody Rigidbody;
    public float BulletStartDistance;

    [Header("Audio")]
    public AudioRandomizer ShootEffect;
    public AudioRandomizer ScreamEffect;

    private PlayerController.InputData CurrentInput;
    
    [System.Serializable]
    public struct PawnData
    {
        [SerializeField]
        public GameObject BulletPrefab;

        [SerializeField]
        public float Speed;

        [SerializeField]
        public int Health;

        [SerializeField]
        public float ShootPerSec;

        [SerializeField]
        public List<GenericPickup> Pickups;
    }

    [Header("Data")]
    [SerializeField]
    public PawnData Data;
    
    private Vector3 Velocity;
    private Vector3 AppliedVelocity;

    private float LastShootTime;

    private float ShootDelay { get { return 1.0f / Data.ShootPerSec; } }

    public bool IsAlive { get; private set; }
    
    public bool HasKey(Door.Key key)
    {
        return Data.Pickups.Any(p =>
        {
            var k = p as KeyPickup;
            if (k != null)
            {
                return k.KeyInstance == key;
            }

            return false;
        });
    }

    public void AddPickup(GenericPickup pickup)
    {
        Data.Pickups.Add(pickup);
    }
    
    void Start()
    {
        IsAlive = true;
        Rigidbody = GetComponent<Rigidbody>();
    }
    
    public void OnLevelLoaded()
    {
        GameModeManager.LoadPawnData(ref Data);
    }

    public void OnLevelCleanUp()
    {
        GameModeManager.SavePawnData(Data);
    }

    public void OnInput(PlayerController.InputData currentInput)
    {
        if (Data.Health <= 0)
        {
            IsAlive = false;
        }

        CurrentInput = IsAlive ? currentInput : new PlayerController.InputData();

        var flatVelocity = new Vector3(CurrentInput.Move.x, 0, CurrentInput.Move.y);
        Velocity = flatVelocity * Data.Speed;

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
        Data.Health += heal;
    }

    public void TakeDamage(int damage)
    {
        ScreamEffect.Play();
        Data.Health -= damage;
    }

    void Shoot(Vector3 dir)
    {
        if (Time.time - LastShootTime > ShootDelay)
        {
            ShootEffect.Play();
            LastShootTime = Time.time;

            var vel = Velocity.normalized * 0.125f;
            
            var go = Instantiate(Data.BulletPrefab) as GameObject;
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
        GUI.Label(new Rect(10,70,200,30), "HP " + Data.Health);
    }
}
