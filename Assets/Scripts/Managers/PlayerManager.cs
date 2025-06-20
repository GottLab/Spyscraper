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

    private ConeVision playerConeVision;

    public void Startup()
    {
        StartCoroutine(StartupState());

        playerConeVision = playerTransform.GetComponentInChildren<ConeVision>();

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

    public Transform PlayerTransform
    {
        get => playerTransform;
    }
    
    public ConeVision ConeVision
    {
        get => playerConeVision;
    }
}