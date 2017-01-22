using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScoreEntryMenu : MenuBase
{
    public UnityEngine.UI.Text ScoreField;
    public UnityEngine.UI.InputField NameField;

    private bool AlreadyClicked;

    public override void OnStart()
    {
        base.OnStart();

        if (ScoreField)
        {
            TimeSpan span = TimeSpan.FromSeconds(Game.Instance.Score.RealTimePassed);
            ScoreField.text = string.Format(" {0}\n {1:D2}m {2:D2}s", Game.Instance.Score.Score, span.Minutes, span.Seconds);
        }

        AlreadyClicked = false;

        Game.Instance.Score.OnScoreUploaded += OnScoreUploaded;
        Game.Instance.Score.OnNetworkError += OnNetworkError;
    }
    
    public void SubmitScore()
    {
        if(AlreadyClicked || !NameField || string.IsNullOrEmpty(NameField.text))
            return;
        
        AlreadyClicked = true;
        Canvas.interactable = false;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            OnNoInternetConnection();
        }
        else
        {
            OnTryUploadScore();
        }
    }

    private void OnTryUploadScore()
    {
        Game.Instance.Score.UploadScore(NameField.text, GameModeManager.Instance.CurrentGameMode);
    }

    public void OnScoreUploaded()
    {
        Game.Instance.Score.OnNetworkError -= OnNetworkError;
        Game.Instance.Score.OnScoreUploaded -= OnScoreUploaded;
        SceneManager.LoadScene(0);
    }

    private void OnNoInternetConnection()
    {
        Game.Instance.MessageMenu.SwitchTo (
            "Theres no internet connection!\nRetry?",
            OnTryUploadScore,
            OnScoreUploaded
        );
    }

    private void OnNetworkError(string message)
    {
        Game.Instance.MessageMenu.SwitchTo (
            "Unable to upload your score due to network error\nRetry?",
            OnTryUploadScore,
            OnScoreUploaded
        );
    }
}
