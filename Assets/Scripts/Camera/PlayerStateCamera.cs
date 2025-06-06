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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Managers.playerManager.OnStatusChange += OnPlayerStataChange;
    }

    void OnDestroy()
    {
        Managers.playerManager.OnStatusChange -= OnPlayerStataChange;
    }

    void OnPlayerStataChange(PlayerManager.PlayerState playerState)
    {
        CinemachineVirtualCameraBase previousCamera = this.currentCamera;

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
        previousCamera?.gameObject.SetActive(false);

    }
}
