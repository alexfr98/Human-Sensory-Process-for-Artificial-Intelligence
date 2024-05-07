using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAvatarSenses
{
    public List<IWorldObject> SightList { get; set; }
    public List<ITemperatureEmitter> TouchList { get; set; }
    public List<ISoundEmitter> HearingList { get; set; }

    void ActualizeSensesListAdding(Collider other);
    void ActualizeSensesListRemoving(Collider other);
    void UseSightSense(Transform avatarTransform);
    float CalculateTemperature(float ambientTemp, Vector3 currentPosition);
    float CalculateSound(Vector3 currentPosition);

    public float Glare { get; }
}