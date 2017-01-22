
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : GameState
{
    public float DelayTime = 3;

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(DelayTime);

        GameModeManager.DispatchGameLose();
    }

    protected override void OnStart()
    {
        Game.Music.FadeSound();
        Game.Score.StopCountingTime();

        StartCoroutine(Delay());
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnEnd()
    {
        Game.OnLevelCleanUp();
    }
}
