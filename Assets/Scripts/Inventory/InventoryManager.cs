using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IGameManager {

    /*
        Manager that controls the inventory of the player
    */

    public ManagerStatus status {get; private set;}
    private Dictionary<ItemData, int> _items;


    [SerializeField]
    private InventoryUIManager inventoryUIManager;

    public void Startup()
    {
        Debug.Log("Inventory manager starting ...");
        _items = new Dictionary<ItemData, int>();
        status = ManagerStatus.Started;
    }

    public bool AddItem(ItemData data) {
        if (_items.ContainsKey(data)) {
            _items[data] += 1;
            Debug.Log(_items[data]);
            inventoryUIManager.AddExistingItem(data, _items[data]);
        } else {
            _items[data] = 1;
            inventoryUIManager.AddNewItem(data, 1);
        }
        DisplayItems();
        return true;
    }
    
    public void ConsumeItem(string name) {
        foreach (ItemData item in _items.Keys) {
            if (name.Equals(item.itemName)) {
                _items[item]--;
                if (_items[item] == 0) {
                    _items.Remove(item);
                    inventoryUIManager.RemoveItem(item);
                }
            } else {
                Debug.Log("cannot consume " + name);
            }
            break;
        }
        DisplayItems();
    }


    private void DisplayItems() {
        string itemDisplay = "List of Items: ";
        foreach (KeyValuePair<ItemData, int> item in _items) {
            itemDisplay += item.Key + "(" + item.Value + ") ";
        }
        Debug.Log(itemDisplay);
    }

    public List<string> GetItemList() {
        List<string> list = new List<string>();
        foreach (ItemData item in _items.Keys)
        {
            list.Add(item.itemName);
        }
        return list;
    }

    public int GetItemCount(string name) {
        foreach (ItemData item in _items.Keys) {
            if (name.Equals(item.itemName)) {
                return _items[item];
            }
            break;
        }
        return 0;
    }
}
