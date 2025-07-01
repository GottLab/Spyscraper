using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : UIInteractionSound
{
    private Button button;

    public void Start()
    {

        this.button = GetComponent<Button>();
        this.button.onClick.AddListener(PlayConfirmSound);
    }

    public void OnDestroy()
    {
        this.button.onClick.RemoveListener(PlayConfirmSound);
    }
}

    
