using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAvatarDrives
{
    void ActualizeDrives(Tuple<bool, float, float> tuple, float temperature, float glare, float sound);
    bool CheckRestart();
    public float Health { get; }
    public float Hunger { get; }
}