using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInteractionSound : MonoBehaviour, IMoveHandler
{

    public virtual void OnMove(AxisEventData eventData)
    {
        Managers.audioManager.uiNavigationAudio.PlayAudio();
    }

    protected void PlayConfirmSound()
    {
        Managers.audioManager.uiConfirmAudio.PlayAudio();
    }

}

    
