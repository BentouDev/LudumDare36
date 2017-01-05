using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeHolder : MonoBehaviour
{
    public static GameModeHolder Instance;

    public Game.GameMode CurrentGameMode { get; private set; }

	void Start()
	{
	    Instance = this;
		DontDestroyOnLoad(this);
	}

    public static void SetGameMode(Game.GameMode mode)
    {
        var holder = FindObjectOfType<GameModeHolder>();
        if (holder == null)
        {
            var go = new GameObject("GameModeHolder");
            holder = go.AddComponent<GameModeHolder>();
            Instance = holder;
        }

        holder.CurrentGameMode = mode;
    }
}
