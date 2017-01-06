using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWin : GameState
{
    public float Delay;

    IEnumerator DoDelay()
    {
        yield return new WaitForSeconds(Delay);

        GameModeManager.DispatchGameWin();
    }

    protected override void OnStart()
    {
        Game.Music.FadeSound();
        Game.Score.StopCountingTime();

        StartCoroutine(DoDelay());
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnEnd()
    {
        Game.OnLevelCleanUp();
    }
}
