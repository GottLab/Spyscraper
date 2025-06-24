using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform howToPlayLookPoint;
    public Transform menuLookPoint;
    public float transitionDuration = 1f;

    private Coroutine transitionRoutine;

    public void PlayGame()
    {
        Managers.game.TransitionScene("Tutorial");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button clicked!");
    }

    public void HowToPlay()
    {
        Debug.Log("How to Play button clicked!");
        MoveCameraTo(howToPlayLookPoint);
    }
    
    public void BackToMenu()
    {
        Debug.Log("Back to Menu button clicked!");
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
            t += Time.deltaTime / transitionDuration;
            cameraTransform.position = Vector3.Lerp(startPos, targetPos, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        cameraTransform.position = targetPos;
        cameraTransform.rotation = targetRot;
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