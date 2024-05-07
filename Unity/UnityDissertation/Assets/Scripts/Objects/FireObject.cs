using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObject : BaseObject, ITemperatureEmitter, ILightEmitter
{
    private float lightIntensity;
    public float LightIntensity { get { return lightIntensity; } }

    private float temperature;
    public float Temperature { get { return temperature; } }

    private float dispersion;
    public float Dispersion { get { return dispersion; } }

    private GameObject temperatureCollider;
    public GameObject TemperatureCollider { get { return temperatureCollider; } }
    void Start()
    {
        objectType = "Fire";
        temperatureCollider = this.gameObject.transform.GetChild(1).gameObject;
        temperature = 60.0f;
        dispersion = 10;

        float colliderRadius = (WorldClass.Instance.AmbientTemperature - temperature) / dispersion;
        this.temperatureCollider.transform.localScale = new Vector3(colliderRadius, colliderRadius, colliderRadius);
    }
}
