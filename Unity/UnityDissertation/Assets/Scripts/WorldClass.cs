using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Singleton class that represents the game world, managing world objects and their positions.
/// </summary>
public class WorldClass : MonoBehaviour
{
    // Singleton instance
    public static WorldClass Instance { get; private set; }

    // Ambient temperature of the world
    [SerializeField]
    private float ambientTemperature = 25.0f;
    public float AmbientTemperature { get { return ambientTemperature; } }

    // Number of rows in the world grid
    private int rows = 5;
    public int Rows { get { return rows; } }

    // Number of columns in the world grid
    private int cols = 5;
    public int Columns { get { return cols; } }

    // List of food objects in the world
    private List<FoodObject> foods = new List<FoodObject>();
    public List<FoodObject> Foods { get { return foods; } set { foods = value; } }

    // List of all world objects
    private List<IWorldObject> totalObjects = new List<IWorldObject>();
    public List<IWorldObject> TotalObjects { get { return totalObjects; } set { totalObjects = value; } }

    // List of possible positions for objects
    [SerializeField]
    private List<Vector3> possiblePositions = new List<Vector3>();
    public List<Vector3> PossiblePositions { get { return possiblePositions; } set { possiblePositions = value; } }

    /// <summary>
    /// Ensures that there is only one instance of WorldClass.
    /// </summary>
    private void Awake()
    {
        // If there is an instance, and it's not this one, destroy this object.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Initializes the world state on start.
    /// </summary>
    private void Start()
    {
        InitializePossiblePositions();
        InitializeObjects();
    }

    /// <summary>
    /// Initializes the possible positions in the world grid.
    /// </summary>
    private void InitializePossiblePositions()
    {
        // Populate the possible positions list based on the grid size.
        for (int i = 0; i < rows; i++)
        {
            for (int x = 0; x < cols; x++)
            {
                possiblePositions.Add(new Vector3(x, 0, i));
            }
        }
    }

    /// <summary>
    /// Initializes objects in the world, setting their positions and types.
    /// </summary>
    private void InitializeObjects()
    {
        // Find objects by their tags.
        GameObject[] fireTag = GameObject.FindGameObjectsWithTag("Fire");
        GameObject[] bulbTag = GameObject.FindGameObjectsWithTag("Bulb");
        GameObject[] foodObjectsTag = GameObject.FindGameObjectsWithTag("Apple");
        GameObject[] speakerObjectsTag = GameObject.FindGameObjectsWithTag("Speaker");

        // Process fire objects.
        for (int i = 0; i < fireTag.Length; i++)
        {
            possiblePositions.Remove(new Vector3(fireTag[i].transform.position.x, 0, fireTag[i].transform.position.z));
            totalObjects.Add(fireTag[i].GetComponent<IWorldObject>());
            if (fireTag[i].GetComponent<FireObject>() == null)
            {
                Debug.LogError("component does not exist");
            }
        }

        // Process food objects.
        for (int i = 0; i < foodObjectsTag.Length; i++)
        {
            possiblePositions.Remove(new Vector3(foodObjectsTag[i].transform.position.x, 0, foodObjectsTag[i].transform.position.z));
            totalObjects.Add(foodObjectsTag[i].GetComponent<FoodObject>());
            foods.Add(foodObjectsTag[i].GetComponent<FoodObject>());
        }

        // Process bulb objects.
        for (int i = 0; i < bulbTag.Length; i++)
        {
            possiblePositions.Remove(new Vector3(bulbTag[i].transform.position.x, 0, bulbTag[i].transform.position.z));
            totalObjects.Add(bulbTag[i].GetComponent<IWorldObject>());
        }

        // Process speaker objects.
        for (int i = 0; i < speakerObjectsTag.Length; i++)
        {
            possiblePositions.Remove(new Vector3(speakerObjectsTag[i].transform.position.x, 0, speakerObjectsTag[i].transform.position.z));
            totalObjects.Add(speakerObjectsTag[i].GetComponent<SpeakerObject>());
        }
    }

    /// <summary>
    /// Resets the position of a world object to a new possible position.
    /// </summary>
    /// <param name="worldObject">The world object to reset.</param>
    public void ResetObject(IWorldObject worldObject)
    {
        // Get the current position of the object.
        Vector3 objectPosition = new Vector3(worldObject.gameObject.transform.position.x, 0, worldObject.gameObject.transform.position.z);

        // Get a random new position from possible positions.
        int index = UnityEngine.Random.Range(0, possiblePositions.Count);
        Vector3 newFoodPosition = possiblePositions[index];
        worldObject.gameObject.transform.position = new Vector3(newFoodPosition.x, worldObject.gameObject.transform.position.y, newFoodPosition.z);

        // Update possible positions list.
        possiblePositions.Add(objectPosition);
        possiblePositions.Remove(newFoodPosition);
    }
}
