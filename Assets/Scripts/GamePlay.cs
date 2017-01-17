using UnityEngine;
using System.Collections.Generic;

public class GamePlay : GameState
{
    protected override void OnStart()
    {
        Game.Player.OnStart(Vector3.zero);
        Game.Camera.SetTarget(Game.Player.Pawn.transform);
        Game.Health.Reference = Game.Player.Pawn;

        Game.OnLevelLoaded();
    }

    protected override void OnUpdate()
    {
        Game.Player.OnUpdate();

        if (!Game.Player.Pawn.IsAlive)
        {
            Game.Music.UnfadeDeath();
            Game.SwitchState<GameOver>();
        }
    }

    protected override void OnEnd()
    {
        Game.Player.OnEnd();
    }
}
