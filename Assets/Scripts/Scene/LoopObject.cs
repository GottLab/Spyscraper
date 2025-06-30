using System;
using UnityEngine;
using UnityEngine.Events;

public class LoopObject : MonoBehaviour
{

    [Serializable]
    public struct StairsInfo
    {
        public float depth;
        public float height;
        public float totalSteps;

        public readonly float StepDepth
        {
            get => depth / totalSteps;
        }

        public readonly float StepHeight
        {
            get => height / totalSteps;
        }

        public readonly float GetHeightRatio(float d)
        {
            return d * (height / this.depth);
        }
    }

    public Transform reference;

    [SerializeField, Tooltip("If it loops going negative")]
    private bool loopDown = true;

    [SerializeField, Tooltip("If it loops going posiitive")]
    private bool loopUp = true;

    [SerializeField, Tooltip("Stairs info needed to loop correctly")]
    private StairsInfo stairsInfo;

    [SerializeField, Tooltip("The distance after which it loops")]
    private float zLoop = 10;


    private int totalLoops = 0;

    public UnityEvent OnFirstLoop;

    // Update is called once per frame
    void Update()
    {
        float limitZ = this.stairsInfo.StepDepth * zLoop;
        float limitY = this.stairsInfo.GetHeightRatio(limitZ);

        Vector3 pos = reference.InverseTransformPoint(this.transform.position);

        if ((pos.y <= 0 && pos.z <= 0 && loopDown) || (pos.y >= limitY && pos.z >= limitZ && loopUp))
        {
            pos.y = Mathf.Repeat(pos.y, limitY);
            pos.z = Mathf.Repeat(pos.z, limitZ);
            this.transform.position = reference.TransformPoint(pos);
            Physics.SyncTransforms();
            totalLoops += 1;

            if (totalLoops == 1)
            {
                OnFirstLoop?.Invoke();
            }


        }
    }

}
