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

    private bool isGameStopped = false;
    private float prevTimeScale = 1.0f;

    private bool isChangingScene;

    public void Startup()
    {
        SetGameStopped(false);
    }

    public void TransitionScene(string toScene)
    {
        if (!this.IsChangingScene)
        {
            this.isChangingScene = true;
            MySceneManager.TransitionAsync(toScene, "LoadingScene");
        }
    }

    public void ReloadScene()
    {
        TransitionScene(MySceneManager.GetActiveScene().name);
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
        if (!this.IsChangingScene)
        {
            this.isGameStopped = !this.isGameStopped;
            SetGameStopped(this.isGameStopped);
        }
    }

    public void CloseGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void PlayEvent(GameEvent gameEvent)
    {
        OnGameEvent?.Invoke(gameEvent);
    }

    public static bool GetKeyDown(KeyCode keyCode)
    {
        return Input.GetKeyDown(keyCode) && !Managers.game.IsGameStopped;
    }

    public static bool GetKey(KeyCode keyCode)
    {
        return Input.GetKey(keyCode) && !Managers.game.IsGameStopped;
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