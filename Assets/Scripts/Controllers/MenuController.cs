using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

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
    }

    public void OpenSettings()
    {
        isSettingsOpen = true;
        MoveCameraTo(settingsLookPoint);
        Debug.Log("Settings button clicked!");
    }

    public void HowToPlay()
    {
        Debug.Log("How to Play button clicked!");
        MoveCameraTo(howToPlayLookPoint);
        isHowToPlayOpen = true;
    }
    
    public void BackToMenu()
    {
        Debug.Log("Back to Menu button clicked!");
        // We need this in order to avoid the settings animator to play the disappearing animation on spawn
        settingsAnimator.SetBool("IsSpawn", false);
        if (isSettingsOpen) {
            
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