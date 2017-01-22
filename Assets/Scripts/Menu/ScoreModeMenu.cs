using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreModeMenu : MenuBase
{
    public Game GameMain;
    public Highscores HighscoreMenu;

    public void GoToHighScoreAttack()
    {
        GameMain.SetGameMode(Game.GameMode.ScoreAttack);
        HighscoreMenu.SwitchTo();
    }

    public void GoToHightTimeAttack()
    {
        GameMain.SetGameMode(Game.GameMode.TimeAttack);
        HighscoreMenu.SwitchTo();
    }
}
