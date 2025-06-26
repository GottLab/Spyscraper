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

    private struct GameData : ISaveData
    {

        public string currentScene;

        public string Name()
        {
            return "game_save";
        }
    }


    public ManagerStatus status => ManagerStatus.Started;

    public static event Action<GameEvent> OnGameEvent;
    public static event Action<bool> OnGamePause;

    [SerializeField, Tooltip("If player can pause this scene using Escape")]
    private bool CanPauseGame = true;

    private bool isGameStopped = false;
    private float prevTimeScale = 1.0f;

    private bool isChangingScene;

    public void Startup()
    {

    }

    public void TransitionScene(string toScene)
    {
        this.isChangingScene = true;
        MySceneManager.TransitionAsync(toScene, "LoadingScene");
    }

    public void ReloadScene()
    {
        TransitionScene(MySceneManager.GetActiveScene().name);
    }

    void Update()
    {
        if (CanPauseGame && Input.GetKeyDown(KeyCode.Escape) && !this.isChangingScene)
        {
            ToggleGameStop();
        }
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
        OnGamePause?.Invoke(isGameStopped);
    }

    public void ToggleGameStop()
    {
        this.isGameStopped = !this.isGameStopped;
        SetGameStopped(this.isGameStopped);
    }

    public void PlayEvent(GameEvent gameEvent)
    {
        OnGameEvent?.Invoke(gameEvent);
    }

    public static bool GetKeyDown(KeyCode keyCode)
    {
        return Input.GetKeyDown(keyCode) && !Managers.game.IsGameStopped;
    }

    public float UnscaledDeltaTime
    {
        get => this.isGameStopped ? 0.0f : Time.unscaledDeltaTime;
    }

    public bool IsGameStopped
    {
        get => this.isGameStopped;
    }

    public bool IsChangingScene
    {
        get => this.isChangingScene;
    }
    

}
