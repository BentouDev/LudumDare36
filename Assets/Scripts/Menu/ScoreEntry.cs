using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEntry : MonoBehaviour
{
    public Text PlayerName;
    public Text Result;
    public Text Time;

    public void SetScore(int index, ScoreManager.OnlineScore score)
    {
        if(PlayerName)
            PlayerName.text = string.Format("{0}. {1}", index, score.PlayerName);

        if (Result)
            Result.text = score.Score.ToString();

        if (Time)
        {
            TimeSpan span = TimeSpan.FromSeconds(score.Time);
            Time.text = string.Format("{0:D2}m {1:D2}s", span.Minutes, span.Seconds);
        }
    }
}
