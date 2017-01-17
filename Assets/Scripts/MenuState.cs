using System.Collections;
using UnityEngine;

class MenuState : GameState
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

        Game.SwitchState<GamePlay>();
    }

    public void BeginGame()
    {
        if (!Started)
        {
            Started = true;
            StartCoroutine(StartAnim());
        }
    }

    protected override void OnStart()
    {
        Started = false;
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnEnd()
    {

    }
}
