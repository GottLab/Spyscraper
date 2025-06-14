using System;
using Enemy;
using UnityEngine;

public class IsometricController : MonoBehaviour
{
    private CharacterController _charController;
    public float speed = 1.0f;
    
    private float vSpeed = 0;

    [SerializeField]
    private float gravity = -9.8f;

    [SerializeField]
    private float lookRotationSpeed = 15.0f;

    [SerializeField]
    private Camera cam;


    [SerializeField, Tooltip("Mask used for cursor looking")]
    private LayerMask lookLayerMask;

    
    [SerializeField]
    private Transform playerModel;

    private Vector3 lookingPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(this._charController.isGrounded)
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

        //this.playerModel.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.TransformDirection(moveDirection)), Time.deltaTime * 5F);
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray,out hit, Mathf.Infinity, lookLayerMask)) // Check if we hit something
        {
            Vector3 lookDirection = (hit.point - playerModel.position).normalized;
            
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            Vector3 euler = lookRotation.eulerAngles;

            // Remove Z axis (roll)
            euler.x = 0f;
            euler.z = 0f;

            this.playerModel.rotation = Quaternion.Slerp(this.playerModel.rotation, Quaternion.Euler(euler), Time.deltaTime * lookRotationSpeed);
            this.lookingPoint = hit.point;
        }


    }

    public Vector3 LookingPoint
    {
        get { return this.lookingPoint; }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.CompareTag("Enemy"))
        {
            hit.gameObject.GetComponent<StateEnemyAI>().OnPlayerCollide();
        }
    }
}
