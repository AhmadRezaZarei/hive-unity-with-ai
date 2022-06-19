using System.Collections.Generic;
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

        var (move, moveValue) = minimax(gameBoard, 4, true, int.MinValue, int.MaxValue);
        return move;
    }


    public bool IsGoodMove(Move move)
    {
        var tokenType = move.token.type;

        if(tokenType == InsectType.Beetle)
        {
            // check the beetle is going to our token

            var pieces = storage.GetPieces(move.to);

            if(pieces[pieces.Count -1].userId == move.token.userId)
            {
                return false;
            }

        }

        var (isQueenInTheBoard, queenPos) = storage.GetQueenPosition(move.token.userId);

        if (!isQueenInTheBoard)
        {
            return true;
        }

        var neighbors = TilemapUtility.FindRing(queenPos, 1);

        foreach (Vector3Int v in neighbors)
        {
            if (v == move.to)
            {
                return false;
            }
        }



        return true;
    }

    public (Move, int) minimax(Board board, int depth, bool isMaximizingPlayer, int alpha, int beta)
    {

        
        if(depth == 0)
        {

            Evaluator eval1 = Evaluator.GetInstance();
            eval1.UpdateState(gameBoard.GetTilemapStorage(), gameBoard.getGameState());
            int evalulatedStateValue = eval1.EvaluateState();

            return (null, evalulatedStateValue);
        }


        var posibleMoves = board.getPossibleMoves(isMaximizingPlayer ? 1 : 0);

        Move bestMove = null;

        for (int i = 0; i < posibleMoves.Count; i++)
        {

            Move move = posibleMoves[i];

            board.AddMove(move);

            var (_, rating) = minimax(board, depth - 1, !isMaximizingPlayer, alpha, beta);

            board.RemoveMove(move);

            // check is a good move or not 

            if(!IsGoodMove(move))
            {
                continue;
            }


            if (isMaximizingPlayer)
            {

                if(rating > alpha)
                {
                    alpha = rating;
                    bestMove = move;
                }

                if(alpha >= beta)
                {
                    return (bestMove, alpha);
                }


            } else
            {
                // minimizer player
                if (rating <= beta)
                {
                    beta = rating;
                    bestMove = move;
                }

                if (alpha >= beta)
                {
                    return (bestMove, beta);
                }
            }

        }

        return (bestMove, isMaximizingPlayer? alpha: beta);
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
