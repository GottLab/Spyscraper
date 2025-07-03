
using UnityEngine;
using static LoopObject;

[ExecuteInEditMode]
public class PositionObjectByStep : MonoBehaviour
{
    [SerializeField, Tooltip("Stairs info needed to place object")]
    private StairsInfo stairsInfo;

    public float step = 0;

    void OnValidate()
    {
        Vector3 direction = new Vector3(0, stairsInfo.StepHeight, stairsInfo.StepDepth) * step;
        this.transform.localPosition = direction;
    }
}