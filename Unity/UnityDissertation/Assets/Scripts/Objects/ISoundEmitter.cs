using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundEmitter: IWorldObject
{
    public float SoundIntensity { get; }
    public GameObject SoundCollider { get; }

}
