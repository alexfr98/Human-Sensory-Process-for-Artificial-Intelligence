using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerObject : BaseObject, ISoundEmitter
{
    private float soundIntensity;
    public float SoundIntensity { get { return soundIntensity; } }

    private GameObject soundCollider;
    public GameObject SoundCollider { get { return soundCollider; } }
    public GameObject GameObject { get { return this.gameObject; } }

    void Awake()
    {
        objectType = "Speaker";
        soundCollider = this.gameObject.transform.GetChild(3).gameObject;
        soundIntensity = 100.0f;

        //For the inverse square relationship --> intensityRedcution = 20*log(distance2/distance1) and distance1 is always 0.5 : https://www.qlight.com/productdata/techdata/en/08_Intensity_of_Sound.pdf
        //Distance1 should be defined by the scale of the avatar and the scale of the object which transmits sound

        //With this equation, we will find the distance where the sound reduction is equal to soundIntensity with: 10^(y/20) * 0.5 = distance, where y is the soundIntensity felt at distance1
        float distance = (float)(Mathf.Pow(10, soundIntensity / 20) * 0.01);
        //We have to set the max distance depending on the intensity

        soundCollider.transform.localScale = new Vector3(distance, distance, distance);
        this.GetComponent<AudioSource>().maxDistance = distance;

        //float intensityValue = Random.Range(80, 130);
    }

}
