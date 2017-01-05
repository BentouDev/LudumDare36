
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : GameState
{
    public MenuBase LoseMenu;
    public MenuBase SubmitScoreMenu;

    public float DelayTime = 3;

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(DelayTime);

        switch (GameModeHolder.Instance.CurrentGameMode)
        {
            case Game.GameMode.TimeAttack:
                Game.Controller.AnimShow(LoseMenu);
                break;
            case Game.GameMode.ScoreAttack:
                Game.Controller.SwitchToMenu(SubmitScoreMenu);
                Game.Controller.AnimShow(SubmitScoreMenu);
                break;
        }
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
