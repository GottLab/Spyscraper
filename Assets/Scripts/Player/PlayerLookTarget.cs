using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerLookTarget : MonoBehaviour
{
    [SerializeField, Tooltip("Rig Used to aim body and head towards the target")]
    private Rig lookAtTargetRig;

    [SerializeField, Tooltip("Rig Builder is needed to rebuild rigs after updating look constraint")]
    private RigBuilder rigBuilder;

    void Start()
    {
        var multiAimConstraints = this.GetComponentsInChildren<MultiAimConstraint>();

        Managers.playerManager.OnStatusChange += OnStatusChange;


        foreach (MultiAimConstraint multiAimConstraint in multiAimConstraints)
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

