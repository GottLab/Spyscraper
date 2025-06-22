using UnityEngine;

public class HighlightTrigger : MonoBehaviour
{
    /*
    This script is attached to the highlight prefab
    What it does is detect if the player is inside the trigger
    */


    public ConeVision highlightVision;


    public ConeVisionObject itemVisionObject;
    public Outline outline;


    void OnTriggerStay(Collider other)
    {

        bool highlighted = highlightVision.CheckVision(itemVisionObject);
        
        if (highlighted != outline.enabled)
            outline.enabled = highlighted;
    }

    void OnTriggerExit(Collider other)
    {
        outline.enabled = false;
    }

    bool IsPlayer(Collider collider)
    {
        return collider.CompareTag("Player");
    }
}
