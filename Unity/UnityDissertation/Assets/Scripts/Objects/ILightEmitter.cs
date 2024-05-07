using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILightEmitter: IWorldObject
{
    float LightIntensity { get; }
}
