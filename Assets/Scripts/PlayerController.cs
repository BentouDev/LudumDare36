using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private InputData CurrentInput;

    public GameObject PawnPrefab;
    public Pawn Pawn { get; private set; }

    public struct InputData
    {
        public bool Attack;
        public bool Special;
        public Vector2 Move;
    }

    public void OnStart(Vector3 spawnPoint)
    {
        SetupPawn(spawnPoint);
    }
    
    public void SetupPawn(Vector3 position)
    {
        if (!Pawn)
        {
            var go = Instantiate(PawnPrefab) as GameObject;
            Pawn = go.GetComponent<Pawn>();
        }

        Pawn.transform.position = position;
    }

    public void OnEnd()
    {
        DestroyObject(Pawn.gameObject);
    }

    public void OnUpdate()
    {
        CurrentInput = GatherInput();

        if (Pawn)
            Pawn.OnInput(CurrentInput);
    }

    private InputData GatherInput()
    {
        InputData input = new InputData();

        input.Attack = Input.GetButtonDown("Fire1");
        input.Special = Input.GetButtonDown("Jump");
        input.Move.x = Input.GetAxis("Horizontal");
        input.Move.y = Input.GetAxis("Vertical");

        return input;
    }
}
