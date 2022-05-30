using System.Collections.Generic;
using System;
using UnityEngine;
public class Board 
{
    Token[] tokens;
    GameState gameState;
    MoveManager moveManager;

    public GameState getGameState()
    {
        return this.gameState;
    }

    public Board(Token[] tokens, GameState gameState, MoveManager moveManager)
    {
        this.tokens = tokens;
        this.gameState = gameState;
        this.moveManager = moveManager;
    }


    public TilemapStorage buildStorage(Token[] tokens)
    {
        TilemapStorage ts = new TilemapStorage();


        Array.Sort(tokens);

        foreach(Token token in tokens) {
            
            if(!token.isInTheBoard)
            {
                continue;
            }

            ts.Insert(token.GetPositionInTilemap(), token.getCorespondingTileInfo());
        }

        return ts;
    }
    // handle moves
    public List<Move> getPossibleMoves(int userId)
    {

        List<Move> totalMoves = new List<Move>();

        // this array cotains openning moves and avoids two openning moves with same insect type
        List<InsectType> openningMoves = new List<InsectType>();

        bool isQueenEntered = userId == 0 ? gameState.isUser1QueenEntered : gameState.isUser2QueenEntered;

        foreach (Token token in tokens)
        {

            // token belongs to openent 
            if(token.userId != userId)
            {
                continue;
            }

            // queen has not entered the game and current_token is in the game;
            if(token.isInTheBoard && !isQueenEntered)
            {
                continue;
            }


            bool isOpenningMove = !token.isInTheBoard;

            List<Vector3Int> posibleCandidatePositions = new List<Vector3Int>();

            if (isOpenningMove && openningMoves.Contains(token.type))
            {
                continue;
            }

            if (isOpenningMove)
            {
                openningMoves.Add(token.type);

                // get openning moves 

                posibleCandidatePositions = moveManager.GetOpenningMoves(token.type, token.userId, gameState.totalPiecesInGame == 1);

            }

            // token is not movable
            if (!isOpenningMove && !moveManager.isMovable(token.GetPositionInTilemap(), token.type == InsectType.Beetle, token.tokenId)) {
                continue;
            }

            Vector3Int tokenPosition = token.GetPositionInTilemap();

           

            
           
            if(!isOpenningMove)
            {
                switch (token.type)
                {
                    case InsectType.Ant:
                        posibleCandidatePositions = moveManager.GetAntCandidateDestinations(tokenPosition, token.tokenId);
                        break;
                    case InsectType.Beetle:
                        posibleCandidatePositions = moveManager.GetBeetleDestinations(tokenPosition);
                        break;
                    case InsectType.Grasshopper:
                        posibleCandidatePositions = moveManager.GetGrasshopperCandidateDestinations(tokenPosition);
                        break;
                    case InsectType.Queen:
                        posibleCandidatePositions = moveManager.GetQueenCandidateDestinations(tokenPosition);
                        break;
                    case InsectType.Spider:
                        posibleCandidatePositions = moveManager.GetSpiderCandidateDestinations(tokenPosition);
                        break;
                    default:
                        break;

                }
            }



            List<Vector3Int> finalCandidatePositions = new List<Vector3Int>();

            foreach(Vector3Int cv in posibleCandidatePositions)
            {

                if(moveManager.isValidPosition(token.type, token.userId, cv, isOpenningMove, token.tokenId, tokenPosition))
                {
                    Move move = new Move(token, tokenPosition, cv, isOpenningMove);

                    totalMoves.Add(move);
                    
                }

            }



        }

        return totalMoves;
    }

}
