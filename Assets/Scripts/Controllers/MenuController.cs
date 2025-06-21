using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void PlayGame()
    {
        if (gameManager != null)
        {
            gameManager.TransitionScene("Tutorial");
        }
        else
        {
            Debug.LogError("GameManager reference is missing!");
        }
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