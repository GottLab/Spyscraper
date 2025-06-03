using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public enum PlayerState
    {
        NORMAL,
        QTE,
        DIED
    }


    public ManagerStatus status => ManagerStatus.Started;
    public Action<PlayerState> OnStatusChange;

    private PlayerState currentState;

    public void Startup()
    {
        this.SetStatus(PlayerState.NORMAL);
    }

    public void SetStatus(PlayerState playerState)
    {
        this.currentState = playerState;
        this.OnStatusChange?.Invoke(playerState);
    }

    public bool IsState(PlayerState playerState)
    {
        return playerState == this.currentState;
    }

    public PlayerState CurrentState
    {
        get => currentState;
    }
}