using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryManager : MonoBehaviour, IGameManager
{

    /*
        Manager that controls the inventory of the player
    */

    public ManagerStatus status { get; private set; }
    private Dictionary<ItemData, int> _items;

    public Action<ItemData, int> OnAddNewItem; // manages the addition of a new item to the inventory
    public Action<ItemData, int> OnSetItemQuantity; // manages the changes of the quantity of an item in the inventory
    public Action<ItemData> OnRemoveItem; // manages the remotion of an item from the inventory


    public void Startup()
    {
        Debug.Log("Inventory manager starting ...");
        _items = new Dictionary<ItemData, int>();
        status = ManagerStatus.Started;
    }

    public bool AddItem(ItemData data, int count = 1)
    {
        if (_items.ContainsKey(data))
        {
            _items[data] += count;
            Debug.Log(_items[data]);
            OnSetItemQuantity?.Invoke(data, _items[data]);
        }
        else
        {
            _items[data] = count;
            OnAddNewItem?.Invoke(data, _items[data]);
        }
        DisplayItems();
        return true;
    }

    public bool ConsumeItem(ItemData item, int count = 1)
    {

        if (!_items.ContainsKey(item))
        {
            return false;
        }

        _items[item] -= count;
        if (_items[item] <= 0)
        {
            _items.Remove(item);
            OnRemoveItem?.Invoke(item);
        }
        else
        {
            OnSetItemQuantity?.Invoke(item, _items[item]);
        }

        DisplayItems();
        return true;
    }


    private void DisplayItems()
    {
        string itemDisplay = "List of Items: ";
        foreach (KeyValuePair<ItemData, int> item in _items)
        {
            itemDisplay += item.Key.itemName + "(" + item.Value + ") ";
        }
    }

    public List<string> GetItemList()
    {
        List<string> list = new List<string>();
        foreach (ItemData item in _items.Keys)
        {
            list.Add(item.itemName);
        }
        return list;
    }

    public int GetItemCount(ItemData data)
    {
        if (_items.ContainsKey(data))
        {
            return _items[data];
        }
        return 0;
    }
    
    public bool HasItemStacks(ItemStack[] items)
    {
        foreach (ItemStack stack in items)
        {
            if (Managers.Inventory.GetItemCount(stack.item) < stack.count)
            {
                return false;
            }
        }
        return true;
    }
}
