using UnityEngine;

class StartState : GameState
{
    public WorldGenerator Generator;

    protected override void OnStart()
    {
        var world = Generator.GenerateWorld();

        Game.World.CreateWorld(Game, world);
        Game.MiniMap.Init(Game, world);
        
        Game.SwitchState<PlayState>();
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnEnd()
    {

    }
}
