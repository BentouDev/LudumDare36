using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScoreEntryMenu : MenuBase
{
    public UnityEngine.UI.Text ScoreField;
    public UnityEngine.UI.InputField NameField;

    private bool Clicked;

    public override void OnStart()
    {
        base.OnStart();

        if (ScoreField)
        {
            TimeSpan span = TimeSpan.FromSeconds(Game.Instance.Score.RealTimePassed);
            ScoreField.text = string.Format(" {0}\n {1:D2}m {2:D2}s", Game.Instance.Score.Score, span.Minutes, span.Seconds);
        }

        Clicked = false;

        Game.Instance.Score.OnScoreUploaded += OnScoreUploaded;
        Game.Instance.Score.OnNetworkError += OnNetworkError;
    }
    
    public void SubmitScore()
    {
        if(Clicked)
            return;
        
        Clicked = true;
        Canvas.interactable = false;
        
        if (NameField && !string.IsNullOrEmpty(NameField.text))
        {
            Game.Instance.Score.UploadScore(NameField.text, GameModeHolder.Instance.CurrentGameMode);
        }
    }

    public void OnScoreUploaded()
    {
        Game.Instance.Score.OnNetworkError -= OnNetworkError;
        Game.Instance.Score.OnScoreUploaded -= OnScoreUploaded;
        SceneManager.LoadScene(0);
    }

    private void RetrySend()
    {
        Game.Instance.Score.UploadScore(NameField.text, GameModeHolder.Instance.CurrentGameMode);
    }

    private void OnNetworkError(string message)
    {
        Game.Instance.MessageMenu.SwitchTo (
            "Unable to upload your score due to network error\nRetry?",
            RetrySend,
            OnScoreUploaded
        );
    }
}
