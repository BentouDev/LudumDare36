using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Pawn : MonoBehaviour
{
    public bool drawDebug;

    private PlayerController.InputData CurrentInput;

    public bool IsAlive { get; private set; }

    public Rigidbody Rigidbody;

    public float Speed;

    private Vector3 Velocity;
    private Vector3 AppliedVelocity;

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
