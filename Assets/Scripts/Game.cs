using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    private List<GameState> States;

    void Start()
    {
        States = new List<GameState>();

        var states = FindObjectsOfType<GameState>();
        foreach (var state in states)
        {
            state.Init(this);
            States.Add(state);
        }
    }
}
