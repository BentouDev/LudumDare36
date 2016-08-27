using System.Collections;
using UnityEngine;

class StartState : GameState
{
    public WorldGenerator Generator;
    public float StartDelay;

    IEnumerator StartAnim()
    {
        yield return new WaitForSeconds(StartDelay);

        var world = Generator.GenerateWorld();

        Game.World.CreateWorld(Game, world);
        Game.MiniMap.Init(Game, world);

        Game.SwitchState<PlayState>();
    }

    public void DoStart()
    {
        StartCoroutine(StartAnim());
    }

    protected override void OnStart()
    {

    }

    protected override void OnUpdate()
    {

    }

    protected override void OnEnd()
    {

    }
}
