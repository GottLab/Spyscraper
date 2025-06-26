using UnityEngine;

public class HandleDeath : MonoBehaviour
{


    public void RestartLevel()
    {
        Managers.game.ReloadScene();
    }
}
