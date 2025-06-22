using System.Collections;
using UnityEngine;

public class ExitLevel : MonoBehaviour
{


    public void GoNextLevel(string nextLevel)
    {
        StartCoroutine(Yest(nextLevel));    
    }

    private IEnumerator Yest(string nextLevel)
    {
        Managers.playerManager.Controller.SetMovement("goingNextLevel", true);
        yield return new WaitForSeconds(2.0f);
        Managers.game.TransitionScene(nextLevel);
    }
}
