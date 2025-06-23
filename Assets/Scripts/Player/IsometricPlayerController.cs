using System;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
[
    RequireComponent(typeof(CharacterController)),
    RequireComponent(typeof(Animator))
]
public class IsometricPlayerController : MonoBehaviour
{

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

    //it indicates the reasons that are locking player movement, only if this set is empty the player can move
    private HashSet<string> movementLocks = new();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
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
        this.SetMovement("playerStatus", playerState != PlayerManager.PlayerState.NORMAL);
    }

    // Update is called once per frame
    void Update()
    {
        //Do not move or rotate player
        if (!CanMove)
        {
            return;
        }

        UpdateRotation();
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("Missing Main Camera in scene", this);
            return;
        }

        Transform cameraTransform = mainCamera.transform;

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

        if (_isWalking)
        {
            Managers.game.PlayEvent(GameManager.GameEvent.Moving);
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
                animator.SetFloat(turnProperty, turnDirection, 0.1f, Time.deltaTime);
            }
        }
        else if (animator.GetFloat(turnProperty) != 0.0)
        {
            animator.SetFloat(turnProperty, 0.0f, 0.1f, Time.deltaTime);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.CompareTag("Enemy"))
        {
            hit.gameObject.GetComponent<StateEnemyAI>().OnPlayerCollide();
        }
    }

    public bool CanMove
    {
        get => this.movementLocks.Count == 0;
    }

    public void SetMovement(string reason, bool locked)
    {
        if (locked)
        {
            animator.SetBool(WalkingProperty, false);
            this.movementLocks.Add(reason);
        }
        else
            this.movementLocks.Remove(reason);
    }
    
}
