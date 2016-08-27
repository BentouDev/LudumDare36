using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class GameState : MonoBehaviour
{
    [System.Serializable]
    public class GameEvent : UnityEvent { }

    protected Game Game;

    public List<Transform> DisableOnStart;

    public List<Transform> DisableOnEnd;

    public GameEvent OnStartEvent;

    public GameEvent OnEndEvent;

    public void Init(Game game)
    {
        Game = game;
    }

    public void CallStart()
    {
        foreach (Transform tr in DisableOnStart)
        {
            tr.gameObject.SetActive(false);
        }

        OnStartEvent.Invoke();
        OnStart();
    }

    public void CallUpdate()
    {
        OnUpdate();    
    }

    public void CallEnd()
    {
        foreach (Transform tr in DisableOnEnd)
        {
            tr.gameObject.SetActive(false);
        }

        OnEndEvent.Invoke();
        OnEnd();
    }

    protected abstract void OnStart();

    protected abstract void OnUpdate();

    protected abstract void OnEnd();
}
