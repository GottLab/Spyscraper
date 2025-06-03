using UnityEngine;

public class TriggerState : MonoBehaviour
{
    public PlayerManager.PlayerState playerState;


    void OnTriggerEnter(Collider other)
    {
        Managers.playerManager.SetStatus(this.playerState);
    }
}