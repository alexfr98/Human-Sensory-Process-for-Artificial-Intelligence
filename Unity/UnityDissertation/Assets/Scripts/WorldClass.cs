using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldClass : MonoBehaviour
{
    public static WorldClass Instance { get; private set; }
    [SerializeField]
    private float ambientTemperature = 25.0f;
    public float AmbientTemperature { get { return ambientTemperature; } }
    private int rows = 5;
    public int Rows { get { return rows; } }
    private int cols = 5;
    public int Columns { get { return cols; } }

    private List<FoodObject> foods = new List<FoodObject>();
    public List<FoodObject> Foods { get { return foods; } set { foods = value; } } 
    private List<IWorldObject> totalObjects = new List<IWorldObject>();
    public List<IWorldObject> TotalObjects { get { return totalObjects; } set { totalObjects = value; } }

    [SerializeField]
    private List<Vector3> possiblePositions = new List<Vector3>();
    public List<Vector3> PossiblePositions { get { return possiblePositions; }set { possiblePositions = value; } }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InitializePossiblePositions();
        InitializeObjects();

    }

    private void InitializePossiblePositions()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int x = 0; x < cols; x++)
            {
                possiblePositions.Add(new Vector3(x, 0, i));
            }
        }

    }

    private void InitializeObjects()
    {
        GameObject[] fireTag = GameObject.FindGameObjectsWithTag("Fire");
        GameObject[] bulbTag = GameObject.FindGameObjectsWithTag("Bulb");
        GameObject[] foodObjectsTag = GameObject.FindGameObjectsWithTag("Apple");
        GameObject[] speakerObjectsTag = GameObject.FindGameObjectsWithTag("Speaker");

        for (int i = 0; i < fireTag.Length; i++)
        {
            possiblePositions.Remove(new Vector3(fireTag[i].transform.position.x, 0, fireTag[i].transform.position.z));
            totalObjects.Add(fireTag[i].GetComponent<IWorldObject>());
            if(fireTag[i].GetComponent<FireObject>() == null)
            {
                Debug.LogError("component does not exist");
            }
        }

        for (int i = 0; i < foodObjectsTag.Length; i++)
        {
            possiblePositions.Remove(new Vector3(foodObjectsTag[i].transform.position.x, 0, foodObjectsTag[i].transform.position.z));
            totalObjects.Add(foodObjectsTag[i].GetComponent<FoodObject>());
            foods.Add(foodObjectsTag[i].GetComponent<FoodObject>());
        }
        for (int i = 0; i < bulbTag.Length; i++)
        {
            possiblePositions.Remove(new Vector3(bulbTag[i].transform.position.x, 0, bulbTag[i].transform.position.z));
            totalObjects.Add(bulbTag[i].GetComponent<IWorldObject>());
        }

        for (int i = 0; i < speakerObjectsTag.Length; i++)
        {
            possiblePositions.Remove(new Vector3(speakerObjectsTag[i].transform.position.x, 0, speakerObjectsTag[i].transform.position.z));
            totalObjects.Add(speakerObjectsTag[i].GetComponent<SpeakerObject>());
        }

    }

    public void ResetObject(IWorldObject worldObject)
    {
        Vector3 objectPosition = new Vector3(worldObject.gameObject.transform.position.x, 0, worldObject.gameObject.transform.position.z);

        int index = UnityEngine.Random.Range(0, possiblePositions.Count);
        Vector3 newFoodPosition = possiblePositions[index];
        worldObject.gameObject.transform.position = new Vector3(newFoodPosition.x, worldObject.gameObject.transform.position.y, newFoodPosition.z);

        // Actualizing possible positions
        possiblePositions.Add(objectPosition);
        possiblePositions.Remove(newFoodPosition);
    }


}
