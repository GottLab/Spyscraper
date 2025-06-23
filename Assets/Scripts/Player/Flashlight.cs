using NUnit.Framework;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class Flashlight : MonoBehaviour
{


    private Light _flashlightSpotlight;
    private Material _FlashlightBulbMaterial;


    private Transform originalParent;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    void Start()
    {
        this.originalParent = this.transform.parent;
        this.originalRotation = this.transform.localRotation;
        this.originalPosition = this.transform.localPosition;
    }

    //Makes the flashlight interact with the world as a rigid body
    //when deattach is false, it returns to the original parent
    public void Deattach(bool deattach)
    {   
        if(originalParent != null)
            this.transform.parent = deattach ? null : this.originalParent;

        if (!deattach)
        {
            this.transform.SetLocalPositionAndRotation(this.originalPosition, this.originalRotation);
        }

        this.GetComponent<Rigidbody>().isKinematic = !deattach;
    }

    public void Turn(bool on)
    {
        //enable or disable spotlight
        FlashLightSpotlight.gameObject.SetActive(on);
        //set the emissive material to on or off
        if (on)
        {
            this.FlashlightBulbMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            this.FlashlightBulbMaterial.DisableKeyword("_EMISSION");
        }
    }


    private Light FlashLightSpotlight
    {
        get
        {
            if (_flashlightSpotlight == null)
            {
                _flashlightSpotlight = GetComponentInChildren<Light>();
                Assert.IsNotNull(_flashlightSpotlight, "Light is missing in children!", this);
            }

            return this._flashlightSpotlight;
        }
    }
    
    private Material FlashlightBulbMaterial
    {
        get {
            if (_FlashlightBulbMaterial == null)
            {
                _FlashlightBulbMaterial = this.GetComponent<MeshRenderer>().materials[1];
            }

            return this._FlashlightBulbMaterial;
        }
    }

}
