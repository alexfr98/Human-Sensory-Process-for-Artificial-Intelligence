using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents the senses of the Avatar, including sight, touch, and hearing.
/// </summary>
public class AvatarSenses : MonoBehaviour, IAvatarSenses
{
    // List of objects currently in the Avatar's sight.
    private List<IWorldObject> sightList = new List<IWorldObject>();
    public List<IWorldObject> SightList { get { return sightList; } set { sightList = value; } }

    // List of temperature emitters currently being touched by the Avatar.
    private List<ITemperatureEmitter> touchList = new List<ITemperatureEmitter>();
    public List<ITemperatureEmitter> TouchList { get { return touchList; } set { touchList = value; } }

    // List of sound emitters currently being heard by the Avatar.
    private List<ISoundEmitter> hearingList = new List<ISoundEmitter>();
    public List<ISoundEmitter> HearingList { get { return hearingList; } set { hearingList = value; } }

    // List of objects discarded from the Avatar's sight.
    private List<IWorldObject> sightListDiscarted = new List<IWorldObject>();

    // Half distance to the furthest point the Avatar can see.
    public float radius = 3;

    // Field of view angle for the Avatar's sight.
    [Range(0, 360)]
    public float angle = 60;

    // Layer mask for obstacles that block the Avatar's sight.
    public LayerMask obstacleLayer;

    // Position used for raycasting in sight calculations.
    public Vector3 raycastPosition;

    // Amount of glare currently felt by the Avatar.
    private float glareFelt = 0;
    public float Glare { get { return glareFelt; } }

    /// <summary>
    /// Start is called before the first frame update.
    /// Initializes the obstacle layer mask.
    /// </summary>
    private void Start()
    {
        obstacleLayer = LayerMask.GetMask("ObstacleLayer");
    }

    public void UseSightSense(Transform avatarTransform)
    {
        raycastPosition = new Vector3(avatarTransform.position.x, 0.0f, avatarTransform.position.z);

        // Clear previous sight lists.
        sightList.Clear();
        sightListDiscarted.Clear();
        glareFelt = 0;

        // Get all colliders within the sight radius.
        Collider[] rangeChecks = Physics.OverlapSphere(raycastPosition, radius, obstacleLayer);

        if (rangeChecks.Length != 0)
        {
            foreach (Collider c in rangeChecks)
            {
                Transform target = c.transform;
                Vector3 directionToTarget = (new Vector3(target.position.x, 0, target.position.z) - raycastPosition).normalized;

                // Check if the target is within the field of view.
                if (Vector3.Angle(avatarTransform.forward, directionToTarget) < angle / 2
                    || new Vector3(avatarTransform.position.x, 0.0f, avatarTransform.position.z) + new Vector3(1.0f, 0.0f, 0.0f) == new Vector3(c.gameObject.transform.position.x, 0.0f, c.gameObject.transform.position.z))
                {
                    float distanceToTarget = Vector3.Distance(raycastPosition, new Vector3(target.position.x, 0, target.position.z));
                    float distanceBeforeTarget = distanceToTarget - 1.0f;

                    // Check if there is a clear line of sight to the target.
                    if (!Physics.Raycast(raycastPosition, directionToTarget, out RaycastHit hit, distanceBeforeTarget, obstacleLayer))
                    {
                        float glare = glareFelt;

                        // Calculate glare if the target is a light emitter.
                        if (c.gameObject.GetComponent<ILightEmitter>() != null)
                        {
                            float theta = Vector3.Angle(new Vector3(avatarTransform.forward.x, 0, avatarTransform.forward.z), directionToTarget);

                            if (theta >= 0 && theta < 60)
                            {
                                glare = c.gameObject.GetComponent<ILightEmitter>().LightIntensity / (distanceToTarget * 115) * (float)Math.Cos(ConvertToRadians(theta));
                            }
                        }

                        if (glare > glareFelt)
                        {
                            glareFelt = glare;
                        }

                        sightList.Add(c.gameObject.GetComponent<IWorldObject>());
                    }
                    else
                    {
                        sightListDiscarted.Add(c.gameObject.GetComponent<IWorldObject>());
                    }
                }
            }
        }
    }

    public float CalculateTemperature(float ambientTemp, Vector3 currentPosition)
    {
        if (touchList.Count == 0)
        {
            return ambientTemp;
        }
        float maximumTemp = ambientTemp;

        // Get the maximum temperature felt from all emitters.
        for (int i = 0; i < touchList.Count; i++)
        {
            float temperature = touchList[i].Temperature;
            float dispersion = touchList[i].Dispersion;
            Vector3 gameObjectPosition = touchList[i].TemperatureCollider.transform.position;

            float distance = Vector3.Distance(new Vector3(currentPosition.x, 0, currentPosition.z), new Vector3(gameObjectPosition.x, 0, gameObjectPosition.z));
            float Tr = temperature - (distance * dispersion);

            if (Tr > maximumTemp)
            {
                maximumTemp = Tr;
            }
        }

        return maximumTemp;
    }
    

    public float CalculateSound(Vector3 currentPosition)
    {
        List<GameObject> hearingList = GameObject.FindGameObjectsWithTag("Speaker").ToList();

        if (hearingList.Count == 0)
        {
            Debug.Log("Hearing list is empty");
            return 0;
        }
        float maximumSound = 0;

        // Get the maximum sound intensity felt from all emitters.
        for (int i = 0; i < hearingList.Count; i++)
        {
            Vector3 gameObjectPosition = hearingList[i].gameObject.transform.position;

            float distance = Vector3.Distance(new Vector3(currentPosition.x, 0, currentPosition.z), new Vector3(gameObjectPosition.x, 0, gameObjectPosition.z));
            float intensityReduction = (float)(20 * Math.Log10(distance / 0.5));
            float intensityFelt = hearingList[i].GetComponent<ISoundEmitter>().SoundIntensity - intensityReduction;

            if (intensityFelt > maximumSound)
            {
                maximumSound = intensityFelt;
            }
        }

        return maximumSound;
    }

    public void ActualizeSensesListAdding(Collider other)
    {
        if (other.gameObject.name == "Temperature")
        {
            touchList.Add(other.gameObject.transform.parent.gameObject.GetComponent<ITemperatureEmitter>());
        }
        else if (other.gameObject.name == "Sound")
        {
            hearingList.Add(other.gameObject.transform.parent.gameObject.GetComponent<ISoundEmitter>());
        }
    }

    public void ActualizeSensesListRemoving(Collider other)
    {
        if (other.gameObject.name == "Temperature")
        {
            touchList.Remove(other.gameObject.transform.parent.gameObject.GetComponent<ITemperatureEmitter>());
        }
        else if (other.gameObject.name == "Sound")
        {
            hearingList.Remove(other.gameObject.transform.parent.gameObject.GetComponent<ISoundEmitter>());
        }
    }

    /// <summary>
    /// Converts an angle from degrees to radians.
    /// </summary>
    /// <param name="angle">The angle in degrees.</param>
    /// <returns>The angle in radians.</returns>
    private double ConvertToRadians(double angle)
    {
        return (Math.PI / 180) * angle;
    }

    /// <summary>
    /// Returns the list of objects that were not viewed by the Avatar.
    /// </summary>
    /// <returns>List of objects not viewed.</returns>
    public List<IWorldObject> GetListNotViewed()
    {
        return sightListDiscarted;
    }
}
