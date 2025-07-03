using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{

    /*
    Manager that controls the UI of the inventory of the player
    */

    [System.Serializable]
    public struct UI_box
    {
        public Image image;
        public Text quantity;
    }

    [SerializeField] private UI_box TL; // the TOP-LEFT
    [SerializeField] private UI_box TR; // the TOP-RIGHT
    [SerializeField] private UI_box BL; // the BOTTOM-LEFT
    [SerializeField] private UI_box BR; // the BOTTOM-RIGHT

    private Dictionary<UI_box, ItemData> _invetory;

    public void Start()
    {

        _invetory = new Dictionary<UI_box, ItemData>();
        _invetory[TL] = null;
        _invetory[TR] = null;
        _invetory[BL] = null;
        _invetory[BR] = null;

        Managers.Inventory.OnAddNewItem += AddItem;
        Managers.Inventory.OnSetItemQuantity += ModifyExistingItem;
        Managers.Inventory.OnRemoveItem += RemoveItem;
    }

    void OnDestroy()
    {
        if (Managers.Inventory != null)
        {
            Managers.Inventory.OnAddNewItem -= AddItem;
            Managers.Inventory.OnSetItemQuantity -= ModifyExistingItem;
            Managers.Inventory.OnRemoveItem -= RemoveItem;
        }
    }

    private void SetQuantity(UI_box item, Sprite sprite, int qta)
    {
        item.image.sprite = sprite; // update the sprite
        item.quantity.text = qta.ToString(); // update the quantity
    }

    public void AddItem(ItemData data, int qta)
    {
        foreach (UI_box item in _invetory.Keys)
        {
            if (_invetory[item] == null)
            {  // we find the first empty slot
                SetQuantity(item, data.sprite, qta);

                item.image.gameObject.SetActive(true);
                item.quantity.gameObject.SetActive(true);

                _invetory[item] = data;
                break;
            }
        }
    }

    public void ModifyExistingItem(ItemData data, int qta)
    {
        foreach (UI_box item in _invetory.Keys)
        {
            if (_invetory[item] != null && _invetory[item].Equals(data))
            { // we find the solt
                SetQuantity(item, data.sprite, qta);
                break;
            }
        }
    }

    public void RemoveItem(ItemData data)
    {
        var items = _invetory.Keys.ToArray();
        foreach (UI_box item in items)
        {
            if (_invetory[item] != null && _invetory[item] == data)
            {
                _invetory[item] = null;

                item.image.gameObject.SetActive(false);
                item.quantity.gameObject.SetActive(false);
            }
        }
    }
}
