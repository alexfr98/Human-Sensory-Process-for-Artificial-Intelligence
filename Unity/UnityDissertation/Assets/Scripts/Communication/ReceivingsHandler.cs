using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// Handles receiving data from the network stream.
/// </summary>
public class ReceiverHandler
{
    private CommunicationHandler communicationHandler;

    /// <summary>
    /// Constructor for ReceiverHandler.
    /// </summary>
    /// <param name="networkStream">The network stream for communication.</param>
    public ReceiverHandler(NetworkStream networkStream)
    {
        communicationHandler = new CommunicationHandler(networkStream);
    }

    /// <summary>
    /// Receives an action from the network stream.
    /// </summary>
    /// <returns>The received action as a parsed string.</returns>
    public string ReceiveAction()
    {
        // Receive the action from the communication handler.
        string action = communicationHandler.ReceiveAction();
        // Debug log statements for received action (commented out).
        // Debug.Log("Inside actualize position, action received: " + action);
        // Debug.Log("Actualizing position with action: " + action);
        // Debug.Log("Actualize position action: " + action);
        // Parse and return the action.
        return ParseAction(action);
    }

    /// <summary>
    /// Receives a float number from the network stream.
    /// </summary>
    /// <returns>The received float number.</returns>
    public float ReceiveNumber()
    {
        // Receive a number from the communication handler.
        return communicationHandler.ReceiveNumber();
    }

    /// <summary>
    /// Parses the received action string.
    /// </summary>
    /// <param name="action">The action string to parse.</param>
    /// <returns>The parsed action string.</returns>
    private string ParseAction(string action)
    {
        // Variable to hold the parsed action.
        string actionParsed = "";
        bool founded = false; // Flag to indicate if the end of action string is found.
        int count = 0; // Counter to iterate through the action string.

        // Loop until the end of the action string is found.
        while (!founded)
        {
            if (action[count] == '0')
            {
                founded = true; // End of action string indicated by '0'.
            }
            else
            {
                // Append the current character to the parsed action.
                actionParsed = actionParsed + action[count];
                count++; // Move to the next character.
            }
        }
        // Return the parsed action string.
        return actionParsed;
    }
}
