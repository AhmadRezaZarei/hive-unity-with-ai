using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager
{

    TilemapStorage tilemapStorage;
    GameState gameState;
    public MoveManager(TilemapStorage storage, GameState gameState)
    {
        this.tilemapStorage = storage;
        this.gameState = gameState;
    }

    public MoveManager Clone(TilemapStorage ts)
    {
        return new MoveManager(ts, gameState);
    }

    private List<Vector3Int> DepthLimitedDistinitions(Vector3Int position, Vector3Int except, HashSet<string> visited, HashSet<string> reachedPositionsSet, int currentDepth)
    {

        List<Vector3Int> res = new List<Vector3Int>();

        var emptyNeighbors = tilemapStorage.GetSurroundingEmptyTiles(position, 1);

        foreach (var pos in emptyNeighbors)
        {
            if (pos.GetUniqueKey() != except.GetUniqueKey())
            {

                if (!reachedPositionsSet.Contains(pos.GetUniqueKey()))
                {

                    if (visited.Contains(pos.GetUniqueKey()))
                    {
                        continue;
                    }

                    if (!hasNoneEmptyNeighboar(pos, except))
                    {
                        continue;
                    }

                    visited.Add(pos.GetUniqueKey());

                    if (currentDepth == 2)
                    {
                        reachedPositionsSet.Add(pos.GetUniqueKey());
                        res.Add(pos);
                    }
                    else
                    {
                        res.AddRange(DepthLimitedDistinitions(pos, except, visited, reachedPositionsSet, currentDepth + 1));
                    }

                }

            }
        }

        return res;
    }

    public List<Vector3Int> GetOpenningMoves(InsectType type, int userId, bool isFirstMove)
    {

        List<Vector3Int> candidates = new List<Vector3Int>();
        HashSet<string> visited = new HashSet<string>();

        tilemapStorage.ForEeachTiles((position, tileInfo, hIndex) => {

            if (isFirstMove)
            {
                Vector3Int[] candidatesArray = TilemapUtility.FindRing(position, 1);

                foreach (Vector3Int c in candidatesArray)
                {
                    candidates.Add(c);
                    return;
                }

                return;
            }

            if (tileInfo.userId != userId)
            {
                return;
            }

            Vector3Int[] ring = TilemapUtility.FindRing(position, 1);

            foreach(Vector3Int v in ring)
            {
                if(!visited.Contains(v.GetUniqueKey()) && isValidForOpenningMove(type, userId, v))
                {
                    candidates.Add(v);
                    visited.Add(v.GetUniqueKey());
                }
            }


        });

        return candidates;
    }

    public List<Vector3Int> GetAntCandidateDestinations(Vector3Int antPosition, int tokenId)
    {
        List<Vector3Int> candiates = new List<Vector3Int>();
        HashSet<string> added = new HashSet<string>();

        tilemapStorage.ForEeachTiles((position, tileInfo, hIndex) => {
            
            if(antPosition.isXYEqual(position))
            {
                return;
            }

            Vector3Int[] ring = TilemapUtility.FindRing(position, 1);
            
            foreach(Vector3Int v in ring)
            {
                
                if(added.Contains(v.GetUniqueKey()))
                {
                    continue;
                }

                if(IsValidMoveForAnt(v, tokenId))
                {
                    added.Add(v.GetUniqueKey());
                    candiates.Add(v);
                }

            }

        });

        return candiates;
    }

    public List<Vector3Int> GetSpiderCandidateDestinations(Vector3Int spiderPosition)
    {

        var list = DepthLimitedDistinitions(spiderPosition, spiderPosition, new HashSet<string>(), new HashSet<string>(), 0);

        //temp_hint(list);


        return list;
    }

    public bool isMovable(Vector3Int initialPosition, bool isBeetle, int tokenId,bool isOpenningMove)
    {


        if(isOpenningMove)
        {
            return true;
        }

        HashSet<int> visited = new HashSet<int>();

        List<Vector3Int> neighbors = tilemapStorage.GetSurroundingNoneEmptyTiles(initialPosition, 1);
        if (neighbors.Count == 0)
        {
            return false;
        }

        Vector3Int exceptPosition = initialPosition;

        if (isBeetle)
        {
            // todo check here 

            List<TileInfo> pices = tilemapStorage.GetPieces(initialPosition);

            // preve code 
            // if (pices.Count > 1 && pices[pices.Count - 1].tokenId == tokenId)
            //{
            //  return true;
            //}
            //
            //

            if(pices == null)
            {
                // log the state of tilestorage 
                // bug => initialPosition is null for beetle
                Debug.Log("MoveManager ");

            }

            if (pices.Count > 1 && pices[pices.Count - 1].tokenId == tokenId)
            {
                return true;
            }
        }

        MovableHelperDFS(neighbors[0], exceptPosition, visited);


        if (visited.Count != gameState.totalPiecesInGame - 1)
        {
            return false;
        }


        return true;
    }

    private void MovableHelperDFS(Vector3Int center, Vector3Int except, HashSet<int> visited)
    {

        var pieces = tilemapStorage.GetPieces(center);

        foreach (var p in pieces)
        {
            if (!visited.Contains(p.tokenId))
            {
                visited.Add(p.tokenId);
            }
        }

        List<Vector3Int> neighbors = tilemapStorage.GetSurroundingNoneEmptyTiles(center, 1);

        foreach (Vector3Int v in neighbors)
        {
            if (!v.GetUniqueKey().Equals(except.GetUniqueKey()))
            {

                var nPieces = tilemapStorage.GetPieces(v);

                if (nPieces != null && nPieces.Count != 0)
                {

                    bool isVisitedAll = true;

                    foreach (var p in nPieces)
                    {
                        if (!visited.Contains(p.tokenId))
                        {
                            isVisitedAll = false;
                            visited.Add(p.tokenId);
                        }

                    }

                    if (!isVisitedAll)
                    {
                        MovableHelperDFS(v, except, visited);
                    }
                }

            }

        }

    }

    private bool IsValidMoveForAnt(Vector3Int tilemapPosition, int tokenId)
    {

        if (!tilemapStorage.isEmptyTile(tilemapPosition))
        {
            return false;
        }

        var neighboars = tilemapStorage.GetSurronudingTilePieces(tilemapPosition, 1);


        bool isCurrentPiesIncluded = false;
        foreach (TileInfo t in neighboars)
        {
            if (t.tokenId == tokenId)
            {
                isCurrentPiesIncluded = true;
                break;
            }
        }


        if (isCurrentPiesIncluded)
        {
            if (neighboars.Count > 5 || neighboars.Count == 0)
            {
                return false;
            }


            return true;
        }

        if (neighboars.Count > 4 || neighboars.Count == 0)
        {
            return false;
        }

        return true;
    }

    private bool ContainsItem(List<Vector3Int> list, Vector3Int toSearch)
    {

        foreach (var v in list)
        {
            if (v.GetUniqueKey() == toSearch.GetUniqueKey())
            {
                return true;
            }
        }

        return false;
    }

    public bool isValidPosition(InsectType type, int userId, Vector3Int destinationPosition, bool isOpenningMove, int tokenId, Vector3Int initialPosition)
    {


        Debug2.Log("# # # # #");
        Debug2.Log("MoveManager:=> line 297");

        if (!isOpenningMove)
        {

            Debug2.Log("MoveManager:=> line 302");

            // this tells us the token is under the beetle or not
            if (!tilemapStorage.canMove(initialPosition, tokenId))
            {
                Debug2.Log("MoveManager:=> line 307");
                return false;
            }
            Debug2.Log("MoveManager:=> line 310");

        }

        Debug2.Log("MoveManager:=> line 315");

        int leftTurnsToEnterQueen = userId == 0 ? gameState.leftTurnsToEnterUsers1Queen : gameState.leftTurnsToEnterUsers2Queen;
        bool isQueenEntered = userId == 0 ? gameState.isUser1QueenEntered : gameState.isUser2QueenEntered;

        Debug2.Log("MoveManager:=> line 319" + isQueenEntered + "   " + isOpenningMove);

        if (!isQueenEntered && !isOpenningMove)
        {
            Debug2.Log("MoveManager:=> line 323");
            return false;
        }


        Debug2.Log("MoveManager:=> line 328");

        if (leftTurnsToEnterQueen == 1 && !isQueenEntered && type != InsectType.Queen)
        {
            Debug2.Log("MoveManager:=> line 332");
            return false;
        }

        Debug2.Log("MoveManager:=> line 341");

        if (gameState.totalTrurnsSinceStart == 0)
        {
            return true;
        }



        Debug2.Log("MoveManager:=> line 350");

        if (gameState.totalTrurnsSinceStart == 1)
        {

            Debug2.Log("MoveManager:=> line 355");

            var neighoars = tilemapStorage.GetSurronudingTilePieces(destinationPosition, 1);

            if (neighoars.Count == 0)
            {
                return false;
            }


            Debug2.Log("MoveManager:=> line 365");

            if (neighoars[0].userId == userId)
            {
                return false;
            }


            Debug2.Log("MoveManager:=> line 373");

            return true;
        }


        Debug2.Log("MoveManager:=> line 379");

        if (isOpenningMove)
        {
            return isValidForOpenningMove(type, userId, destinationPosition);
        }


        Debug2.Log("MoveManager:=> line 387");

        if (!isMovable(initialPosition, type == InsectType.Beetle, tokenId, isOpenningMove) && !isOpenningMove)
        {
            //isMovable(initialPosition, type == InsectType.Beetle, tokenId);
            return false;
        }


        Debug2.Log("MoveManager:=> line 395");

        if (type == InsectType.Ant && !isOpenningMove)
        {
            return IsValidMoveForAnt(destinationPosition, tokenId);
        }


        Debug2.Log("MoveManager:=> line 403");

        if (type == InsectType.Spider && !isOpenningMove)
        {

            var candidatePositions = GetSpiderCandidateDestinations(initialPosition);

            return ContainsItem(candidatePositions, destinationPosition);

        }



        Debug2.Log("MoveManager:=> line 416");

        if (type == InsectType.Grasshopper && !isOpenningMove)
        {
            var candidatePositions = GetGrasshopperCandidateDestinations(initialPosition);

            return ContainsItem(candidatePositions, destinationPosition);
        }



        Debug2.Log("MoveManager:=> line 427");

        if (type == InsectType.Queen && !isOpenningMove)
        {
            var candidatePositions = GetQueenCandidateDestinations(initialPosition);

            return ContainsItem(candidatePositions, destinationPosition);
        }



        Debug2.Log("MoveManager:=> line 438 " + type + "  " + InsectType.Beetle);

        if (type == InsectType.Beetle && !isOpenningMove)
        {
            var candidatePositions = GetBeetleDestinations(initialPosition);

            return ContainsItem(candidatePositions, destinationPosition);

        }


        Debug2.Log("MoveManager:=> line 450");

        return false;
    }

    public bool isValidForOpenningMove(InsectType type, int userId, Vector3Int tilemapPosition)
    {

        if(type != InsectType.Beetle && !tilemapStorage.isEmptyTile(tilemapPosition))
        {
            return false;
        }

        var neighboars = tilemapStorage.GetSurronudingTilePieces(tilemapPosition, 1);

        if (neighboars.Count == 0)
        {
            return false;
        }

        foreach (TileInfo t in neighboars)
        {
            if (t.userId != userId)
            {
                return false;
            }
        }

        return true;
    }

    public (bool, int) isGameOver(Vector3Int queen1Position, Vector3Int queen2Position)
    {


        var neighbors = tilemapStorage.GetSurronudingTilePieces(queen1Position, 1);

        if (neighbors.Count == 6)
        {
            return (true, 1);
        }



        neighbors = tilemapStorage.GetSurronudingTilePieces(queen2Position, 1);

        if (neighbors.Count == 6)
        {
            return (true, 0);
        }

        return (false, -1);
    }

    public List<Vector3Int> GetGrasshopperCandidateDestinations(Vector3Int grasshoperPosition)
    {

        var list = new List<Vector3Int>();

        for (int i = 0; i < 6; i++)
        {
            var pos = TilemapUtility.NeighborTilePosition(grasshoperPosition, i);

            if (tilemapStorage.isEmptyTile(pos))
            {
                continue;
            }

            while (!tilemapStorage.isEmptyTile(pos))
            {
                pos = TilemapUtility.NeighborTilePosition(pos, i);
            }


            list.Add(pos);

        }

        return list;
    }

    public List<Vector3Int> GetQueenCandidateDestinations(Vector3Int queenPosition)
    {
        List<Vector3Int> surroundingEmptyTiles = tilemapStorage.GetSurroundingEmptyTiles(queenPosition, 1);

        var res = new List<Vector3Int>();

        foreach (var v in surroundingEmptyTiles)
        {
            if (!hasNoneEmptyNeighboar(v, queenPosition))
            {
                continue;
            }

            res.Add(v);
        }
        //29902669 // khonome jeloiary

        return res;
    }

    private bool hasNoneEmptyNeighboar(Vector3Int position, Vector3Int except)
    {

        var list = tilemapStorage.GetSurroundingNoneEmptyTiles(position, 1);

        if (list.Count == 0)
        {
            return false;
        }

        if (list.Count > 1)
        {
            return true;
        }

        if (list[0].GetUniqueKey().Equals(except.GetUniqueKey()))
        {
            return false;
        }

        return true;

    }

    public List<Vector3Int> GetBeetleDestinations(Vector3Int beetlePosition)
    {
        Vector3Int[] neighboars = TilemapUtility.FindRing(beetlePosition, 1);
        var res = new List<Vector3Int>();
        foreach (var n in neighboars)
        {
            if (hasNoneEmptyNeighboar(n, beetlePosition))
            {
                res.Add(n);
            }
        }

        return res;
    }

}
