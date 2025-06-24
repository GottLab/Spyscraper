using UnityEngine;

public class MenuController : MonoBehaviour
{

    public void PlayGame()
    {
        Managers.game.TransitionScene("Tutorial");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button clicked!");
    }

    public void QuitGame()
    {
        Debug.Log("Quit button clicked!");

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}