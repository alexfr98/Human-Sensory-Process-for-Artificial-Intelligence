using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbObject : BaseObject, ILightEmitter, ITemperatureEmitter
{
    private float lightIntensity;
    public float LightIntensity { get { return lightIntensity; } }

    private float temperature;
    public float Temperature { get { return temperature; } }

    private float dispersion;
    public float Dispersion { get { return dispersion; } }

    private GameObject temperatureCollider;
    public GameObject TemperatureCollider { get { return temperatureCollider; } }

    public GameObject GameObject { get { return this.gameObject; } }
    void Start()
    {
        objectType = "Bulb";
        temperatureCollider = this.gameObject.transform.GetChild(1).gameObject;
        temperature = 35.0f;
        dispersion = 15;
        Debug.Log("ambient temperature is: " + WorldClass.Instance.AmbientTemperature);

        float colliderRadius = (WorldClass.Instance.AmbientTemperature - temperature) / dispersion;
        //This will be negative because the temperature of the bulb is not high enough
        Debug.Log("Collider Radius = " + colliderRadius.ToString());
        this.temperatureCollider.transform.localScale = new Vector3(colliderRadius, colliderRadius, colliderRadius);


        //https://technical-tips.com/blog/hardware/luminance-cdm2-explained-simply--33616. The higher the candela, the higher the brightness
        lightIntensity = 1000f * ((this.gameObject.transform.GetChild(2).GetComponent<Light>().intensity + this.gameObject.transform.GetChild(2).GetComponent<Light>().range) / 2);
        Debug.Log("Intensity: " + lightIntensity.ToString() + "for bulb " + this.name);
    }
}
