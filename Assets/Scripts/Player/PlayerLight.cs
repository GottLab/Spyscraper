using UnityEngine;

[RequireComponent(typeof(Light))]
public class PlayerLight : MonoBehaviour
{

    [SerializeField]
    private IsometricController isometricController;

    private Vector3 targetLookingLight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.targetLookingLight = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.targetLookingLight = Vector3.Lerp(this.targetLookingLight, this.isometricController.LookingPoint, Time.deltaTime * 20.0f);
        //this.spotLight.range = Vector3.Distance(isometricPlayer.LookingPoint, this.transform.position);

        Vector3 lightDirection = this.targetLookingLight - this.transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(lightDirection);
        this.transform.rotation = lookRotation;
    }
}
