using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour, IGameManager {

    /*
    Manager that controls the UI of the inventory of the player
    */

    [System.Serializable]
    public struct UI_box {
        public Image image;
        public Text quantity;
    }

    [SerializeField] private UI_box TL; // the TOP-LEFT
    [SerializeField] private UI_box TR; // the TOP-RIGHT
    [SerializeField] private UI_box BL; // the BOTTOM-LEFT
    [SerializeField] private UI_box BR; // the BOTTOM-RIGHT

    public ManagerStatus status { get; private set; }
    private Dictionary<UI_box, string> _invetory;

    public void Startup() {
        Debug.Log("Inventory manager starting ...");

        _invetory = new Dictionary<UI_box, string>();
        _invetory[TL] = null;
        _invetory[TR] = null;
        _invetory[BL] = null;
        _invetory[BR] = null;

        status = ManagerStatus.Started;
    }

    public void AddNewItem(ItemData data, int qta) {
        foreach (UI_box item in _invetory.Keys) {
            if (_invetory[item] == null) {  // we find the first empty slot
                item.image.sprite = data.sprite; // update the sprite
                item.quantity.text = qta.ToString(); // update the quantity

                item.image.gameObject.SetActive(true);
                item.quantity.gameObject.SetActive(true);

                _invetory[item] = data.itemName;
                break;
            }
        }
    }

    public void AddExistingItem(ItemData data, int qta) {
        foreach (UI_box item in _invetory.Keys) {
            if (_invetory[item] != null && _invetory[item].Equals(data.itemName)) { // we find the solt
                item.image.sprite = data.sprite;
                item.quantity.text = qta.ToString(); // update the quantity
                break;
            }
        }
    }

    public void RemoveItem(ItemData data) {
        foreach (UI_box item in _invetory.Keys) {
            if (_invetory[item] != null && _invetory[item].Equals(data.itemName)) {
                _invetory[item] = null;
                
                item.image.gameObject.SetActive(false);
                item.quantity.gameObject.SetActive(false);
            }
        }
    }
}
