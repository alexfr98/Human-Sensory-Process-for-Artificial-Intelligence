using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldObject
{
    public string ObjectType { get; }
    public float Distance { get; }
    public GameObject gameObject { get; }
}
