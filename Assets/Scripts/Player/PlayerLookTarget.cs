using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Rig))]
public class PlayerLookTarget : MonoBehaviour
{
    private Rig lookAtTargetRig;

    private MultiAimConstraint[] multiAimConstraints;


    public RigBuilder rigBuilder;

    void Start()
    {
        this.lookAtTargetRig = this.GetComponent<Rig>();
        this.multiAimConstraints = this.GetComponentsInChildren<MultiAimConstraint>();

        Managers.playerManager.OnStatusChange += OnStatusChange;


        foreach (MultiAimConstraint multiAimConstraint in this.multiAimConstraints)
        {
            WeightedTransformArray weightedTransforms = multiAimConstraint.data.sourceObjects;
            weightedTransforms.SetTransform(0, Managers.pointerManager.pointer);
            multiAimConstraint.data.sourceObjects = weightedTransforms;

        }
        this.rigBuilder.Build();
    }

    void OnDestroy()
    {
        Managers.playerManager.OnStatusChange -= OnStatusChange;
    }
    

    private void OnStatusChange(PlayerManager.PlayerState playerState)
    {
        this.lookAtTargetRig.weight = playerState == PlayerManager.PlayerState.NORMAL ? 1.0f : 0.0f;
    }
}

