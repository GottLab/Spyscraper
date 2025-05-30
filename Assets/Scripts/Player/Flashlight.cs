using NUnit.Framework;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class Flashlight : MonoBehaviour
{


    private Light _flashlightSpotlight;
    private Material _FlashlightBulbMaterial;



    public void Turn(bool on)
    {
        //enable or disable spotlight
        FlashLightSpotlight.enabled = on;
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
