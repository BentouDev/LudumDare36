using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Game : MonoBehaviour
{
    public WorldController World;
    public MiniMapController MiniMap;
    public PlayerController Player;
    public FollowCamera Camera;

    private List<GameState> States;
    
    private GameState CurrentState;

    void Start()
    {
        if (!World)
            World = FindObjectOfType<WorldController>();

        if (!MiniMap)
            MiniMap = FindObjectOfType<MiniMapController>();

        if (!Player)
            Player = FindObjectOfType<PlayerController>();

        States = new List<GameState>();

        var states = FindObjectsOfType<GameState>();
        foreach (var state in states)
        {
            state.Init(this);
            States.Add(state);
        }
        
        CurrentState = States.FirstOrDefault(s => s is StartState);

        if(CurrentState)
            CurrentState.CallStart();
    }

    void Update()
    {
        if(CurrentState)
            CurrentState.CallUpdate();
    }

    public void SwitchState<T>() where T : GameState
    {
        var state = States.First(s => s is T);

        if(CurrentState) CurrentState.CallEnd();
            CurrentState = state;
        if(CurrentState) CurrentState.CallStart();
    }
}
