using System;
using UnityEngine;

public class IsometricController : MonoBehaviour
{
    private CharacterController _charController;
    public float speed = 1.0f;


    private float vSpeed = 0;

    [SerializeField]
    private float gravity = -9.8f;


    [SerializeField]
    private Camera cam;

    
    [SerializeField]
    private Transform playerModel;

    private Vector3 lookingPoint;

    public Animator animator;


    private readonly int WalkingProperty = Animator.StringToHash("walking");
    private readonly int SpeedXProperty = Animator.StringToHash("speedX");
    private readonly int SpeedYProperty = Animator.StringToHash("speedY");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (this._charController.isGrounded)
            this.vSpeed = 0;

        this.vSpeed += this.gravity * Time.deltaTime;

        Transform cameraTransform = this.cam.transform;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * Vector3.forward * vertical;
        Vector3 right = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * Vector3.right * horizontal;
        Vector3 moveDirection = (forward + right).normalized;
        Vector3 movement = moveDirection *= speed;

        movement.y = this.vSpeed;
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        _charController.Move(movement);
        

        

        
    }

    public Vector3 LookingPoint
    {
        get { return this.lookingPoint; }
    }
}
