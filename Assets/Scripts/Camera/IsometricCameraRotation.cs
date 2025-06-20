using UnityEngine;

public class IsometricCameraRotation : MonoBehaviour
{

    private int currentRotation;
    private float currentYRotation;

    private float velocity = 0f;

    [SerializeField]
    private float rotationSpeed = 1.0F;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        float targetYRotation = 45F + currentRotation * 90F;

        currentYRotation = Mathf.SmoothDampAngle(currentYRotation, targetYRotation, ref velocity, 1f / rotationSpeed);

        // Apply the rotation while keeping other axes unchanged
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, currentYRotation, transform.eulerAngles.z);


        if (Managers.playerManager.IsState(PlayerManager.PlayerState.NORMAL))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateRight();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateLeft();
            }
        }

    }

    void RotateRight()
    {
        this.currentRotation = (this.currentRotation + 1) % 4;
    }
    void RotateLeft()
    {
        this.currentRotation = (this.currentRotation - 1) % 4;
    }
}
