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

}
