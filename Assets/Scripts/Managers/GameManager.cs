using System;
using MyGameDevTools.SceneLoading;
using UnityEngine;

public class GameManager : MonoBehaviour, IGameManager
{
    public enum GameEvent
    {
        None,
        TurnCameraLeft,
        TurnCameraRight,
        Moving,
        TorchOff,
        TorchOn,
        ZoomInOut
    }

    public ManagerStatus status => ManagerStatus.Started;


    public static event Action<GameEvent> OnGameEvent;
    public static event Action<bool> OnGameStop;

    private bool isGameStopped = false;
    private float prevTimeScale = 1.0f;


    public void Startup()
    {

    }

    public void TransitionScene(string toScene)
    {
        MySceneManager.TransitionAsync(toScene, "LoadingScene");
    }

    public void SetGameStopped(bool stopped)
    {
        if (stopped)
        {
            isGameStopped = true;
            this.prevTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;
        }
        else
        {
            isGameStopped = false;
            Time.timeScale = this.prevTimeScale;
        }
        OnGameStop?.Invoke(stopped);
    }

    public void PlayEvent(GameEvent gameEvent)
    {
        OnGameEvent?.Invoke(gameEvent);
    }

    public float UnscaledDeltaTime
    {
        get => this.isGameStopped ? 0.0f : Time.unscaledDeltaTime;
    }

    public bool IsGameStopped
    {
        get => this.isGameStopped;
    }
    

}
