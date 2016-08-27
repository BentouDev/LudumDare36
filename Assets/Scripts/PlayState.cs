using UnityEngine;
using System.Collections.Generic;

public class PlayState : GameState
{
    protected override void OnStart()
    {
        Game.Player.OnStart(Vector3.zero);
        Game.Camera.SetTarget(Game.Player.Pawn.transform);
    }

    protected override void OnUpdate()
    {
        Game.Player.OnUpdate();

        if (!Game.Player.Pawn.IsAlive)
        {
            Game.SwitchState<GameOver>();
        }
    }

    protected override void OnEnd()
    {
        Game.Player.OnEnd();
    }
}
