using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    [SerializeField, Tooltip("Canvas to activate/deactivate when game is paused")]
    private Canvas pauseCanvas;


    void Start()
    {
        GameManager.OnGamePause += ShowPauseMenu;
    }

    void OnDestroy()
    {
        GameManager.OnGamePause -= ShowPauseMenu;
    }

    void ShowPauseMenu(bool paused)
    {
        this.pauseCanvas.gameObject.SetActive(paused);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.game.ToggleGameStop();
        }
    }


}
