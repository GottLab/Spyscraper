using UnityEngine;

public class CollectibleItem : MonoBehaviour
{

    /*
        Module that implements the logic of the collection of the item
    */

    [SerializeField] private ItemData itemData;

    public void HandleInteract()
    {
        Debug.Log("Item collected: " + itemData.itemName);
        if (Managers.Inventory.AddItem(itemData)) {
            Destroy(this.gameObject);
        }
    }
}
