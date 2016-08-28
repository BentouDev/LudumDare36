using UnityEngine;
using System.Collections.Generic;

public class DeepDoors : MonoBehaviour
{
    private Game Game;

    public void Init(Game game)
    {
        Game = game;
    }

    void OnTriggerEnter(Collider collider)
    {
        var pawn = collider.GetComponentInParent<Pawn>();
        if (pawn)
        {
            Game.SwitchState<GameWin>();
        }
    }
}
