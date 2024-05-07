using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ReceiverHandler
{
    private CommunicationHandler communicationHandler;
    NetworkStream networkStream;
    public ReceiverHandler(NetworkStream networkStream)
    {
        communicationHandler = new CommunicationHandler(networkStream);
        this.networkStream = networkStream;
    }

    public string ReceiveAction()
    {
        string action = communicationHandler.ReceiveAction();
        //Debug.Log("Inside actualize position, action received: " + action);
        //Debug.Log("Actualizing position with action: " + action);
        //Debug.Log("Actualize position action: " + action);
        return ParseAction(action); ;

    }

    public float ReceiveNumber()
    {
        return communicationHandler.ReceiveNumber();
    }

    private string ParseAction(string action)
    {
        string actionParsed = "";
        bool founded = false;
        int count = 0;
        while (!founded)
        {
            if (action[count] == '0')
            {
                founded = true;
            }
            else
            {
                actionParsed = actionParsed + action[count];
                count++;
            }
        }
        return actionParsed;

    }

}
