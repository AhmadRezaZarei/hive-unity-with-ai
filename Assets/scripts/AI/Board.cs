using System.Collections.Generic;
using System;
using UnityEngine;
public class Board 
{
    Token[] tokens;
    GameState gameState;
    MoveManager moveManager;

    public Board(Token[] tokens, GameState gameState, MoveManager moveManager)
    {
        this.tokens = tokens;
        this.gameState = gameState;
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

        List<Move> moves = new List<Move>();


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


            // token is not movable
            if (!moveManager.isMovable(token.GetPositionInTilemap(), token.type == InsectType.Beetle, token.tokenId)) {
                continue;
            }


            switch(token.type)
            {
                case InsectType.Ant:
                    break;
                case InsectType.Beetle:
                    break;
                case InsectType.Grasshopper:
                    break;
                case InsectType.Queen:
                    break;
                case InsectType.Spider:
                    break;
                default:
                    break;

            }

            


        }


        return null;
    }

}
