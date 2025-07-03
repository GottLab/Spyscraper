using Enemy;
using TMPro;
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

    [SerializeField, Tooltip("Meter Text")]
    private TextMeshProUGUI suspitionMeterText;

    [SerializeField, Tooltip("Speed to update meter fill amount")]
    private float meterUpdateSpeed = 1.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.canvas = this.GetComponent<Canvas>();
        this.canvas.worldCamera = Camera.main;
    }

    void OnEnable()
    {
        this.stateEnemyAI.OnSuspitionReset += ResetMeter;
        this.stateEnemyAI.OnSuspitionChange += UpdateText;
    }

    void OnDisable()
    {
        this.stateEnemyAI.OnSuspitionReset -= ResetMeter;
        this.stateEnemyAI.OnSuspitionChange -= UpdateText;
    }

    void ResetMeter()
    {
        this.suspitionMeterImage.fillAmount = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = Camera.main.transform.rotation;
        this.transform.LookAt(this.transform.position + rotation * Vector3.forward, rotation * Vector3.up);
        if (this.stateEnemyAI != null && this.suspitionMeterImage != null && this.suspitionMeterText != null)
        {
            this.suspitionMeterImage.fillAmount = Mathf.Lerp(this.suspitionMeterImage.fillAmount, this.stateEnemyAI.SuspitionLevel, Time.deltaTime * meterUpdateSpeed);
            this.suspitionMeterImage.color = Color.Lerp(Color.yellow, Color.red, this.suspitionMeterImage.fillAmount);
        }

    }

    void UpdateText(bool suspitionIncreased)
    {
        bool maxSuspition = this.stateEnemyAI.SuspitionLevel >= 1.0f;
        this.suspitionMeterText.text = maxSuspition ? "!" : "?";
    }

}
