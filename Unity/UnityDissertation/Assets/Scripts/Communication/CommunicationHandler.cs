using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// Handles communication over a network stream, including sending and receiving data.
/// </summary>
public class CommunicationHandler
{
    private NetworkStream networkStream;

    /// <summary>
    /// Constructor for CommunicationHandler.
    /// </summary>
    /// <param name="networkStream">The network stream for communication.</param>
    public CommunicationHandler(NetworkStream networkStream)
    {
        this.networkStream = networkStream;
    }

    /// <summary>
    /// Receives an action string from the network stream.
    /// </summary>
    /// <returns>The received action string.</returns>
    public string ReceiveAction()
    {
        // The max size for an action = 16 bytes.
        int size = 16;
        byte[] buffer = new byte[size];
        // Receiving data from the host.
        int bytesRead = networkStream.Read(buffer, 0, size);
        string actionReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        return actionReceived;
    }

    /// <summary>
    /// Receives a float number from the network stream.
    /// </summary>
    /// <returns>The received float number.</returns>
    public float ReceiveNumber()
    {
        // The max size for a number = 16 bytes.
        int size = 16;
        byte[] buffer = new byte[size];
        // Receiving data from the host.
        int bytesRead = networkStream.Read(buffer, 0, size);
        string numberReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        // Convert the received string to a float and return it.
        return float.Parse(numberReceived);
    }

    /// <summary>
    /// Sends a number string to the network stream.
    /// </summary>
    /// <param name="number">The number string to send.</param>
    public void SendNumber(string number)
    {
        // Limit each number to 16 bytes.
        int maxBytes = 16;
        if (Encoding.ASCII.GetBytes(number).Length < maxBytes)
        {
            int difference = maxBytes - number.Length;

            // Append spaces to make the string 16 bytes long.
            for (int i = 0; i < difference; i++)
            {
                number = number + ' ';
            }
        }
        // Write the number to the network stream.
        networkStream.Write(Encoding.ASCII.GetBytes(number), 0, Encoding.ASCII.GetBytes(number).Length);
    }

    /// <summary>
    /// Sends a name string to the network stream.
    /// </summary>
    /// <param name="name">The name string to send.</param>
    public void SendName(string name)
    {
        // Limit each message to 16 bytes.
        int maxBytes = 16;
        if (Encoding.ASCII.GetBytes(name).Length < maxBytes)
        {
            int difference = maxBytes - name.Length;

            // Append dashes to make the string 16 bytes long.
            for (int i = 0; i < difference; i++)
            {
                name = name + "-";
            }
        }
        // Write the name to the network stream.
        networkStream.Write(Encoding.ASCII.GetBytes(name), 0, Encoding.ASCII.GetBytes(name).Length);
    }
}
