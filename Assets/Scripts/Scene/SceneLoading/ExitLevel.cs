using UnityEngine;

public class ExitLevel : MonoBehaviour
{


    public void GoNextLevel(string nextLevel)
    {
        Managers.playerManager.Controller.movementLock.SetLock("goingNextLevel", true);
        Managers.game.TransitionScene(nextLevel);
    }
}
