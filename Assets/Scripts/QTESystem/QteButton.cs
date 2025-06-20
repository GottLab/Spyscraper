using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class QteButton : MonoBehaviour
{
    private Image buttonImage;
    
    private Text buttonText;
    
    private Coroutine timerCoroutine;

    public void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>();
    }

    private IEnumerator StartButtonTimer(KeyCode keyCode, float time)
    {
        buttonText.text = keyCode.ToString();
        
        float timer = 0;
        
        while (timer < time)
        {
            timer += Time.unscaledDeltaTime;
            buttonImage.fillAmount = Mathf.Lerp(1.0f, 0.0f, timer / time);
            
            yield return null;
        }
    }

    public void StartTimer(KeyCode keyCode, float time)
    {
        timerCoroutine = StartCoroutine(StartButtonTimer(keyCode, time));
    }
    
    public void OnQteElementEnd(bool success)
    {
        if(timerCoroutine != null)
            StopCoroutine(timerCoroutine);
    }

    
}
