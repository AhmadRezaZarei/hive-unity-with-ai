using System.Collections.Generic;
using System;
public class Board 
{
    Token[] tokens;
    int user1Turns;
    int user2Turns;
    bool isUser1QueenEntered;
    bool isUser2QueenEntered;

    public Board(Token[] tokens, int user1Turns, int user2Turns, bool isUser1QueenEntered, bool isUser2QueenEntered)
    {
        this.tokens = tokens;
        this.user1Turns = user1Turns;
        this.user2Turns = user2Turns;
        this.isUser1QueenEntered = isUser1QueenEntered;
        this.isUser2QueenEntered = isUser2QueenEntered;
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

            ts.Insert(token.getPositionInTilemap(), token.getCorespondingTileInfo());
        }

        return ts;
    }
    // handle moves
    public List<Move> getPossibleMoves(int userId)
    {

        foreach(Token token in tokens)
        {

            if(token.userId != userId)
            {
                continue;
            }



        }


        return null;
    }

}
