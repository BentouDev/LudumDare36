using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highscores : MenuBase
{
    public int ScoresToLoad = 10;

    public GameObject ScoreEntryPrefab;

    public RectTransform ScoreEntryParent;

    private List<GameObject> currentScores = new List<GameObject>();

    public override void OnStart()
    {
        gameObject.SetActive(true);

        Game.Instance.Score.OnHighscoreDownloaded += OnScoresDownloaded;
        Game.Instance.Score.OnNetworkError += OnNetworkError;
        
        Game.Instance.Score.DownloadHighscores(ScoresToLoad, GameModeHolder.Instance.CurrentGameMode);
    }

    public override void OnEnd()
    {
        base.OnEnd();

        Game.Instance.Score.OnHighscoreDownloaded -= OnScoresDownloaded;
        Game.Instance.Score.OnNetworkError -= OnNetworkError;

        foreach (GameObject score in currentScores)
        {
            Destroy(score);
        }

        currentScores.Clear();
    }

    private void OnNetworkError(string message)
    {
        Game.Instance.MessageMenu.SwitchTo (
            "Unable to download Highscores\ndue to network error\nReturning to Main Menu",
            GoToMainMenu
        );
    }

    private void GoToMainMenu()
    {
        Controller.AnimShow();
    }

    private void OnScoresDownloaded(List<ScoreManager.OnlineScore> onlineScores)
    {
        StartCoroutine(AnimShow(AnimTime));

        int index = 1;
        foreach (ScoreManager.OnlineScore score in onlineScores)
        {
            var go = Instantiate(ScoreEntryPrefab) as GameObject;
                go.transform.SetParent(ScoreEntryParent);

            var scoreEntry = go.GetComponent<ScoreEntry>();
                scoreEntry.SetScore(index++, score);

            currentScores.Add(go);
        }
    }
}
