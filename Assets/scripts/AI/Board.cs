using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class Board 
{
    public Token[] tokens;
    GameState gameState;
    MoveManager moveManager;
    TilemapStorage storage;

    public GameState getGameState()
    {
        return this.gameState;
    }

    public Board(Token[] tokens, GameState gameState, MoveManager moveManager, TilemapStorage storage)
    {
        this.tokens = tokens;
        this.gameState = gameState;
        this.moveManager = moveManager;
        this.storage = storage;
    }


    public class myComparer : System.Collections.IComparer
    {
        public int Compare(object xx, object yy)
        {
            Token x = (Token)xx;
            Token y = (Token)yy;
            return x.tokenId.CompareTo(y.tokenId);
        }


    }

    public void AddMove(Move move)
    {


        Debug2.Log("is null ?::: move" + (move == null));

        if (this.storage == null)
        {
            buildStorage(tokens);
        }


        Array.Sort(tokens, new myComparer());


        Debug2.Log("Move is null tilestorage " + (move == null));

        var tokenObj = tokens[move.token.tokenId];
        tokenObj.isInTheBoard = true;
        tokenObj.x = move.to.x;
        tokenObj.y = move.to.y;


        if(tokenObj.type == InsectType.Beetle)
        {
            var pices = storage.GetPieces(move.to);

            if(pices != null)
            {
                tokenObj.hIndex = pices.Count;
            }
        }


        if (!move.isOpenningMove)
        {
            this.storage.Remove(move.from, move.token.tokenId);
        }
        
        this.storage.Insert(move.to, move.token.getCorespondingTileInfo());

        // update game state

        gameState.currentUserTurnId = move.token.userId == 0 ? 1 : 0;
        gameState.setIsQueen1Entered(move.token.userId == 0 && move.isOpenningMove && move.token.type == InsectType.Queen ? true : gameState.isUser1QueenEntered, "line 83");
        gameState.setIsQueen2Entered(move.token.userId == 1 && move.isOpenningMove && move.token.type == InsectType.Queen ? true : gameState.isUser2QueenEntered, "line 84");
        gameState.leftTurnsToEnterUsers1Queen += move.token.userId == 0 ? -1 : 0;
        gameState.leftTurnsToEnterUsers2Queen += move.token.userId == 1 ? -1 : 0;
        gameState.totalPiecesInGame += move.isOpenningMove ? 1 : 0;
        gameState.totalTrurnsSinceStart += 1;
        
    }

    public TilemapStorage GetTilemapStorage()
    {
        if(this.storage == null)
        {
            buildStorage(this.tokens);
        }

        return this.storage;
    }

    public void RemoveMove(Move move)
    {


        if (this.storage == null)
        {
            buildStorage(tokens);
        }

        Array.Sort(tokens, new myComparer());


        var tokenObj = tokens[move.token.tokenId];

        tokenObj.isInTheBoard = move.isOpenningMove ? false : true;
        tokenObj.x = move.from.x;
        tokenObj.y = move.from.y;

        if(tokenObj.type == InsectType.Beetle)
        {
            var pices = storage.GetPieces(move.from);
            
            if(pices == null)
            {
                tokenObj.hIndex = 0;
            } else
            {
                tokenObj.hIndex = pices.Count;
            }
        }

        this.storage.Remove(move.to, move.token.tokenId);
        
        if(!move.isOpenningMove)
        {
            this.storage.Insert(move.from, move.token.getCorespondingTileInfo());
        }


        // update game state
        gameState.currentUserTurnId = move.token.userId == 0 ? 0 : 1;
        gameState.setIsQueen1Entered(move.token.userId == 0 && move.isOpenningMove && move.token.type == InsectType.Queen ? false : gameState.isUser1QueenEntered, "line 141");
        gameState.setIsQueen2Entered(move.token.userId == 1 && move.isOpenningMove && move.token.type == InsectType.Queen ? false : gameState.isUser2QueenEntered, "line 142");
        gameState.leftTurnsToEnterUsers1Queen += move.token.userId == 0 ? +1 : 0;
        gameState.leftTurnsToEnterUsers2Queen += move.token.userId == 1 ? +1 : 0;
        gameState.totalPiecesInGame += move.isOpenningMove ? -1 : 0;
        gameState.totalTrurnsSinceStart += -1;
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

        this.storage = ts;

        return ts;
    }
    // handle moves
    public List<Move> getPossibleMoves(int userId)
    {

        Array.Sort(tokens, new myComparer());

        if (userId == 0)
        {
            int i = 0;
        }

        Debug2.Log("Board:=> board size is ");
        List<Move> totalMoves = new List<Move>();

        // this array cotains openning moves and avoids two openning moves with same insect type
        List<InsectType> openningMoves = new List<InsectType>();

        bool isQueenEntered = userId == 0 ? gameState.isUser1QueenEntered : gameState.isUser2QueenEntered;

        if(gameState.isUser1QueenEntered)
        {

        }

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


            if (isOpenningMove && openningMoves.Contains(token.type))
            {
                continue;
            }
            
            
            List<Vector3Int> posibleCandidatePositions = new List<Vector3Int>();

            if (isOpenningMove)
            {
                openningMoves.Add(token.type);

                // get openning moves 

                posibleCandidatePositions = moveManager.GetOpenningMoves(token.type, token.userId, gameState.totalPiecesInGame == 1);

            }

            bool isMovable = false;

            if(!isOpenningMove)
            {
                isMovable = moveManager.isMovable(token.GetPositionInTilemap(), token.type == InsectType.Beetle, token.tokenId);
            } else
            {
                isMovable = true;
            }

            
            if(!isOpenningMove )
            {
                Debug2.Log("Board:=> token-id, isMovable:=> " + token.tokenId + "  " + isMovable);
            }

            // token is not movable
            if (!isOpenningMove && !isMovable) {
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
