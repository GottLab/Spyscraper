using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PlayerTrigger : MonoBehaviour
{
    private Collider collider;


    [SerializeField, Tooltip("If it should trigger again if entering/exit again")]
    private bool retrigger = false;

    [SerializeField]
    private UnityEvent OnPlayerEnter;

    [SerializeField]
    private UnityEvent OnPlayerExit;

    private bool triggered_enter = false;
    private bool triggered_exit = false;

    void Awake()
    {
        this.collider = GetComponent<Collider>();
        this.collider.isTrigger = true;
        int playerOnlyMask = LayerMask.NameToLayer("OnlyPlayer");
        //exclude every other layer except the player
        this.gameObject.layer = playerOnlyMask;
    }

    void OnTriggerEnter(Collider other)
    {
        if (this.retrigger || !this.triggered_enter)
            OnPlayerEnter?.Invoke();
        triggered_enter = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (this.retrigger || !this.triggered_exit)
            OnPlayerExit?.Invoke();
        triggered_exit = true;
    }

}
