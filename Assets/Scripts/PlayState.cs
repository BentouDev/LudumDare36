using UnityEngine;
using System.Collections.Generic;

public class PlayState : GameState
{
    protected override void OnStart()
    {
        Game.Player.OnStart(Vector3.zero);
    }

    protected override void OnUpdate()
    {
        Game.Player.OnUpdate();
    }

    protected override void OnEnd()
    {
        Game.Player.OnEnd();
    }
}
