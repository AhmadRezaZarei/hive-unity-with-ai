using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
public class Agent
{

    private MoveManager moveManager;
    private TilemapStorage storage;
    private Board gameBoard;
    private List<Move> currentMoves;
    private Evaluator evaluator;

    public Agent(MoveManager manager, TilemapStorage storage, Board gameBoard)
    {
        this.moveManager = manager;
        this.storage = storage;
        this.gameBoard = gameBoard;

    }

    public List<Move> getCurrentMoves()
    {
        return currentMoves;
    }
    public Move getRandomMove()
    {
        printGameState();

        currentMoves = gameBoard.getPossibleMoves(1);
        Debug.Log("posible moves: " + currentMoves.Count);
        System.Random ran = new System.Random();
        int num = ran.Next() % currentMoves.Count;
        return currentMoves[num];
    }

    public Move getBestMove()
    {
        return findBestMove();
    }

    public int total = 0;
    public Move findBestMove()
    {

        var (move, moveValue) = minimax(gameBoard, 2, true);
        return move;
    }

    public (Move, int) minimax(Board board, int depth, bool isMaximizingPlayer)
    {

        var posibleMoves = board.getPossibleMoves(isMaximizingPlayer ? 1 : 0);

        Move bestMove = null;
        int bestMoveValue = isMaximizingPlayer ? int.MinValue : int.MaxValue;

        if(depth == 0)
        {
            for (int i = 0; i < posibleMoves.Count; i++)
            {
                Move move = posibleMoves[i];
                
                if (i == 0)
                {
                    bestMove = move;
                    gameBoard.AddMove(move);


                    Evaluator eval1 = new Evaluator(gameBoard.GetTilemapStorage(), gameBoard.getGameState());
                    int evalulatedStateValue = eval1.evaluateState();
                    bestMoveValue = evalulatedStateValue;
                    gameBoard.RemoveMove(move);

                    continue;
                }

                gameBoard.AddMove(move);
                Evaluator eval = new Evaluator(gameBoard.GetTilemapStorage(), gameBoard.getGameState());
                int evaluateStateValue = eval.evaluateState();
                gameBoard.RemoveMove(move);

     
                if (isMaximizingPlayer)
                {
                    if (bestMoveValue < evaluateStateValue)
                    {
                        bestMove = move;
                        bestMoveValue = evaluateStateValue;
                    }
                    continue;
                }

                if (bestMoveValue > evaluateStateValue)
                {
                    bestMove = move;
                    bestMoveValue = evaluateStateValue;
                }

            }

            return (bestMove, bestMoveValue);
        }



        for(int i = 0; i < posibleMoves.Count; i++)
        {

            Move move = posibleMoves[i];

            board.AddMove(move);

            
            var (bestMoveMinimax, bestMoveValueMinimax) = minimax(board, depth - 1, !isMaximizingPlayer);

            board.RemoveMove(move);

            if (isMaximizingPlayer)
            {

                if(bestMoveValueMinimax > bestMoveValue)
                {
                    bestMoveValue = bestMoveValueMinimax;
                    bestMove = move;
                }

            } else
            {
                if (bestMoveValueMinimax < bestMoveValue)
                {
                    bestMoveValue = bestMoveValueMinimax;
                    bestMove = move;
                }
            }

        }

        return (bestMove, bestMoveValue);
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
