using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

/// <summary>
/// Manages TCP socket communication.
/// </summary>
public class SocketManager
{
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25003;
    private bool initReceived = false;
    public bool InitReceived { get { return initReceived; } }

    private NetworkStream networkStream;

    private string receivedAction = "init";
    public string ReceivedAction { get { return receivedAction; } set { receivedAction = value; } }
    private int numberInstructionsPython = 1;
    public int NumberInstructionsPython { get { return numberInstructionsPython; } }

    private TcpListener listener;
    private TcpClient client;

    private bool running;

    private ReceiverHandler receiverHandler;
    private SendingsHandler sendingsHandler;
    private CommunicationInitializer communicationInitializer;

    /// <summary>
    /// Constructor for SocketManager.
    /// </summary>
    public SocketManager()
    {

    }

    /// <summary>
    /// Initializes the TCP listener and accepts a client connection.
    /// </summary>
    private void Initialize()
    {
        // Create a TCP listener to listen for incoming connections on the specified port.
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start(); // Start the listener.

        // Accept a client connection. This call blocks until a client connects.
        client = listener.AcceptTcpClient();
        running = true;

        // Get the network stream for data transfer.
        networkStream = client.GetStream();
        receiverHandler = new ReceiverHandler(networkStream); // Initialize receiver handler with the network stream.
        sendingsHandler = new SendingsHandler(networkStream); // Initialize sending handler with the network stream.
        communicationInitializer = new CommunicationInitializer(this); // Initialize communication initializer.
    }

    /// <summary>
    /// Starts the communication thread.
    /// </summary>
    /// <param name="board">WorldClass object representing the board.</param>
    /// <param name="avatarPosition">Vector3 representing the avatar's position.</param>
    /// <param name="avatarForward">Vector3 representing the avatar's forward direction.</param>
    public void StartCommunication(WorldClass board, Vector3 avatarPosition, Vector3 avatarForward)
    {
        // Start a new thread to handle communication.
        Thread threat = new Thread(() => GetInfo(board, avatarPosition, avatarForward));
        threat.Start();
    }

    /// <summary>
    /// Initializes the connection and continuously checks for incoming data.
    /// </summary>
    /// <param name="board">WorldClass object representing the board.</param>
    /// <param name="avatarPosition">Vector3 representing the avatar's position.</param>
    /// <param name="avatarForward">Vector3 representing the avatar's forward direction.</param>
    public void GetInfo(WorldClass board, Vector3 avatarPosition, Vector3 avatarForward)
    {
        Initialize(); // Initialize the connection.

        // Main loop to check for data and handle communication.
        while (running)
        {
            if (networkStream != null)
            {
                // Check if initial data has been received.
                if (!initReceived)
                {
                    // Initialize game and positions.
                    communicationInitializer.InitGame(avatarPosition, avatarForward);
                    communicationInitializer.InitPossiblePositions(board);
                    initReceived = true;
                }
                else
                {
                    // Receive action if not in "notAchieved" state.
                    if (receivedAction != "notAchieved")
                    {
                        ReceiveAction();
                    }
                }
            }
            else
            {
                initReceived = false; // Reset initial received flag if network stream is null.
            }
        }
        listener.Stop(); // Stop the listener when done.
    }

    /// <summary>
    /// Receives an action from the network stream.
    /// </summary>
    public void ReceiveAction()
    {
        // Receive an action from the receiver handler.
        string action = receiverHandler.ReceiveAction();
        if (action != null && action != "")
        {
            receivedAction = action; // Update received action.
            numberInstructionsPython++; // Increment the number of instructions received from Python.
        }
    }

    /// <summary>
    /// Receives a float number from the network stream.
    /// </summary>
    /// <returns>Received float number.</returns>
    public float ReceiveNumber()
    {
        return receiverHandler.ReceiveNumber();
    }

    /// <summary>
    /// Sends a new position to the network stream.
    /// </summary>
    /// <param name="newPosition">Vector3 representing the new position.</param>
    public void SendNewPosition(Vector3 newPosition)
    {
        sendingsHandler.SendNewPosition(newPosition);
    }

    /// <summary>
    /// Sends a new float value to the network stream.
    /// </summary>
    /// <param name="value">Float value to send.</param>
    public void SendNewValue(float value)
    {
        sendingsHandler.SendNewValue(value);
    }

    /// <summary>
    /// Sends a list of IWorldObject to the network stream.
    /// </summary>
    /// <param name="listToSend">List of IWorldObject to send.</param>
    /// <param name="sendListName">Name of the list being sent.</param>
    public void SendNewList(List<IWorldObject> listToSend, string sendListName)
    {
        sendingsHandler.SendNewList(listToSend, sendListName);
    }

    /// <summary>
    /// Sends a list of ITemperatureEmitter to the network stream.
    /// </summary>
    /// <param name="listToSend">List of ITemperatureEmitter to send.</param>
    /// <param name="sendListName">Name of the list being sent.</param>
    public void SendNewList(List<ITemperatureEmitter> listToSend, string sendListName)
    {
        List<IWorldObject> list = new List<IWorldObject>();
        for (int i = 0; i < listToSend.Count; i++)
        {
            list.Add(listToSend[i]);
        }
        sendingsHandler.SendNewList(list, sendListName);
    }

    /// <summary>
    /// Sends a list of ISoundEmitter to the network stream.
    /// </summary>
    /// <param name="listToSend">List of ISoundEmitter to send.</param>
    /// <param name="sendListName">Name of the list being sent.</param>
    public void SendNewList(List<ISoundEmitter> listToSend, string sendListName)
    {
        List<IWorldObject> list = new List<IWorldObject>();
        for (int i = 0; i < listToSend.Count; i++)
        {
            list.Add(listToSend[i]);
        }
        sendingsHandler.SendNewList(list, sendListName);
    }

    /// <summary>
    /// Sends a list of possible positions (Vector3) to the network stream.
    /// </summary>
    /// <param name="possiblePositions">List of Vector3 representing possible positions.</param>
    public void SendNewPossiblePositions(List<Vector3> possiblePositions)
    {
        sendingsHandler.SendNewPossiblePositions(possiblePositions);
    }
}
