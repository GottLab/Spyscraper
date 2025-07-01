using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownSound : UIInteractionSound
{
    private TMP_Dropdown dropdown;

    public void Start()
    {
        this.dropdown = GetComponent<TMP_Dropdown>();
        this.dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    public void OnDestroy()
    {
        this.dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
    }

    void OnDropdownChanged(int value)
    {
        PlayConfirmSound();
    }
}

    
