using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;


[RequireComponent(typeof(Animator))]
public class LookAndRotate : MonoBehaviour
{

    public Transform target;

    public float maxAngle = 90F;

    public float speed = 10f;

    private Animator animator;

    private readonly int turnProperty = Animator.StringToHash("Turn");
    private readonly int walkingProperty = Animator.StringToHash("walking");

    public MultiParentConstraint multiParentConstraint;

    public AnimationCurve animationCurve;

    public Rig flashlightRig;

    private bool useTorch = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.target == null || animator == null)
            return;

        Vector3 dir = target.position - this.transform.position;
        dir.y = 0;

        float angleY = Vector3.SignedAngle(this.transform.forward, dir, Vector3.up);

        if (Mathf.Abs(angleY) >= maxAngle)
        {
            float turnDirection = Mathf.Sign(angleY);


            if (animator.GetBool(walkingProperty))
            {

                float angleDifference = (Mathf.Abs(angleY) - maxAngle) * turnDirection;
                //while walking we need to rotate the player since there is not a root animation
                this.transform.Rotate(Vector3.up, angleDifference * Time.deltaTime * speed);
            }
            else
            {
                //When walking we use the turn animation itself to rotate the player since it is a root animation
                animator.SetFloat(turnProperty, -turnDirection);
            }
        }
        else if (animator.GetFloat(turnProperty) != 0.0)
        {
            animator.SetFloat(turnProperty, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            useTorch = !useTorch;
            StopAllCoroutines();
            StartCoroutine(SwitchFlashLightPose(useTorch, 0.3f));
        }

    }

    IEnumerator FlashLightRig(bool start, float time)
    {
        float elapsed = 0f;

        float startValue = start ? 0.0f : 1.0f;
        float target = start ? 1.0f : 0.0f;

        while (elapsed < time)
        {
            float t = elapsed / time;
            this.flashlightRig.weight = Mathf.Lerp(startValue, target, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        this.flashlightRig.weight = target;
    }

    IEnumerator SwitchFlashLightPose(bool start, float time)
    {

        if (start)
            yield return StartCoroutine(FlashLightRig(true, 0.3f));


        var sourceObjects = multiParentConstraint.data.sourceObjects;

        int startTransform = start ? 0 : 2;
        int endTransform = start ? 2 : 0;

        float elapsed = 0f;

        while (elapsed < time)
        {
            float t = elapsed / time;

            float interpo = animationCurve.Evaluate(t);


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
            yield return StartCoroutine(FlashLightRig(false, 0.3f));

    }
    
}
