using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private InputData CurrentInput;

    public GameObject PawnPrefab;
    public Pawn Pawn { get; private set; }

    public struct InputData
    {
        public bool Special;
        public Vector2 Move;
        public Vector2 Shoot;
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
        if (Pawn)
        {
            DestroyObject(Pawn.gameObject);
            Pawn = null;
        }
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

        input.Special = Input.GetButtonDown("Jump");

        input.Move.x = Input.GetAxis("Horizontal");
        input.Move.y = Input.GetAxis("Vertical");

        input.Shoot.x = Input.GetAxis("Shoot Y");
        input.Shoot.y = Input.GetAxis("Shoot X");

        return input;
    }
}
