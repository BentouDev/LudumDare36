using System.Collections;
using UnityEngine;

class StartState : GameState
{
    public WorldGenerator Generator;
    public float StartDelay;

    private bool Started;

    IEnumerator StartAnim()
    {
        yield return new WaitForSeconds(StartDelay);

        var world = Generator.GenerateWorld();

        Game.World.CreateWorld(Game, world);
        Game.MiniMap.Init(Game, world);

        Game.Score.StartCountingTime();

        Game.SwitchState<PlayState>();
    }

    public void DoStart()
    {
        if (!Started)
        {
            Started = true;
            StartCoroutine(StartAnim());
        }
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
