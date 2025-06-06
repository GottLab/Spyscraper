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
    public event Action<PlayerState> OnStatusChange;

    [SerializeField]
    private PlayerState currentState = PlayerState.NORMAL;

    public void Startup()
    {
    }

    void Start()
    {
        this.SetStatus(this.currentState);
    }

    void OnValidate()
    {
        this.SetStatus(this.currentState);
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