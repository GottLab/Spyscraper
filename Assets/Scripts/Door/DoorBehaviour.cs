using UnityEditor.EditorTools;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{

    public void OpenDoor()
    {
        this.gameObject.SetActive(false);
    }

}
