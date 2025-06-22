using UnityEngine;
using UnityEngine.Events;

public class InteractableItem : MonoBehaviour
{
    /*
    Module that handles the interaction with an item, showing the prompt and calling the related function
    */

    private InteractionPrompt prompt;
    [SerializeField] private UnityEvent OnInteract;

    void Start()
    {
        prompt = this.GetComponentInChildren<InteractionPrompt>();
    }

    void OnTriggerEnter(Collider other) // the trigger interacts only with player layer colliders
    {
        prompt.ShowPrompt();
    }

    void OnTriggerExit(Collider other) // the trigger interacts only with player layer colliders
    {
        prompt.HidePrompt();
    }

    void OnTriggerStay()
    {
        if (Input.GetKeyDown(KeyCode.F) && Managers.playerManager.IsState(PlayerManager.PlayerState.NORMAL))
        {
            OnInteract?.Invoke();
        }
    }
}
