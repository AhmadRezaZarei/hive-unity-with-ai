using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Evaluator
{
    private TilemapStorage storage;
    private GameState gameState;
    private MoveManager moveManager;
    public Evaluator()
    {
    }

    public void UpdateState(TilemapStorage storage, GameState gameState)
    {
        this.storage = storage;
        this.gameState = gameState;
        moveManager = new MoveManager(storage, gameState);
    }

    private static Evaluator instance;

    public static Evaluator GetInstance()
    {
        if(instance == null)
        {
            instance = new Evaluator();
        }

        return instance;
    }


    public int EvaluateState()
    {
        return GetSuroundingTokens(10) + GetMovableTokensCount(1) + GetSurrounderOfOponnetQueenCount();
    }

    private int GetSuroundingTokens(int centerTokenId)
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
            return 0;
        }

        var neighbors = storage.GetSurronudingTilePieces(tokenPos, 1);

        return neighbors.Count;

    }

    private int GetMovableTokensCount(int userId)
    {
        int movableTokenCount = 0;
        storage.ForEeachTiles((position, tileInfo , hIndex) =>
        {

            if(tileInfo.userId != userId)
            {
                return;
            }

            if(moveManager.isMovable(position, tileInfo.type == InsectType.Beetle, tileInfo.tokenId, false)) {
                movableTokenCount++;
            }


        });
        return movableTokenCount;
    }

    private (Vector3Int, bool) GetTokenPos(int tokenId)
    {
        bool found = false;
        Vector3Int targetPosition = Vector3Int.zero;
        
        storage.ForEeachTiles((position, tileInfo, hIndex) =>
        {
            if(tileInfo.tokenId == tokenId)
            {
                found = true;
                targetPosition = position;
            }
        });

        return (targetPosition, found);
    }

    // this function calculates count of tokens which can sourround the black queen in just 1 move
    private int GetSurrounderOfOponnetQueenCount()
    {

        int count = 0;

        if(gameState.isUser1QueenEntered)
        {
            return 0;
        }

        var (blackQueenPositon, found) = GetTokenPos(10);

        if(!found)
        {
            return 0;
        }

        List<Vector3Int> emptyPositions = storage.GetSurroundingEmptyTiles(blackQueenPositon, 1);
        Vector3Int[] blackQueenRingPositions = TilemapUtility.FindRing(blackQueenPositon, 1);


        storage.ForEeachTiles((position, tileInfo, hIndex) => {

            if(tileInfo.tokenId == 0)
            {
                return;
            }


            if(Array.Exists<Vector3Int>(blackQueenRingPositions, el => el == position))
            {
                return;
            }

            foreach (Vector3Int targetPos in emptyPositions)
            {
                if (moveManager.isValidPosition(tileInfo.type, tileInfo.userId, targetPos, false, tileInfo.tokenId, position))
                {
                    count++;
                    break;
                }
            }

        });
        



        return count;
    }



}
