using UnityEditor.EditorTools;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{

    [SerializeField, Tooltip("Required Items to open this door")]
    private ItemStack[] requiredItems;

    public void InteractDoor()
    {
        if (Managers.Inventory.HasItemStacks(this.requiredItems))
        {
            foreach (ItemStack stack in requiredItems)
            {
                Managers.Inventory.ConsumeItem(stack.item, stack.count);
            }

            print("OPEN!");

            Destroy(this.gameObject);
        }
        else
        {
            print("NOPE");
        }
    }

}
