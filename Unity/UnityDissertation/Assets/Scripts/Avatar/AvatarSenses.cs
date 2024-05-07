using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AvatarSenses: MonoBehaviour, IAvatarSenses
{

    private List<IWorldObject> sightList = new List<IWorldObject>();
    public List<IWorldObject> SightList { get { return sightList; } set { sightList = value; } }
    private List<ITemperatureEmitter> touchList = new List<ITemperatureEmitter>();
    public List<ITemperatureEmitter> TouchList { get { return touchList; } set { touchList = value; } }
    private List<ISoundEmitter> hearingList = new List<ISoundEmitter>();
    public List<ISoundEmitter> HearingList { get { return hearingList; } set { hearingList = value; } }

    private List<IWorldObject> sightListDiscarted = new List<IWorldObject>();
    
    //Half Distance to the furthest point I can see
    public float radius = 3 ;

    [Range(0, 360)]
    public float angle = 60;

    public LayerMask obstacleLayer;

    public Vector3 raycastPosition;

    private float glareFelt = 0;

    public float Glare { get { return glareFelt; } }

    private void Start()
    {
        obstacleLayer = LayerMask.GetMask("ObstacleLayer");
    }
    public void UseSightSense(Transform avatarTransform)
    {
        raycastPosition = new Vector3(avatarTransform.position.x, 0.0f, avatarTransform.position.z);
        

        //First - view sense
        sightList.Clear();
        sightListDiscarted.Clear();
        glareFelt = 0;

        //This should be limited for the number of objects that a human can focus. 

        //raycastPosition + (this.transform.forward * radius/2) this line is an optimization. With this line, we do the overlapBox function just for the part in front of the player. 
        //We move the raycast position in front of the player only the half of the radius to be able to calculate everything.
        Collider[] rangeChecks = Physics.OverlapSphere(raycastPosition, radius , obstacleLayer);

        if (rangeChecks.Length != 0)
        {
            
            foreach (Collider c in rangeChecks)
            {
                Transform target = c.transform;
                Vector3 directionToTarget = (new Vector3(target.position.x, 0, target.position.z) - raycastPosition).normalized;
                //Debug.Log("Angle with avatar: " + Vector3.Angle(transform.forward, directionToTarget).ToString());

                //If inside field of view
                //It is also checked if is one of the three objects that is just in front of me
                if (Vector3.Angle(avatarTransform.forward, directionToTarget) < angle / 2
                    || new Vector3(avatarTransform.position.x, 0.0f, avatarTransform.position.z) + new Vector3(1.0f, 0.0f, 0.0f) == new Vector3(c.gameObject.transform.position.x, 0.0f, c.gameObject.transform.position.z))

                {

                    float distanceToTarget = Vector3.Distance(raycastPosition, new Vector3(target.position.x, 0, target.position.z));

                    float distanceBeforeTarget = distanceToTarget - 1.0f;


                    if (!Physics.Raycast(raycastPosition, directionToTarget, out RaycastHit hit, distanceBeforeTarget, obstacleLayer))
                    {
                        float glare = glareFelt;

                        if(c.gameObject.GetComponent<ILightEmitter>() != null)
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
        if(touchList.Count == 0)
        {
            return ambientTemp;
        }
        float maximumTemp = ambientTemp;
        if (touchList.Count > 0)
        {
            //Getting maximum temperature being felt
            for (int i = 0; i < touchList.Count; i++)
            {
                float temperature = touchList[i].Temperature;
                float dispersion = touchList[i].Dispersion;
                //Radiation temperature
                Vector3 gameObjectPosition = touchList[i].TemperatureCollider.transform.position;

                float distance = Vector3.Distance(new Vector3(currentPosition.x, 0, currentPosition.z), new Vector3(gameObjectPosition.x, 0, gameObjectPosition.z));
                float Tr = temperature - (distance * dispersion);

                if (Tr > maximumTemp)
                {

                    maximumTemp = Tr;
                }
                
            }

        }


        return maximumTemp;
        


    }

    public float CalculateSound(Vector3 currentPosition)
    {
        // Doing it like this because the avatar is always inside the hearing collider
        List<GameObject> hearingList = GameObject.FindGameObjectsWithTag("Speaker").ToList();

        if (hearingList.Count == 0)
        {
            Debug.Log("Hearing list is empty");
            return 0;
        }
        float maximumSound = 0;
        if (hearingList.Count > 0)
        {
            //Getting maximum temperature being felt
            for (int i = 0; i < hearingList.Count; i++)
            {

                //Sound intensity heared by the avatar

                Vector3 gameObjectPosition = hearingList[i].gameObject.transform.position;

                float distance = Vector3.Distance(new Vector3(currentPosition.x, 0, currentPosition.z), new Vector3(gameObjectPosition.x, 0, gameObjectPosition.z));
                float intensityReduction = (float)(20 * Math.Log10(distance / 0.5));
                float intensityFelt = hearingList[i].GetComponent<ISoundEmitter>().SoundIntensity - intensityReduction;

                if (intensityFelt > maximumSound)
                {

                    maximumSound = intensityFelt;
                }

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

    public double ConvertToRadians(double angle)
    {
        return (Math.PI / 180) * angle;
    }

    public List<IWorldObject> getListNotViewed()
    {
        return sightListDiscarted;
    }
}
