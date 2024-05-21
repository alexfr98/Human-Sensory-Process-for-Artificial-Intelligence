using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// Handles sending data over the network stream.
/// </summary>
public class SendingsHandler : MonoBehaviour
{
    private CommunicationHandler communicationHandler;

    /// <summary>
    /// Constructor for SendingsHandler.
    /// </summary>
    /// <param name="networkStream">The network stream for communication.</param>
    public SendingsHandler(NetworkStream networkStream)
    {
        communicationHandler = new CommunicationHandler(networkStream);
    }

    /// <summary>
    /// Sends a new position to the network stream.
    /// </summary>
    /// <param name="newPosition">The new position to send.</param>
    public void SendNewPosition(Vector3 newPosition)
    {
        // Send each coordinate of the position as a string.
        communicationHandler.SendNumber(newPosition.x.ToString());
        communicationHandler.SendNumber(newPosition.y.ToString());
        communicationHandler.SendNumber(newPosition.z.ToString());
    }

    /// <summary>
    /// Sends a new float value to the network stream.
    /// </summary>
    /// <param name="value">The float value to send.</param>
    public void SendNewValue(float value)
    {
        // Send the float value as a string.
        communicationHandler.SendNumber(value.ToString());
    }

    /// <summary>
    /// Sends a list of IWorldObject to the network stream.
    /// </summary>
    /// <param name="listToSend">The list of IWorldObject to send.</param>
    /// <param name="sendListName">The name of the list being sent.</param>
    public void SendNewList(List<IWorldObject> listToSend, string sendListName)
    {
        // Determine the type of list based on the name and send a corresponding identifier.
        if (sendListName == "sight")
        {
            communicationHandler.SendNumber(0.ToString());
        }
        else if (sendListName == "smell")
        {
            communicationHandler.SendNumber(1.ToString());
        }
        else if (sendListName == "hearing")
        {
            communicationHandler.SendNumber(2.ToString());
        }
        else if (sendListName == "taste")
        {
            communicationHandler.SendNumber(3.ToString());
        }
        else
        {
            // touch
            communicationHandler.SendNumber(4.ToString());
        }

        // Send the count of the list.
        communicationHandler.SendNumber(listToSend.Count.ToString());

        // Iterate through the list and send each object.
        for (int i = 0; i < listToSend.Count; i++)
        {
            if (listToSend[i] != null)
            {
                SendObject(listToSend[i].gameObject);
            }
        }
    }

    /// <summary>
    /// Sends a GameObject's relevant data to the network stream.
    /// </summary>
    /// <param name="gameObject">The GameObject to send.</param>
    private void SendObject(GameObject gameObject)
    {
        // Send the object's type.
        communicationHandler.SendName(gameObject.GetComponent<IWorldObject>().ObjectType);

        // Send the object's position.
        communicationHandler.SendNumber(gameObject.transform.position.x.ToString());
        communicationHandler.SendNumber(gameObject.transform.position.y.ToString());
        communicationHandler.SendNumber(gameObject.transform.position.z.ToString());

        // Send the distance to the object.
        communicationHandler.SendNumber(gameObject.GetComponent<IWorldObject>().Distance.ToString());
    }

    /// <summary>
    /// Sends a list of possible positions (Vector3) to the network stream.
    /// </summary>
    /// <param name="possiblePositions">List of Vector3 representing possible positions.</param>
    public void SendNewPossiblePositions(List<Vector3> possiblePositions)
    {
        // Send the count of possible positions.
        communicationHandler.SendNumber(possiblePositions.Count.ToString());

        // Iterate through each position and send its coordinates.
        for (int position = 0; position < possiblePositions.Count; position++)
        {
            communicationHandler.SendNumber(possiblePositions[position].x.ToString());
            communicationHandler.SendNumber(possiblePositions[position].z.ToString());
        }
    }
}
