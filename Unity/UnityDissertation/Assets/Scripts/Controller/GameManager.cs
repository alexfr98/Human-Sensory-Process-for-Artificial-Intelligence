using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private WorldClass board;

    [SerializeField]
    private GameObject Food;

    private SocketManager socketManager;
    private Avatar avatar;
    private Vector3 targetPosition = Vector3.zero;
    private float movementSpeed = 1000.0f;
    private bool firstInit = true;

    private int currentUnityInstruction = 1;
    private string newAction = "init";
    private GameObject modifiableobject;
    private bool actionFinished = true;
    private bool rotation = false;
    private bool movement = false;
    private void Awake()
    {

        QualitySettings.vSyncCount = 2;
        //Application.targetFrameRate = 300;
    }
    void Start()
    {
        board = WorldClass.Instance;

        avatar = GameObject.Find("Avatar").GetComponent<Avatar>();
        avatar.transform.forward = new Vector3(0.0f, 0.0f, 1.0f);
        socketManager = new SocketManager();
        socketManager.StartCommunication(board, avatar.transform.position, avatar.transform.forward);


    }

    private void FixedUpdate()
    {

        // Q-learning activated
        if (socketManager.InitReceived)
        {
            avatar.Senses.UseSightSense(avatar.transform);

            if (firstInit)
            {
                ActualizeObservation();
                firstInit = false;
                Debug.Log(socketManager.NumberInstructionsPython.ToString() + " " + currentUnityInstruction.ToString() + " " + actionFinished.ToString());
                currentUnityInstruction++;

            }

            else if (socketManager.NumberInstructionsPython == currentUnityInstruction && actionFinished)
            {

                rotation = false;
                movement = false;
                //RECEIVING NEW ACTION DECIDED (PYTHON)
                newAction = socketManager.ReceivedAction;

                if (newAction == "up" || newAction == "moveUpRot")
                {
                    targetPosition = avatar.transform.position + new Vector3(0.0f, 0.0f, 1.0f);

                    movement = true;
                    if (newAction == "moveUpRot")
                    {
                        rotation = true;
                    }
                }
                else if (newAction == "down" || newAction == "moveDownRot")
                {
                    targetPosition = avatar.transform.position + new Vector3(0.0f, 0.0f, -1.0f);
                    movement = true;
                    if (newAction == "moveDownRot")
                    {
                        rotation = true;
                    }

                }
                else if (newAction == "right" || newAction == "moveRightRot")
                {
                    targetPosition = avatar.transform.position + new Vector3(1.0f, 0.0f, 0.0f);
                    movement = true;
                    if (newAction == "moveRightRot")
                    {
                        rotation = true;
                    }



                }
                else if (newAction == "left" || newAction == "moveLeftRot")
                {
                    targetPosition = avatar.transform.position + new Vector3(-1.0f, 0.0f, 0.0f);
                    movement = true;
                    if (newAction == "moveLeftRot")
                    {
                        rotation = true;
                    }

                }

                else if (newAction == "rotRight")
                {
                    avatar.ChangeForward(new Vector3(1.0f, 0.0f, 0.0f));
                }
                else if (newAction == "rotLeft")
                {
                    avatar.ChangeForward(new Vector3(-1.0f, 0.0f, 0.0f));
                }
                else if (newAction == "rotUp")
                {
                    avatar.ChangeForward(new Vector3(0.0f, 0.0f, 1.0f));
                }
                else if (newAction == "rotDown")
                {
                    avatar.ChangeForward(new Vector3(0.0f, 0.0f, -1.0f));
                }

                else if (newAction == "eat")
                {
                    targetPosition = avatar.transform.position + new Vector3((int)avatar.transform.forward.x, (int)avatar.transform.forward.y, (int)avatar.transform.forward.z);
                }

                actionFinished = false;
                currentUnityInstruction++;

            }

            // This means that the aciton is still running
            if (!actionFinished)
            {
                if (newAction == "eat")
                {

                    Eat(targetPosition);
                    actionFinished = true;
                    ActualizeObservation();
                }

                else if (newAction == "left" || newAction == "moveLeftRot" || newAction == "right" || newAction == "moveRightRot" || newAction == "down" || newAction == "moveDownRot" || newAction == "up" || newAction == "moveUpRot")
                {
                    ActionExecuted(newAction, rotation, movement);
                }

                else if (newAction == "rotLeft" || newAction == "rotRight" || newAction == "rotUp" || newAction == "rotDown" || newAction == "idle")
                {
                    ActualizeObservation();
                    actionFinished = true;
                }

                else if (newAction == "notAchieved")
                {

                    float newFoodPositionX = socketManager.ReceiveNumber();
                    float newFoodPositionZ = socketManager.ReceiveNumber();
                    Vector3 pastObjectPosition = new Vector3(Food.transform.position.x, 0, Food.transform.position.z);
                    Food.transform.position = new Vector3(newFoodPositionX, Food.transform.position.y, newFoodPositionZ);

                    // Actualizing possible position. Maybe is better to do it inside the world class?
                    board.PossiblePositions.Add(pastObjectPosition);
                    board.PossiblePositions.Remove(new Vector3(newFoodPositionX, 0, newFoodPositionZ));
                    socketManager.ReceivedAction = "other";
                    actionFinished = true;
                }
            }

        }
    }

    private void Update()
    {
        // Manual movement
        if (Input.GetKeyDown("space"))
        {
            avatar.Senses.UseSightSense(avatar.transform);
            float glareFelt = avatar.Senses.Glare;
            float temperatureFelt = avatar.Senses.CalculateTemperature(board.AmbientTemperature, avatar.transform.position);
            float soundFelt = avatar.Senses.CalculateSound(avatar.transform.position);

            Debug.Log("Temp felt: " + temperatureFelt.ToString());
            Debug.Log("Sound felt: " + soundFelt.ToString());
            Debug.Log("Glare felt = " + glareFelt.ToString());
            Tuple<bool, float, float> tuple = CheckModifiableAction(newAction);

            avatar.ActualizeDrives(tuple, temperatureFelt, glareFelt, soundFelt);
        }



    }



    void ActionExecuted(string newAction, bool rotation, bool movement)
    {
        //This if checks if the avatar is already in his target position or not
        if (Vector3.Distance(avatar.transform.position, targetPosition) > 0.01f)
        {
            if (movement)
            {
                if (rotation)
                {
                    avatar.RotateToObject(targetPosition, movementSpeed * Time.deltaTime);
                }

                avatar.MoveToObject(targetPosition, movementSpeed * Time.deltaTime);
                //avatar.PlayAnimation("Speed", 0.5f);
            }
        }

        else
        {


            if (newAction == "moveUpRot")
            {
                avatar.ChangeForward(new Vector3(0.0f, 0.0f, 1.0f));
            }
            else if (newAction == "moveDownRot")
            {
                avatar.ChangeForward(new Vector3(0.0f, 0.0f, -1.0f));

            }
            else if (newAction == "moveRightRot")
            {
                avatar.ChangeForward(new Vector3(1.0f, 0.0f, 0.0f));
            }
            else if (newAction == "moveLeftRot")
            {
                avatar.ChangeForward(new Vector3(-1.0f, 0.0f, 0.0f));
            }

            ActualizeObservation();
            actionFinished = true;
        }




    }

    private void ActualizeObservation()
    {
        //SENDING OBSERVATION
        float temperatureFelt = avatar.Senses.CalculateTemperature(board.AmbientTemperature, avatar.transform.position);
        float soundFelt = avatar.Senses.CalculateSound(avatar.transform.position);

        socketManager.SendNewValue(temperatureFelt);
        socketManager.SendNewValue(avatar.Senses.Glare);
        socketManager.SendNewValue(soundFelt);

        //Sending the objects positions detected by smell and sight
        //sigtList
        socketManager.SendNewList(avatar.Senses.SightList, "sight");
        //HearingList
        socketManager.SendNewList(avatar.Senses.HearingList, "hearing");
        //touchLst
        socketManager.SendNewList(avatar.Senses.TouchList, "touch");


        Tuple<bool, float, float> tuple = CheckModifiableAction(newAction);

        avatar.ActualizeDrives(tuple, temperatureFelt, avatar.Senses.Glare, soundFelt);
        socketManager.SendNewValue(avatar.Drives.Health);
        socketManager.SendNewValue(avatar.Drives.Hunger);
        avatar.Drives.CheckRestart();

        if (newAction == "eat")
        {
            socketManager.SendNewPossiblePositions(board.PossiblePositions);
        }
        //OBSERVATION SENT

    }

    private Tuple<bool, float, float> CheckModifiableAction(string actionChecked)
    {
        Tuple<bool, float, float> tuple = new Tuple<bool, float, float>(false, 0, 0);
        if (actionChecked == "Eat")
        {
            if (modifiableobject != null)
            {
                if (modifiableobject.tag == "Apple")
                {
                    tuple = new Tuple<bool, float, float>(true, -50, 0);
                }
                else if (modifiableobject.tag == "Fire" || modifiableobject.tag == "Bulb" || modifiableobject.tag == "Speaker")
                {
                    tuple = new Tuple<bool, float, float>(true, 0, -20);
                }
                else
                {
                    tuple = new Tuple<bool, float, float>(false, 0, 30);
                }
            }
            else
            {
                tuple = new Tuple<bool, float, float>(false, 0, 0);
            }

        }

        return tuple;
    }


    private void Eat(Vector3 objectPosition)
    {
        List<Vector3> possiblePositions = board.PossiblePositions;
        possiblePositions.Remove(new Vector3(avatar.transform.position.x, 0, avatar.transform.position.z));


        possiblePositions.Add(new Vector3(avatar.transform.position.x, 0, avatar.transform.position.z));

        bool found = false;
        int counter = 0;
        while(!found && counter < avatar.Senses.SightList.Count)
        {
            IWorldObject worldObject = avatar.Senses.SightList[counter];
            if(worldObject.gameObject.transform.position.x == objectPosition.x && worldObject.gameObject.transform.position.z == objectPosition.z )
            {
                modifiableobject = worldObject.gameObject;
                if(worldObject.gameObject.tag == "Apple")
                {
                    board.ResetObject(worldObject);

                    avatar.Senses.UseSightSense(avatar.transform);
                }
                found = true;
            }
            else
            {
                counter++;
            }
        }

    }
}
