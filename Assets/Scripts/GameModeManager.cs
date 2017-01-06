using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    private static GameModeManager _instance;
    public static GameModeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = SetupInstance();
            }

            return _instance;
        }
    }

    public Game.GameMode CurrentGameMode { get; private set; }

    public Game Game { get; set; }

    [Header("Mode Settings")]
    public int CurrentWorld;
    public int TimeAttackWorlds = 5;

    [Header("Menus")]
    public MenuBase LoseMenu;
    public MenuBase WinMenu;

    private static GameModeManager SetupInstance()
    {
        var holder = FindObjectOfType<GameModeManager>();
        if (holder == null)
        {
            var go = new GameObject("GameModeManager");
            holder = go.AddComponent<GameModeManager>();
        }

        return holder;
    }

    void Start()
	{
		DontDestroyOnLoad(this);
	}

    public static void SetGameMode(Game.GameMode mode)
    {
        Instance.CurrentGameMode = mode;
    }

    public static void DispatchGameWin()
    {
        Instance.HandleGameResult();
    }

    public static void DispatchGameLose()
    {
        Instance.HandleGameLose();
    }

    private void HandleGameResult()
    {
        CurrentWorld++;

        switch (CurrentGameMode)
        {
            case Game.GameMode.ScoreAttack:
                Game.RestartGame();
                break;
            case Game.GameMode.TimeAttack:
                if (CurrentWorld >= TimeAttackWorlds)
                {
                    Game.Controller.SwitchToMenu(WinMenu);
                    Game.Controller.AnimShow(WinMenu);
                }
                else
                {
                    Game.RestartGame();
                }
                break;
        }
    }

    private void HandleGameLose()
    {
        CurrentWorld++;

        switch (CurrentGameMode)
        {
            case Game.GameMode.TimeAttack:
                Game.Controller.AnimShow(LoseMenu);
                break;
            case Game.GameMode.ScoreAttack:
                Game.Controller.SwitchToMenu(WinMenu);
                Game.Controller.AnimShow(WinMenu);
                break;
        }
    }
}
