using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITemperatureEmitter: IWorldObject
{
    public GameObject TemperatureCollider { get;}
    float Temperature { get; }
    float Dispersion { get; }

}
