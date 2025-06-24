using System;
using System.Collections;
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
    public static event Action<PlayerState> OnStatusChange;

    [SerializeField]
    private PlayerState currentState = PlayerState.NORMAL;

    [SerializeField]
    private Transform playerTransform;

    private IsometricPlayerController controller;

    private ConeVision coneVision;

    private PlayerAnimator animator;

    public void Startup()
    {
        StartCoroutine(StartupState());
        this.controller = this.playerTransform.GetComponent<IsometricPlayerController>();
        this.coneVision = this.playerTransform.GetComponentInChildren<ConeVision>();
        this.animator = this.playerTransform.GetComponent<PlayerAnimator>();
    }

    void OnValidate()
    {
        if (this.isActiveAndEnabled)
            StartCoroutine(StartupState());
    }

    public void SetStatus(PlayerState playerState)
    {
        this.currentState = playerState;
        OnStatusChange?.Invoke(playerState);
    }

    //this is to ensure at startup of this manager all listeners will receive the initial state
    private IEnumerator StartupState()
    {
        yield return new WaitForEndOfFrame();
        SetStatus(this.currentState);
    }

    public bool IsState(PlayerState playerState)
    {
        return playerState == this.currentState;
    }

    public PlayerState CurrentState
    {
        get => currentState;
    }

    public IsometricPlayerController Controller
    {
        get => this.controller;
    }

    public ConeVision ConeVision
    {
        get => this.coneVision;
    }

    public PlayerAnimator Animator
    {
        get => this.animator;
    }
}