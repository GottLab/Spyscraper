using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IGameManager {

    /*
        Manager that controls the inventory of the player
    */

    public ManagerStatus status {get; private set;}
    private Dictionary<string, int> _items;

    // try for the ui (can carry max 4 item)
    [SerializeField] private Image item_1;
    [SerializeField] private Text quantity_1;
    [SerializeField] private Sprite gold;

    public void Startup() {
        Debug.Log("Inventory manager starting ...");
        _items = new Dictionary<string, int>();
        status = ManagerStatus.Started;
    }

    public bool AddItem(string name) {
        if (_items.ContainsKey(name)) {
            _items[name] += 1;
        } else {
            _items[name] = 1;
        }
        UpdateItemUI(name);
        DisplayItems();
        return true;
    }

    private void UpdateItemUI(string name) {
        if (_items.ContainsKey(name)) {
            if (_items[name] == 1) {
                item_1.sprite = gold;
                quantity_1.text = _items[name].ToString();

                item_1.gameObject.SetActive(true);
                quantity_1.gameObject.SetActive(true);
            } else {
                quantity_1.text = _items[name].ToString();
            }
        }
    }

    private void DisplayItems() {
        string itemDisplay = "List of Items: ";
        foreach (KeyValuePair<string, int> item in _items) {
            itemDisplay += item.Key + "(" + item.Value + ") ";
        }
        Debug.Log(itemDisplay);
    }

    public List<string> GetItemList() {
        List<string> list = new List<string>(_items.Keys);
        return list;
    }

    public int GetItemCount(string name) {
        if (_items.ContainsKey(name)) {
            return _items[name];
        }
        return 0;
    }

    public void ConsumeItem(string name) {
        if (_items.ContainsKey(name)) {
            _items[name]--;
            if (_items[name] == 0) {
                _items.Remove(name);
            }
        } else {
            Debug.Log("cannot consume " + name);
        }
        UpdateItemUI(name);
        DisplayItems();
    }
}
