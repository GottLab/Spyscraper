using UnityEngine;
using System.Collections;
using UnityEngine.ProBuilder.Shapes;

public class CollectableItem : MonoBehaviour
{

    /*
        (Trial)
        Module that has to be attached to item in order to collect them when the player enters the trigger
        TODO: implement the item interaction logic (a prompt appears when entering the trigger,
                if the player presses the button then it takes the item)
    */

    [SerializeField] private ItemData itemData;

    void OnTriggerEnter(Collider other) {

        Debug.Log("Item collected: " + itemData.itemName);
        if (Managers.Inventory.AddItem(itemData)) {
            Destroy(this.gameObject);
        }
        
    }
}
