using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    /*
    This is the scriptable obect for a general item
    */
    public Sprite sprite;
    public string itemName;
    public UnityEvent onConsume;
}
