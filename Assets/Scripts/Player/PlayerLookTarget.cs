using System.Collections.Generic;
using QTESystem;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerLookTarget : MonoBehaviour
{
    [SerializeField, Tooltip("Rig Used to aim body and head towards the target")]
    private Rig lookAtTargetRig;

    [SerializeField, Tooltip("Rig Builder is needed to rebuild rigs after updating look constraint")]
    private RigBuilder rigBuilder;

    private MultiAimConstraint[] multiAimConstraints;

    void Start()
    {
        this.multiAimConstraints = this.GetComponentsInChildren<MultiAimConstraint>();

        
    }

    void OnEnable()
    {
        PlayerManager.OnStatusChange += OnStatusChange;
        QTEManager.OnQteSequenceStart += QteSequenceStart;
    }

    void OnDisable()
    {
        PlayerManager.OnStatusChange -= OnStatusChange;
        QTEManager.OnQteSequenceStart -= QteSequenceStart;
    }

    //this method makes the player look continuosly to a transform target
    //if target is null then the weight is set to 0 and stops the player looking behaviour
    void SetLookTarget(Transform target)
    {

        if (target == null)
        {
            this.lookAtTargetRig.weight = 0.0f;
            return;
        }

        foreach (MultiAimConstraint multiAimConstraint in multiAimConstraints)
        {
            WeightedTransformArray weightedTransforms = multiAimConstraint.data.sourceObjects;
            weightedTransforms.SetTransform(0, target);
            multiAimConstraint.data.sourceObjects = weightedTransforms;

        }
        this.rigBuilder.Build();
        this.lookAtTargetRig.weight = 1.0f;
    }

    private void QteSequenceStart(IQtePlayer enemy)
    {
         //player should look to the qteEnemy
        this.SetLookTarget(enemy.GetTransform());
    }

    private void OnStatusChange(PlayerManager.PlayerState playerState)
    {
        if (playerState == PlayerManager.PlayerState.NORMAL) //normally players looks at the cursor
        {
            this.SetLookTarget(Managers.pointerManager.pointer);
        }
        else if (playerState == PlayerManager.PlayerState.DIED) //if player dies the players stops looking target 
        {
            this.SetLookTarget(null);
        }
    }
}

