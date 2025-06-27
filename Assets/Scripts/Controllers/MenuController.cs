using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform howToPlayLookPoint;
    public Transform menuLookPoint;
    
    public float cameraTransitionDuration = 1f;
    
    public Transform settingsLookPoint;
    public Animator settingsAnimator;
    
    private bool isHowToPlayOpen = false;
    private bool isSettingsOpen = false; 
    
    private Coroutine transitionRoutine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isHowToPlayOpen || isSettingsOpen)
            {
                BackToMenu();
            }
            else
            {
                // TODO: IMPLEMENT PROMPT OR REMOVE THIS
                // QuitGame();
            }
        }
    }

    public void PlayGame()
    {
        Managers.game.TransitionScene("Tutorial");
        DeselectButton();
    }

    public void OpenSettings()
    {
        isSettingsOpen = true;
        MoveCameraTo(settingsLookPoint);
        Debug.Log("Settings button clicked!");
        DeselectButton();
    }

    public void HowToPlay()
    {
        Debug.Log("How to Play button clicked!");
        MoveCameraTo(howToPlayLookPoint);
        isHowToPlayOpen = true;
        DeselectButton();
    }
    
    public void BackToMenu()
    {
        Debug.Log("Back to Menu button clicked!");
        // We need this in order to avoid the settings animator to play the disappearing animation on spawn
        if (isSettingsOpen) {
            settingsAnimator.SetBool("IsSpawn", false);
            settingsAnimator.SetBool("IsOpen", false);
        }
        isSettingsOpen = false;
        isHowToPlayOpen = false;
        MoveCameraTo(menuLookPoint);
    }

    private void MoveCameraTo(Transform target)
    {
        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(MoveCameraRoutine(target));
    }

    private IEnumerator MoveCameraRoutine(Transform target)
    {
        // Store starting position & rotation
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        // Get target position & rotation
        Vector3 targetPos = target.position;
        Quaternion targetRot = target.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / cameraTransitionDuration;
            cameraTransform.position = Vector3.Lerp(startPos, targetPos, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        cameraTransform.position = targetPos;
        cameraTransform.rotation = targetRot;

        if (isSettingsOpen) {
            settingsAnimator.SetBool("IsOpen", true);
        }
        DeselectButton();
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

    public void DeselectButton()
    {
        Debug.Log("The currently selected button is: " + EventSystem.current.currentSelectedGameObject);
        EventSystem.current.SetSelectedGameObject(null);
        Debug.Log("button deselected, current is: " + EventSystem.current.currentSelectedGameObject);
    }
}