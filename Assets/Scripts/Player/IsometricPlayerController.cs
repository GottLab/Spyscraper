using Unity.Cinemachine;
using UnityEngine;


[
    RequireComponent(typeof(CharacterController)),
    RequireComponent(typeof(Animator))
]
public class IsometricPlayerController : MonoBehaviour
{

    [SerializeField, Tooltip("Transform which rotation is used to move relative to it")]
    private Transform cam;

    [SerializeField]
    private float gravity = -9.8f;

    private Animator animator;

    [SerializeField, Tooltip("the angle after which the player must rotate")]
    private float maxAngle = 90F;

    [SerializeField, Tooltip("Speed used to rotate the player while walking")]
    public float walkingRotationSpeed = 10f;

    private CharacterController _characterController;

    private readonly int WalkingProperty = Animator.StringToHash("walking");
    private readonly int SpeedXProperty = Animator.StringToHash("speedX");
    private readonly int SpeedYProperty = Animator.StringToHash("speedY");
    private readonly int turnProperty = Animator.StringToHash("Turn");

    private readonly int speedProperty = Animator.StringToHash("Speed");


    private bool _isWalking = false;

    private float verticalVelocity = 0f;

    public float speed = 4.0f;

    private float lastMovementTime = 0.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateRotation();
        UpdateMovement();
        
        
    }

    void LateUpdate()
    {
        
    }

    /*
    void OnAnimatorMove()
    {
        Vector3 rootMotion = animator.deltaPosition;

        // Apply gravity
        if (_characterController.isGrounded)
        {
            verticalVelocity = 0f; // Reset when grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        rootMotion.y = verticalVelocity * Time.deltaTime;
        _characterController.Move(rootMotion);
        transform.rotation *= animator.deltaRotation;
    }
    */


    private void UpdateMovement()
    {
        Transform cameraTransform = this.cam.transform;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * Vector3.forward * vertical;
        Vector3 right = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * Vector3.right * horizontal;
        Vector3 moveDirection = (forward + right).normalized;


        Vector3 movement = moveDirection * speed;
        if (!this._characterController.isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        else
        {
            verticalVelocity = 0;
        }
        movement.y = verticalVelocity;
        _characterController.Move(movement * Time.deltaTime);

        Vector3 localDirection = transform.InverseTransformDirection(moveDirection).normalized;

        animator.SetFloat(SpeedXProperty, localDirection.x, 0.07f, Time.deltaTime);
        animator.SetFloat(SpeedYProperty, localDirection.z, 0.07f, Time.deltaTime);
        //we divide by 2.0 since the animation author said that the speed of the walking animation is 2.0 m/s
        animator.SetFloat(speedProperty, this.speed / 2.0f);

        bool inputActive = horizontal != 0.0f || vertical != 0.0f;

        if (inputActive)
        {
            lastMovementTime = Time.time;
        }

        //the player is still walking event after some time, this is to avoid jarring state transition when quickly going left or right
        _isWalking = Time.time - lastMovementTime < 0.1f;

        if (animator.GetBool(WalkingProperty) != _isWalking)
        {
            animator.SetBool(WalkingProperty, _isWalking);
        }
    }

    private void UpdateRotation()
    {
        Transform target = Managers.pointerManager.pointer;
        if (target == null || this.animator == null)
            return;

        Vector3 dir = target.position - this.transform.position;
        dir.y = 0;

        float angleY = Vector3.SignedAngle(this.transform.forward, dir, Vector3.up);

        if (Mathf.Abs(angleY) >= maxAngle)
        {
            float turnDirection = Mathf.Sign(angleY);

            if (this._isWalking)
            {

                float angleDifference = (Mathf.Abs(angleY) - maxAngle) * turnDirection;
                //while walking we need to rotate the player since there is not a root animation
                this.transform.Rotate(Vector3.up, angleDifference * Time.deltaTime * this.walkingRotationSpeed);
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
    }

}
