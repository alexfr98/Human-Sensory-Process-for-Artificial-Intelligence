using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject :  MonoBehaviour, IWorldObject
{
    protected string objectType;
    public string ObjectType { get { return objectType; } }

    // TO DO -- DISTANCE TO THE OBJECT
    protected float distance = -1;
    public float Distance { get { return distance; } }
}
