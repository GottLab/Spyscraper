using Enemy;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SuspitionMeter : MonoBehaviour
{
    private Canvas canvas;

    [SerializeField, Tooltip("Enemy to track suspition level")]
    private StateEnemyAI stateEnemyAI;

    [SerializeField, Tooltip("Image to represent suspition level")]
    private Image suspitionMeterImage;

    [SerializeField, Tooltip("Speed to update meter fill amount")]
    private float meterUpdateSpeed = 1.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.canvas = this.GetComponent<Canvas>();
        this.canvas.worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = Quaternion.LookRotation(this.canvas.worldCamera.transform.position - this.transform.position);

        if (this.stateEnemyAI != null && this.suspitionMeterImage != null)
        {
            this.suspitionMeterImage.fillAmount = Mathf.Lerp(this.suspitionMeterImage.fillAmount, this.stateEnemyAI.SuspitionLevel, Time.deltaTime * meterUpdateSpeed);

            this.suspitionMeterImage.color = Color.Lerp(Color.yellow, Color.red, this.suspitionMeterImage.fillAmount);
        }
    }

}
