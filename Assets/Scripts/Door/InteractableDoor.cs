using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;


[RequireComponent(typeof(DoorBehaviour))]
public class InteractableDoor : MonoBehaviour
{

    
    private DoorBehaviour doorBehaviour;

    [SerializeField]
    private DialogueHandler failOpenDialogue;

    [SerializeField]
    private DialogueHandler successOpenDialogue;

    
    [SerializeField, Tooltip("Required Items to open this door")]
    private ItemStack[] requiredItems;

    void Start()
    {
        this.doorBehaviour = GetComponent<DoorBehaviour>();
    }

    public void InteractDoor()
    {
        if (Managers.Inventory.HasItemStacks(this.requiredItems))
        {
            foreach (ItemStack stack in requiredItems)
            {
                Managers.Inventory.ConsumeItem(stack.item, stack.count);
            }
            this.doorBehaviour.OpenDoor();
            this.successOpenDialogue?.StartDialogue();
        }
        else
        {
            this.failOpenDialogue?.StartDialogue();
        }
    }

    public void OpenDoor()
    {
        Destroy(this.gameObject);
    }

}
