using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public int currentUserTurnId = 0;

    public int leftTurnsToEnterUsers1Queen = 4;
    public int leftTurnsToEnterUsers2Queen = 4;

    public bool isUser1QueenEntered = false;

    public bool isUser2QueenEntered = false;

    public int totalTrurnsSinceStart = 0;

    public int totalPiecesInGame;

    public void setIsQueen1Entered(bool b, string w)
    {
        Debug.Log("GameStagte queen 1 " + w + "   " + b);
        this.isUser1QueenEntered = b;
    }

    public void setIsQueen2Entered(bool b, string w)
    {
        Debug.Log("GameStagte queen2" + w + "   " + b);
        this.isUser2QueenEntered = b;
    }



}
