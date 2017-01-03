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
        base.OnStart();

        Game.Instance.Score.OnHighscoreDownloaded -= OnScoresDownloaded;
        Game.Instance.Score.OnHighscoreDownloaded += OnScoresDownloaded;

        Game.Instance.Score.DownloadHighscores(ScoresToLoad);
    }

    public override void OnEnd()
    {
        base.OnEnd();

        foreach (GameObject score in currentScores)
        {
            Destroy(score);
        }

        currentScores.Clear();
    }

    private void OnScoresDownloaded(List<ScoreManager.OnlineScore> onlineScores)
    {
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

    public override void OnLevelLoaded()
    {
        
    }

    public override void OnLevelCleanUp()
    {

    }
}
