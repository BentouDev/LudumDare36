using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeHolder : MonoBehaviour
{
    public static GameModeHolder Instance;

    public Game.GameMode CurrentGameMode;

	void Start()
	{
	    Instance = this;
		DontDestroyOnLoad(this);
	}
}
