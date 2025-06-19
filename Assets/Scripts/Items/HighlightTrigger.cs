using UnityEngine;

public class HighlightTrigger : MonoBehaviour {
    /*
    This script is attached to the highlight prefab
    What it does is detect if the player is inside the trigger
    */

    void OnTriggerStay(Collider other) {
        if (other.GetComponent<CharacterController>()) {
            CollectableItem collectableItem = gameObject.GetComponentInParent<CollectableItem>();
            collectableItem.HighlightOn();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.GetComponent<CharacterController>()) {
            CollectableItem collectableItem = gameObject.GetComponentInParent<CollectableItem>();
            collectableItem.HighlightOff();
        }
    }
}
