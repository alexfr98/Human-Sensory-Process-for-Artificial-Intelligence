using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

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
    public SocketManager() { 
        
    }
    private void Initialize()
    {

        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();
        running = true;


        networkStream = client.GetStream();
        receiverHandler = new ReceiverHandler(networkStream);
        sendingsHandler = new SendingsHandler(networkStream);
        communicationInitializer = new CommunicationInitializer(this);
    }
    public void StartCommunication(WorldClass board, Vector3 avatarPosition, Vector3 avatarForward)
    {
        //ThreadStart ts = new ThreadStart(GetInfo);
        Thread threat = new Thread(() => GetInfo(board, avatarPosition, avatarForward));
        threat.Start();
    }


    public void GetInfo(WorldClass board, Vector3 avatarPosition, Vector3 avatarForward)
    {
        Initialize();
        while (running)
        {
            if (networkStream != null)
            {
                if (!initReceived)
                {
                    communicationInitializer.InitGame(avatarPosition, avatarForward);
                    communicationInitializer.InitPossiblePositions(board);
                    initReceived = true;
                }

                else
                {
                    if (receivedAction != "notAchieved")
                    {
                        ReceiveAction();
                    }
                }

            }

            else
            {
                initReceived = false;
            }

        }
        listener.Stop();

    }


    public void ReceiveAction()
    {
        string action = receiverHandler.ReceiveAction();
        if (action != null && action != "")
        {
            receivedAction = action;
            numberInstructionsPython++;
        }
    }

    public float ReceiveNumber()
    {
        return receiverHandler.ReceiveNumber();
    }

    public void SendNewPosition(Vector3 newPosition)
    {
        sendingsHandler.SendNewPosition(newPosition);
    }

    public void SendNewValue(float value)
    {
        sendingsHandler.SendNewValue(value);
    }
    public void SendNewList(List<IWorldObject> listToSend, string sendListName)
    {
        sendingsHandler.SendNewList(listToSend, sendListName);
    }
    public void SendNewList(List<ITemperatureEmitter> listToSend, string sendListName)
    {
        List<IWorldObject> list = new List<IWorldObject>();
        for(int i= 0; i < listToSend.Count; i++) {
            list.Add(listToSend[i]);
        }
        sendingsHandler.SendNewList(list, sendListName);
    }
    public void SendNewList(List<ISoundEmitter> listToSend, string sendListName)
    {
        List<IWorldObject> list = new List<IWorldObject>();
        for (int i = 0; i < listToSend.Count; i++)
        {
            list.Add(listToSend[i]);
        }
        sendingsHandler.SendNewList(list, sendListName);
    }


    public void SendNewPossiblePositions(List<Vector3> possiblePositions)
    {
        sendingsHandler.SendNewPossiblePositions(possiblePositions);
    }

}
