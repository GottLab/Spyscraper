using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSound : UIInteractionSound
{
    private Toggle toggle;

    public void Start()
    {
        this.toggle = GetComponent<Toggle>();
        this.toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void OnDestroy()
    {
        this.toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    void OnToggleChanged(bool toggled)
    {
        PlayConfirmSound();
    }
}

    
