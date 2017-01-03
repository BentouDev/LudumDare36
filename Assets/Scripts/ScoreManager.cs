using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreManager : MonoBehaviour
{
    public delegate void HighscoreDelegate(List<OnlineScore> scores);
    public delegate void ScoreDelegate();

    public event HighscoreDelegate OnHighscoreDownloaded;
    public event ScoreDelegate OnScoreUploaded;

    [System.Serializable]
    public struct OnlineScore
    {
        [SerializeField]
        public string PlayerName;

        [SerializeField]
        public int Score;

        [SerializeField]
        public int Time;
    }

    [System.Serializable]
    public struct Highscore
    {
        public OnlineScore[] Scores;
    }

    public Game Game;

    public string Domain;

    public string Api;

    public int Port;

    public string DomainApiUrl
    {
        get { return string.Format("{0}:{1}/{2}", Domain, Port, Api); }
    }

    private bool _countTime;

    private float _realTimePassed;

    public float RealTimePassed
    {
        get { return _realTimePassed; }
    }

    private int _score;

    public int Score
    {
        get { return _score; }
    }

    public void OnEnemyKilled()
    {
        _score += 2;
    }

    public void OnEnemyDamaged(int damage)
    {

    }

    public void StartCountingTime()
    {
        _countTime = true;
    }

    public void StopCountingTime()
    {
        _countTime = false;
    }

    void Update()
    {
        if (Game.IsPlaying() && _countTime)
        {
            _realTimePassed += Time.unscaledDeltaTime;
        }
    }

    public void UploadScore(string playerName)
    {
        StartCoroutine(UploadCoroutine(new OnlineScore()
        {
            PlayerName = playerName,
            Score = Score,
            Time = Mathf.RoundToInt(RealTimePassed)
        }));
    }

    private IEnumerator UploadCoroutine(OnlineScore score)
    {
        string json = JsonUtility.ToJson(score);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

        var headers = new Dictionary<string, string> {{"Content-Type", "application/json"}};

        WWW www = new WWW(DomainApiUrl, data, headers);

        yield return www;
        
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Form upload failed : " + www.error);
        }
        else
        {
            Debug.Log("Form upload succeded : " + www.text);
        }

        if (OnScoreUploaded != null)
        {
            OnScoreUploaded();
        }
    }

    public void DownloadHighscores(int topScoreCount)
    {
        StartCoroutine(DownloadCoroutine(topScoreCount));
    }

    private IEnumerator DownloadCoroutine(int topScoreCount)
    {
        UnityWebRequest request = UnityWebRequest.Get(string.Format("{0}/{1}", DomainApiUrl, topScoreCount));
        yield return request.Send();

        if (request.isError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("Form download complete!");

            try
            {
                string    json       = WrapToClass(request.downloadHandler.text, "Scores");
                Highscore highscores = JsonUtility.FromJson<Highscore>(json);

                HighscoreDownloaded(highscores.Scores.ToList());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    private void HighscoreDownloaded(List<OnlineScore> scores)
    {
        if (OnHighscoreDownloaded != null)
        {
            OnHighscoreDownloaded(scores);
        }
    }

    public static string WrapToClass(string source, string topClass)
    {
        return string.Format("{{ \"{0}\": {1}}}", topClass, source);
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(10,10,200,30), "Time : " + RealTimePassed);
        GUI.Label(new Rect(10,30,200,30), "Score : " + Score);
    }
}
