using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluator
{
    private TilemapStorage storage;
    private GameState gameState;
    public Evaluator(TilemapStorage storage, GameState gameState)
    {
        this.storage = storage;
        this.gameState = gameState;
    }

    public int evaluateState()
    {
        // 10 black queen token id
        // 21 white queen token id

        return getSuroundingTokens(10);
    }

    private int getSuroundingTokens(int centerTokenId)
    {
        Vector3Int tokenPos = Vector3Int.zero;
        bool foundedToken = false;

        storage.ForEeachTiles((position, tileInfo, hIndex) =>
        {
            if(tileInfo.tokenId == centerTokenId)
            {
                tokenPos = position;
                foundedToken = true;
            }

        });


        if(!foundedToken)
        {
            return -1;
        }

        var neighbors = storage.GetSurronudingTilePieces(tokenPos, 1);

        return neighbors.Count;

    }


}
