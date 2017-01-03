using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Game : MonoBehaviour
{
    public MenuController Controller;
    public ScoreManager Score;
    public WorldController World;
    public MusicController Music;
    public MiniMapController MiniMap;
    public HealthBar Health;
    public PlayerController Player;
    public FollowCamera Camera;

    private List<GameState> States;
    
    private GameState CurrentState;

    public static Game Instance;

    public bool IsPlaying()
    {
        return CurrentState is PlayState;
    }

    void Start()
    {
        Instance = this;

        if (!Controller)
            Controller = FindObjectOfType<MenuController>();

        if (!Music)
            Music = FindObjectOfType<MusicController>();

        if (!World)
            World = FindObjectOfType<WorldController>();

        if (!MiniMap)
            MiniMap = FindObjectOfType<MiniMapController>();

        if (!Player)
            Player = FindObjectOfType<PlayerController>();

        if (!Score)
            Score = FindObjectOfType<ScoreManager>();

        if (Score)
            Score.Game = this;

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

    public void OnLevelLoaded()
    {
        var allObjs = FindObjectsOfType<GameObject>();
        foreach (GameObject allObj in allObjs)
        {
            var allLevelDependables = allObj.GetInterfaces<ILevelDependable>();
            if (allLevelDependables != null && allLevelDependables.Length > 0)
            {
                foreach (ILevelDependable dependable in allLevelDependables)
                {
                    dependable.OnLevelLoaded();
                }
            }
        }
    }

    public void OnLevelCleanUp()
    {
        var allObjs = FindObjectsOfType<GameObject>();
        foreach (GameObject allObj in allObjs)
        {
            var allLevelDependables = allObj.GetInterfaces<ILevelDependable>();
            if (allLevelDependables != null && allLevelDependables.Length > 0)
            {
                foreach (ILevelDependable dependable in allLevelDependables)
                {
                    dependable.OnLevelCleanUp();
                }
            }
        }
    }
}
