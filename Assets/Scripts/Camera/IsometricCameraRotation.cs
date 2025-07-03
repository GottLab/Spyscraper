using UnityEngine;

public class IsometricCameraRotation : MonoBehaviour
{

    private int currentRotation;
    private float currentYRotation;

    private float velocity = 0f;

    [SerializeField, Tooltip("Rotation Speed")]
    private float rotationSpeed = 1.0F;

    [SerializeField, Min(2), Tooltip("Total Angles that divide this camera rotation")]
    private int totalAngles = 4;

    [SerializeField, Tooltip("Default Base Angle Offset")]
    private float angleOffset = 45F;

    void Update()
    {

        //angle to make a single turn
        float turnAngle = 360.0f / this.totalAngles;
        float targetYRotation = angleOffset + currentRotation * turnAngle;

        currentYRotation = Mathf.SmoothDampAngle(currentYRotation, targetYRotation, ref velocity, 1f / rotationSpeed);

        // Apply the rotation while keeping other axes unchanged
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, currentYRotation, transform.eulerAngles.z);


        if (Managers.playerManager.IsState(PlayerManager.PlayerState.NORMAL) && !Managers.game.IsGameStopped)
        {
            if (GameManager.GetKeyDown(KeyCode.E))
            {
                RotateRight();
                Managers.game.PlayEvent(GameManager.GameEvent.TurnCameraRight);
            }

            if (GameManager.GetKeyDown(KeyCode.Q))
            {
                RotateLeft();
                Managers.game.PlayEvent(GameManager.GameEvent.TurnCameraLeft);
            }
        }

    }

    void RotateRight()
    {
        this.currentRotation = (this.currentRotation + 1) % totalAngles;
    }
    void RotateLeft()
    {
        this.currentRotation = (this.currentRotation - 1) % totalAngles;
    }
}
