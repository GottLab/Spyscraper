using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class InteractionPrompt : MonoBehaviour
{
    /*
    This module manages the UI prompt
    */

    private Canvas canvas;
    private Text text;
    [SerializeField] private bool followCamera;

    private bool wasShowing = false;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        this.canvas.worldCamera = Camera.main;

        PlayerManager.OnStatusChange += OnPlayerStatusChange;

        text = GetComponentInChildren<Text>();
        HidePrompt();
    }

    void OnDestroy()
    {
        PlayerManager.OnStatusChange -= OnPlayerStatusChange;
    }

    void Update()
    {
        if (IsShowing)
        {
            this.transform.rotation = Quaternion.LookRotation(this.canvas.worldCamera.transform.position - this.transform.position);
        }
    }

    public void ShowPrompt()
    {
        if (Managers.playerManager.IsState(PlayerManager.PlayerState.NORMAL))
        {
            this.text.gameObject.SetActive(true);
        }
    }

    public void HidePrompt()
    {
        this.text.gameObject.SetActive(false);
    }

    private void OnPlayerStatusChange(PlayerManager.PlayerState status)
    {

        if (status != PlayerManager.PlayerState.NORMAL)
        {
            if (IsShowing)
                wasShowing = true;
            HidePrompt();
        }
        else if (wasShowing)
        {
            ShowPrompt();
            wasShowing = false;
        }
    }
    
    public bool IsShowing
    {
        get => this.text.gameObject.activeSelf;
    }
}
