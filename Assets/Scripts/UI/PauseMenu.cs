using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class PauseMenu : MonoBehaviour
{

    private Canvas pauseCanvas;


    void Start()
    {
        this.pauseCanvas = this.GetComponent<Canvas>();
        GameManager.OnGamePause += ShowPauseMenu;
    }

    void OnDestroy()
    {
        GameManager.OnGamePause -= ShowPauseMenu;
    }

    void ShowPauseMenu(bool paused)
    {
        this.pauseCanvas.enabled = paused;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.game.ToggleGameStop();
        }
    }


}
