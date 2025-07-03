using System.Collections;
using Enemy;
using UnityEngine;

[RequireComponent(typeof(StateEnemyAI))]
public class TutorialEnemy : MonoBehaviour
{

    [SerializeField, Tooltip("Point to teleport player when loses to this enemy")]
    private Transform spawnpoint;

    [SerializeField, Tooltip("Dialogue When Player Wins")]
    private DialogueHandler DialogueWin;

    
    [SerializeField, Tooltip("Dialogue When Player Loses")]
    private DialogueHandler DialogueLose;

    public void OnEnemyWon()
    {
        StartCoroutine(HandlePlayerDefeat());
    }

    public void OnEnemyLost()
    {
        DialogueWin?.StartDialogue();
    }

    IEnumerator HandlePlayerDefeat()
    {

        yield return new WaitUntil(() => Managers.playerManager.IsState(PlayerManager.PlayerState.DIED));

        DialogueLose?.StartDialogue();

        yield return new WaitForSeconds(1.6f);

        Transform playerTransform = Managers.playerManager.PlayerTransform;

        Managers.playerManager.SetStatus(PlayerManager.PlayerState.NORMAL);

        //teleport player
        playerTransform.position = new Vector3(this.spawnpoint.position.x, spawnpoint.position.y, this.spawnpoint.position.z);
        Physics.SyncTransforms();



    }
}
