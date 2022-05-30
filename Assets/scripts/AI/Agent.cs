using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
public class Agent
{

    private MoveManager moveManager;
    private TilemapStorage storage;
    private Board gameBoard;
    public Agent(MoveManager manager, TilemapStorage storage, Board gameBoard)
    {
        this.moveManager = manager;
        this.storage = storage;
        this.gameBoard = gameBoard;
    }

    public Move getRandomMove()
    {

        printGameState();
        List<Move> totalMoves = gameBoard.getPossibleMoves(1);
        Debug.Log("posible moves: " +  totalMoves.Count);
        System.Random ran = new System.Random();
        int num = ran.Next() % totalMoves.Count;
        return totalMoves[num];
    }

    private void printGameState()
    {
        GameState gs = gameBoard.getGameState();
        Debug.Log("@ @ @ @ @ @");
        Debug.Log("@ isUser1QueenEntered: " + gs.isUser1QueenEntered);
        Debug.Log("@ isUser2QueenEntered: " + gs.isUser2QueenEntered);
        Debug.Log("@ leftTurnsToEnterUsers1Queen: " + gs.leftTurnsToEnterUsers1Queen);
        Debug.Log("@ leftTurnsToEnterUsers2Queen: " + gs.leftTurnsToEnterUsers2Queen);
        Debug.Log("@ totalPiecesInGame: " + gs.totalPiecesInGame);
        Debug.Log("@ totalTrurnsSinceStart: " + gs.totalTrurnsSinceStart);
        Debug.Log("# # # # #");
    }
}
