using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWin : GameState
{
    public MenuBase WinMenu;

    public float Delay;

    IEnumerator DoDelay()
    {
        yield return new WaitForSeconds(Delay);

        Game.Controller.SwitchToMenu(WinMenu);
        Game.Controller.AnimShow(WinMenu);
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
