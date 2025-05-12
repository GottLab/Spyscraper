using UnityEngine;
using System.Collections;

public class CollectableItem : MonoBehaviour {

    /*
        (Tral)
        Module that has to be attached to item in order to collect them when the player enters the trigger
        TODO: implement the item interaction logic (a prompt appears when entering the trigger,
                if the player presses the button then it takes the item)
    */

    [SerializeField] private string itemName;

    void OnTriggerEnter(Collider other) {
        if (other.GetComponent<CharacterController>()) {
            Debug.Log("Item collected: "+itemName);
            if (Managers.Inventory.AddItem(itemName)) {
                Destroy(this.gameObject);
            }
        }
    }
}
