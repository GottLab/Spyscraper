
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class PlayerFlashLight : MonoBehaviour
{

    [SerializeField, Tooltip("Constraint to make the flashlight move between player holster and holding position")]
    private MultiParentConstraint multiParentConstraint;
    
    [SerializeField, Tooltip("describes the animation curve the flashlight follows to reach the target")]
    private AnimationCurve flashLightEquipAnimationCurve;

    [SerializeField, Tooltip("The rig used to: make the head, spine and head look the target and enable ik animation for the right arm ")]
    private Rig flashlightRig;

    [SerializeField]
    private Flashlight flashlight;

    [SerializeField, Tooltip("the time it takes in seconds for the player to reach/release the flashlight and return to the default animation")]
    private float flashlightRigTime = 0.3f;

    [SerializeField, Tooltip("the time it takes in seconds for the player to switch between holding the flashlight and holster")]
    private float flashlightEquipTime = 0.5f;

    private bool usingFlashlight = false;

    public void Start()
    {
        this.SwitchFlashLight(this.usingFlashlight);
    }
    
    void OnEnable()
    {
        PlayerManager.OnStatusChange += OnStatusChange;
    }

    void OnDisable()
    {
        PlayerManager.OnStatusChange -= OnStatusChange;
    }

    private void OnStatusChange(PlayerManager.PlayerState playerState)
    {

        bool isDead = playerState == PlayerManager.PlayerState.DIED;
        this.flashlight.Deattach(isDead);

        if (playerState == PlayerManager.PlayerState.NORMAL)
        {
            SwitchFlashLight(this.usingFlashlight);
        }
        else
        {
            ResetWeights();

            //obviously a dead person cannot turn the flashlight off
            if (!isDead)
            {
                this.flashlight.Turn(false);
            }
        }

    }

    public void Update()
    {
        if (GameManager.GetKeyDown(KeyCode.Space) && Managers.playerManager.IsState(PlayerManager.PlayerState.NORMAL))
        {
            this.usingFlashlight = !this.usingFlashlight;
            SwitchFlashLight(this.usingFlashlight);
        }
    }

    private void SwitchFlashLight(bool on)
    {
        StopAllCoroutines();
        StartCoroutine(SwitchFlashLightPose(on));
        this.usingFlashlight = on;

        Managers.game.PlayEvent(on ? GameManager.GameEvent.TorchOn : GameManager.GameEvent.TorchOff);
    }

    private void ResetWeights()
    {
        var sourceObjects = multiParentConstraint.data.sourceObjects;
        sourceObjects.SetWeight(0, 1.0f);
        sourceObjects.SetWeight(1, 0.0f);
        sourceObjects.SetWeight(2, 0.0f);
        multiParentConstraint.data.sourceObjects = sourceObjects;
        this.flashlightRig.weight = 0.0f;
    }
    

    //This method is used to smootly enable and disable the flashlight rig.
    IEnumerator FlashLightRig(bool start)
    {
        float elapsed = 0f;

        float startValue = start ? 0.0f : 1.0f;
        float target = start ? 1.0f : 0.0f;

        while (elapsed < flashlightRigTime)
        {
            float t = elapsed / flashlightRigTime;
            this.flashlightRig.weight = Mathf.Lerp(startValue, target, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        this.flashlightRig.weight = target;
    }

    //switched the flashlight pose in a certain amount of time
    IEnumerator SwitchFlashLightPose(bool start)
    {

        
        if (start)
        {
            //if we are starting the animation then enable the flashlight rig
            yield return StartCoroutine(FlashLightRig(true));
        }
        else
        {
            //turn off the flashlight at the end of the animation
            flashlight.Turn(false);
        }



        var sourceObjects = multiParentConstraint.data.sourceObjects;

        //0 index is the holster transform and 2 is the flashlight target position
        //1 is used as a guide for movement between the two transforms avoiding a linear interpolation between the two
        int startTransform = start ? 0 : 2;
        int endTransform = start ? 2 : 0;

        float elapsed = 0f;

        while (elapsed < flashlightEquipTime)
        {
            float t = elapsed / flashlightEquipTime;

            float interpo = flashLightEquipAnimationCurve.Evaluate(t);


            sourceObjects.SetWeight(startTransform, 1 - interpo);
            sourceObjects.SetWeight(1, Mathf.Sin(t * Mathf.PI));
            sourceObjects.SetWeight(endTransform, interpo);
            multiParentConstraint.data.sourceObjects = sourceObjects;
            elapsed += Time.deltaTime;
            yield return null;
        }

        sourceObjects.SetWeight(startTransform, 0.0f);
        sourceObjects.SetWeight(1, 0.0f);
        sourceObjects.SetWeight(endTransform, 1.0f);
        multiParentConstraint.data.sourceObjects = sourceObjects;

        if (!start)
        {
            yield return StartCoroutine(FlashLightRig(false));
        }
        else
        {
            flashlight.Turn(true);
        }

    }
}