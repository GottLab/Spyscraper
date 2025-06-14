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
}
