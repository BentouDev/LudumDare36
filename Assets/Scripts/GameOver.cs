
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : GameState
{
    public MenuBase OverMenu;

    public float DelayTime = 3;

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(DelayTime);

        Game.Controller.SwitchToMenu(OverMenu);
        Game.Controller.AnimShow(OverMenu);
    }

    protected override void OnStart()
    {
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
