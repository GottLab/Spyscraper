using System;
using System.Collections.Generic;
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

    private HashSet<KeyCode> ConsumedKeys = new();

    public void Startup()
    {
        SetGameStopped(false);
    }

    void LateUpdate()
    {
        this.ConsumedKeys.Clear();
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

    //used to prevent for example that pressing with return in the pause menu will unpause the game AND skip dialogue in one frame
    public void ConsumeSubmit()
    {
        ConsumeKey(KeyCode.Return);
    }

    public static bool GetKeyDown(KeyCode keyCode)
    {
        return !Managers.game.IsGameStopped && Input.GetKeyDown(keyCode) && !Managers.game.ConsumedKeys.Contains(keyCode);
    }

    //when a key is consumed future calls of GetKeyDown in the current frame will fail to prevent other inputs
    public static void ConsumeKey(KeyCode keyCode)
    {
        Managers.game.ConsumedKeys.Add(keyCode);
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