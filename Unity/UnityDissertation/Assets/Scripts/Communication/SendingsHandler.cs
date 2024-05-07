using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SendingsHandler : MonoBehaviour
{
    private CommunicationHandler communicationHandler;
    NetworkStream networkStream;
    public SendingsHandler(NetworkStream networkStream)
    {
        communicationHandler = new CommunicationHandler(networkStream);
        this.networkStream = networkStream;
    }


    public void SendNewPosition(Vector3 newPosition)
    {
        //Debug.Log("checking new pos");
        //Debug.Log(newPosition.x.ToString() + newPosition.y.ToString() + newPosition.z.ToString());
        communicationHandler.SendNumber(newPosition.x.ToString());
        communicationHandler.SendNumber(newPosition.y.ToString());
        communicationHandler.SendNumber(newPosition.z.ToString());
    }

    public void SendNewValue(float value)
    {
        communicationHandler.SendNumber(value.ToString());
    }
    public void SendNewList(List<IWorldObject> listToSend, string sendListName)
    {
        //Debug.Log("Send new List");
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
            //touch
            communicationHandler.SendNumber(4.ToString());
        }
        //Debug.Log("Sense: " + sendListName);
        //foreach (GameObject go in listToSend)
        //{
        //    Debug.Log("Position object " + go.gameObject.name + " = " + go.transform.position.x.ToString() + go.transform.position.z.ToString());
        //}

        //Debug.Log("List name " + sendListName);
        communicationHandler.SendNumber(listToSend.Count.ToString());


        //Debug.Log("Length " + listToSend.Count.ToString());
        //Debug.Log("Info sended newList: " + listToSend.Count.ToString());
        for (int i = 0; i < listToSend.Count; i++)
        {
            if (listToSend[i] != null)
            {
                //Debug.Log("Character sended = " + listToSend[i].GetComponent<ObjectClass>().Character);
                //Debug.Log("Position object " + listToSend[i].gameObject.name + " = " + listToSend[i].transform.position.x.ToString() + listToSend[i].transform.position.y.ToString() + listToSend[i].transform.position.z.ToString());

                SendObject(listToSend[i].gameObject);

            }
        }

    }

    private void SendObject(GameObject gameObject)
    {
        communicationHandler.SendName(gameObject.GetComponent<IWorldObject>().ObjectType);
        //Sending object position
        communicationHandler.SendNumber(gameObject.transform.position.x.ToString());
        communicationHandler.SendNumber(gameObject.transform.position.y.ToString());
        communicationHandler.SendNumber(gameObject.transform.position.z.ToString());

        //Sending distance to the object
        communicationHandler.SendNumber(gameObject.GetComponent<IWorldObject>().Distance.ToString());

    }

    public void SendNewPossiblePositions(List<Vector3> possiblePositions)
    {
        communicationHandler.SendNumber(possiblePositions.Count.ToString());
        //Debug.Log("numberPossiblePositions = " + board.getPossiblePositions().Count);
        for (int position = 0; position < possiblePositions.Count; position++)
        {

            communicationHandler.SendNumber(possiblePositions[position].x.ToString());
            communicationHandler.SendNumber(possiblePositions[position].z.ToString());
            //Debug.Log("Possible position: " + board.getPossiblePositions()[position].x.ToString()
            //    + board.getPossiblePositions()[position].z.ToString());
        }
    }
}
