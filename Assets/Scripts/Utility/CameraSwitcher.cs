using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CinemachineBrain))]
public class CameraSwitcher : MonoBehaviour
{
    private CinemachineBrain brain;


    void Start()
    {
        this.brain = GetComponent<CinemachineBrain>();

    }

    public void SwitchCamera(GameObject endCamera)
    {
        SetPriority(brain.ActiveVirtualCamera, 0);

        ICinemachineCamera targetCamera = endCamera.GetComponent<ICinemachineCamera>();

        SetPriority(targetCamera, 1);
    }

    private void SetPriority(ICinemachineCamera cinemachineCamera, int priority)
    {

        if (cinemachineCamera is CinemachineCamera camera)
        {
            camera.Priority = priority;
        }
        else if (cinemachineCamera is CinemachineCameraManagerBase managerBase)
        {
            managerBase.Priority = priority;
        } 
    }
}
