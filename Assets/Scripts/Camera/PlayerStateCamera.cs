using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerStateCamera : MonoBehaviour
{

    [SerializeField]
    private CinemachineVirtualCameraBase normalCamera;

    [SerializeField]
    private CinemachineVirtualCameraBase qteCamera;

    [SerializeField]
    private CinemachineVirtualCameraBase deathCamera;

    private CinemachineVirtualCameraBase currentCamera;


    void Start()
    {
        SetupCameras();
    }

    void OnEnable()
    {
        PlayerManager.OnStatusChange += OnPlayerStateChange;
    }

    void OnDisable()
    {
        PlayerManager.OnStatusChange -= OnPlayerStateChange;
    }

    void OnPlayerStateChange(PlayerManager.PlayerState playerState)
    {

        currentCamera?.gameObject.SetActive(false);

        switch (playerState)
        {
            case PlayerManager.PlayerState.NORMAL:
                currentCamera = normalCamera;
                break;
            case PlayerManager.PlayerState.QTE:
                currentCamera = qteCamera;
                break;
            case PlayerManager.PlayerState.DIED:
                currentCamera = deathCamera;
                break;
        }

        currentCamera.gameObject.SetActive(true);

    }

    void SetupCameras()
    {
        var player = GameObject.FindWithTag("Player");

        if (!player)
        {
            Debug.LogWarning("Player not found!, Cameras will not work", this);
            return;
        }

        var cameras = this.GetComponentsInChildren<CinemachineCamera>();

        foreach (var camera in cameras)
        {
            if (camera.Target.TrackingTarget == null)
            {
                camera.Target.TrackingTarget = player.transform;
            }

        }

        var qteTargetGroup = this.GetComponentInChildren<CinemachineTargetGroup>();

        qteTargetGroup.Targets.Clear();
        qteTargetGroup.AddMember(player.transform, 1.0f, 0.0f);
    }
}
