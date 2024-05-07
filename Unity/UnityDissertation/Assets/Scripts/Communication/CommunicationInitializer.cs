using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CommunicationInitializer
{
    SocketManager socketManager;
    public CommunicationInitializer(SocketManager socketManager)
    {
        this.socketManager = socketManager;
    }

    public void InitGame(Vector3 avatarPosition, Vector3 avatarForward)
    {

        socketManager.SendNewPosition(avatarPosition);
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


    public void InitPossiblePositions(WorldClass board)
    {
        socketManager.SendNewValue(board.Rows);
        socketManager.SendNewValue(board.Columns);
        //SendInfo("Init Pos", "string", network);
        socketManager.SendNewValue(board.PossiblePositions.Count);
        //Debug.Log("numberPossiblePositions = " + board.getPossiblePositions().Count);
        for (int position = 0; position < board.PossiblePositions.Count; position++)
        {

            socketManager.SendNewValue(board.PossiblePositions[position].x);
            socketManager.SendNewValue(board.PossiblePositions[position].z);
            Debug.Log("Possible position: " + board.PossiblePositions[position].x.ToString()
                + board.PossiblePositions[position].z.ToString());
        }
    }

}
