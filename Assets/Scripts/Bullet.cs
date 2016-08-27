using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float MaxLifeTime;

    public float Speed = 3;

    public float StartTime { get; set; }

    public Vector3 Direction { get; set; }

    public Rigidbody Rigidbody { get; set; }
    
    public void OnStart(Vector3 direction)
    {
        Rigidbody = GetComponentInChildren<Rigidbody>();
        
        StartTime = Time.time;
        Direction = direction;
    }

    void Update()
    {
        Rigidbody.velocity = Direction * Speed;

        if(Time.time - StartTime > MaxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        DestroyObject(gameObject);
    }
}
