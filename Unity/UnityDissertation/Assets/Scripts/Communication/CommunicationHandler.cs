using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class CommunicationHandler
{
    NetworkStream networkStream;
    public CommunicationHandler(NetworkStream networkStream) {
        this.networkStream = networkStream;
    }
    public string ReceiveAction()
    {
        //The max size for an action = 10
        int size = 16;
        byte[] buffer = new byte[size];
        //Receiving Data from the Host
        int bytesRead = networkStream.Read(buffer, 0, size);
        string actionReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); ;
        return actionReceived;
    }


    public float ReceiveNumber()
    {
        //The max size for an action = 10
        int size = 16;
        byte[] buffer = new byte[size];
        //Receiving Data from the Host
        int bytesRead = networkStream.Read(buffer, 0, size);
        string numberReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); ;
        //Debug.Log("Number received: " + numberReceived);
        return float.Parse(numberReceived);
    }

    public void SendNumber(string number)
    {
        //We limit each number to 16 bytes
        int maxBytes = 16;
        if (Encoding.ASCII.GetBytes(number).Length < maxBytes)
        {

            int difference = maxBytes - number.Length;

            //Here we are doing a loop that made us lose some time. Change it later
            for (int i = 0; i < difference; i++)
            {
                number = number + ' ';
            }
        }
        networkStream.Write(Encoding.ASCII.GetBytes(number), 0, Encoding.ASCII.GetBytes(number).Length);
    }

    public void SendName(string name)
    {
        //We limit each message to 16 bytes
        int maxBytes = 16;
        if (Encoding.ASCII.GetBytes(name).Length < maxBytes)
        {

            int difference = maxBytes - name.Length;

            //Here we are doing a loop that made us lose some time. Change it later
            for (int i = 0; i < difference; i++)
            {
                name = name + "-";
            }
        }
        networkStream.Write(Encoding.ASCII.GetBytes(name), 0, Encoding.ASCII.GetBytes(name).Length);
    }


}
