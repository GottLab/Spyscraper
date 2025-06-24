using System.Collections;
using Enemy;
using QTESystem;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(StateEnemyAI))]
public class TutorialEnemy : MonoBehaviour
{

    [SerializeField, Tooltip("Point to teleport player when loses to this enemy")]
    private Transform spawnpoint;

    public void OnEnemyWon()
    {
        StartCoroutine(HandlePlayerDefeat());
    }

    public void OnEnemyLost()
    {
        print("PLAYER WON");
    }

    IEnumerator HandlePlayerDefeat()
    {

        yield return new WaitUntil(() => Managers.playerManager.IsState(PlayerManager.PlayerState.DIED));
        yield return new WaitForSeconds(2.0f);

        Transform playerTransform = Managers.playerManager.PlayerTransform;

        Managers.playerManager.SetStatus(PlayerManager.PlayerState.NORMAL);

        //teleport player
        playerTransform.position = new Vector3(this.spawnpoint.position.x, spawnpoint.position.y, this.spawnpoint.position.z);

    }
}
