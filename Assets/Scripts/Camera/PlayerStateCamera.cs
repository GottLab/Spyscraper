using QTESystem;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerStateCamera : MonoBehaviour
{
    [SerializeField]
    private CinemachineBrain cinemachineBrain;

    [SerializeField]
    private CinemachineVirtualCameraBase normalCamera;

    [SerializeField]
    private CinemachineVirtualCameraBase qteCamera;

    [SerializeField]
    private CinemachineVirtualCameraBase deathCamera;

    private CinemachineVirtualCameraBase currentCamera;

    //this is to store if before game pause the brain had ignore timescale
    private bool prevIgnoreTimescale;


    void Awake()
    {
        SetupCameras();
    }

    void OnEnable()
    {
        PlayerManager.OnStatusChange += OnPlayerStateChange;
        QTEManager.OnQteSequenceStart += OnQteSequenceStart;
        GameManager.OnGamePause += OnGameStop;
    }

    void OnDisable()
    {
        PlayerManager.OnStatusChange -= OnPlayerStateChange;
        QTEManager.OnQteSequenceStart -= OnQteSequenceStart;
        GameManager.OnGamePause -= OnGameStop;
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

    void OnGameStop(bool stopped)
    {
        if (stopped)
        {
            prevIgnoreTimescale = this.cinemachineBrain.IgnoreTimeScale;
            this.cinemachineBrain.IgnoreTimeScale = false;
        }
        else
        {
            this.cinemachineBrain.IgnoreTimeScale = this.prevIgnoreTimescale;
        }
    }

    void SetupCameras()
    {
        var player = GameObject.FindWithTag("Player");

        if (!player)
        {
            Debug.LogWarning("Player not found!, Cameras will not work", this);
            return;
        }


        var cameras = this.GetComponentsInChildren<CinemachineCamera>(true);

        foreach (var camera in cameras)
        {
            if (camera.Target.TrackingTarget == null)
            {
                camera.Target.TrackingTarget = player.transform;
            }
        }

        var qteTargetGroup = this.GetComponentInChildren<CinemachineTargetGroup>(true);

        qteTargetGroup.Targets.Clear();
        qteTargetGroup.AddMember(player.transform, 1.0f, 1.0f);
    }

    void OnQteSequenceStart(IQtePlayer target)
    {
        if (this.qteCamera is CinemachineCamera cinemachineCamera)
        {
            cinemachineCamera.Target.LookAtTarget = target.GetTransform();
        }
    }
}
