using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWin : GameState
{
    public int NextLevel;
    public float Delay;

    IEnumerator DoDelay()
    {
        yield return new WaitForSeconds(Delay);

        LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(NextLevel);
    }

    protected override void OnStart()
    {
        StartCoroutine(DoDelay());
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnEnd()
    {

    }
}
