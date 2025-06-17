using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour, IGameManager {

    [SerializeField] private Image item_TL; // the TOP-LEFT item
    [SerializeField] private Image item_TR; // the TOP-RIGHT item
    [SerializeField] private Image item_BL; // the BOTTOM-LEFT item
    [SerializeField] private Image item_BR; // the BOTTOM-RIGHT item

    [SerializeField] private Text quantity_TL; // the TOP-LEFT quantity
    [SerializeField] private Text quantity_TR; // the TOP-RIGHT quantity
    [SerializeField] private Text quantity_BL; // the BOTTOM-LEFT quantity
    [SerializeField] private Text quantity_BR; // the BOTTOM-RIGHT quantity

    public ManagerStatus status { get; private set; }
    private Dictionary<Image, Text> _UImap;
    private Dictionary<Image, string> _invetory;

    public void Startup() {
        Debug.Log("Inventory manager starting ...");

        _UImap = new Dictionary<Image, Text>();
        _UImap[item_TL] = quantity_TL;
        _UImap[item_TR] = quantity_TR;
        _UImap[item_BL] = quantity_BL;
        _UImap[item_BR] = quantity_BR;

        _invetory = new Dictionary<Image, string>();
        _invetory[item_TL] = null;
        _invetory[item_TR] = null;
        _invetory[item_BL] = null;
        _invetory[item_BR] = null;

        status = ManagerStatus.Started;
    }




    public void AddNewItem(ItemData data, int qta) {
        foreach (Image item in _invetory.Keys) {
            if (_invetory[item] == null) {  // we find the first empty slot
                item.sprite = data.sprite; // update the sprite
                _UImap[item].text=qta.ToString(); // update the quantity

                item.gameObject.SetActive(true);
                _UImap[item].gameObject.SetActive(true);

                _invetory[item] = data.itemName;
                break;
            }
        }
    }

    public void AddExistingItem(ItemData data, int qta) {
        foreach (Image item in _invetory.Keys) {
            if (_invetory[item] != null && _invetory[item].Equals(data.itemName)) { // we find the solt
                item.sprite = data.sprite;
                _UImap[item].text = qta.ToString(); // update the quantity
                break;
            }
        }
    }

    public void RemoveItem(ItemData data) {
        foreach (Image item in _invetory.Keys) {
            if (_invetory[item] != null && _invetory[item].Equals(data.itemName)) {
                _invetory[item] = null;
                _UImap[item].text = null;
                
                item.gameObject.SetActive(false);
                _UImap[item].gameObject.SetActive(false);
            }
        }
    }
}
