using UnityEngine;

/// <summary>
/// Handles the initialization of communication by sending initial game states and possible positions.
/// </summary>
public class CommunicationInitializer
{
    // Reference to the SocketManager instance.
    private SocketManager socketManager;

    /// <summary>
    /// Constructor for CommunicationInitializer.
    /// </summary>
    /// <param name="socketManager">Instance of SocketManager to handle socket communications.</param>
    public CommunicationInitializer(SocketManager socketManager)
    {
        this.socketManager = socketManager;
    }

    /// <summary>
    /// Initializes the game by sending the avatar's position and forward direction.
    /// </summary>
    /// <param name="avatarPosition">The position of the avatar.</param>
    /// <param name="avatarForward">The forward direction of the avatar.</param>
    public void InitGame(Vector3 avatarPosition, Vector3 avatarForward)
    {
        // Send the avatar's initial position.
        socketManager.SendNewPosition(avatarPosition);

        // Check the avatar's forward direction and send corresponding values.
        if (avatarForward == new Vector3(1, 0, 0))
        {
            Debug.Log("rotation X");
            socketManager.SendNewValue(1);
            socketManager.SendNewValue(0);
        }
        else if (avatarForward == new Vector3(-1, 0, 0))
        {
            Debug.Log("rotation -X");
            socketManager.SendNewValue(-1);
            socketManager.SendNewValue(0);
        }
        else if (avatarForward == new Vector3(0, 0, 1))
        {
            Debug.Log("rotation Z");
            socketManager.SendNewValue(0);
            socketManager.SendNewValue(1);
        }
        else if (avatarForward == new Vector3(0, 0, -1))
        {
            Debug.Log("rotation -Z");
            socketManager.SendNewValue(0);
            socketManager.SendNewValue(-1);
        }
        else
        {
            Debug.Log("rotation??");
            socketManager.SendNewValue(0);
            socketManager.SendNewValue(0);
        }
    }

    /// <summary>
    /// Sends the possible positions on the board to the client.
    /// </summary>
    /// <param name="board">The board containing possible positions.</param>
    public void InitPossiblePositions(WorldClass board)
    {
        // Send the number of rows and columns of the board.
        socketManager.SendNewValue(board.Rows);
        socketManager.SendNewValue(board.Columns);

        // Send the number of possible positions.
        socketManager.SendNewValue(board.PossiblePositions.Count);

        // Iterate through each possible position and send their coordinates.
        for (int position = 0; position < board.PossiblePositions.Count; position++)
        {
            socketManager.SendNewValue(board.PossiblePositions[position].x);
            socketManager.SendNewValue(board.PossiblePositions[position].z);
            Debug.Log("Possible position: " + board.PossiblePositions[position].x.ToString()
                + board.PossiblePositions[position].z.ToString());
        }
    }
}
