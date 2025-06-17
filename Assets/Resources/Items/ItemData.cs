using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    /*
    This is the scriptable obect for a general item
    */
    [SerializeField] public Sprite sprite;
    [SerializeField] public string itemName;
}
