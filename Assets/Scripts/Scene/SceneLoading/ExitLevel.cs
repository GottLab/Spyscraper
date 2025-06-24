using System.Collections;
using UnityEngine;

public class ExitLevel : MonoBehaviour
{


    public void GoNextLevel(string nextLevel)
    {
        Managers.playerManager.Controller.SetMovement("goingNextLevel", true);
        Managers.game.TransitionScene(nextLevel);
    }
}
