using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager
{

    TilemapStorage storage;
    public MoveManager(TilemapStorage storage)
    {
        this.storage = storage;
    }

    public (bool, int) isGameOver(Vector3Int queen1Position, Vector3Int queen2Position)
    {


        var neighbors = storage.GetSurronudingTilePieces(queen1Position, 1, null);

        if (neighbors.Count == 6)
        {
            return (true, 1);
        }



        neighbors = storage.GetSurronudingTilePieces(queen2Position, 1, null);

        if (neighbors.Count == 6)
        {
            return (true, 0);
        }

        return (false, -1);
    }

    public List<Vector3Int> GetGrasshopperCandidateDestinations(Vector3Int grasshoperPosition)
    {

        var list = new List<Vector3Int>();

        for(int i = 0; i < 6; i++)
        {
            var pos = TilemapUtility.NeighborTilePosition(grasshoperPosition, i);

            if(storage.isEmptyTile(pos))
            {
                continue;
            }

            while(!storage.isEmptyTile(pos))
            {
                pos = TilemapUtility.NeighborTilePosition(pos, i);
            }


            list.Add(pos);

        }

        return list;
    }


    public List<Vector3Int> GetQueenCandidateDestinations(Vector3Int queenPosition)
    {   
        var temp = storage.GetSurroundingEmptyTiles(queenPosition, 1);

        var res = new List<Vector3Int>();

        foreach(var v in temp)
        {
            if(!hasNoneEmptyNeighboar(v, queenPosition))
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

        var list = storage.GetSurroundingNoneEmptyTiles(position, 1, null);

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
        foreach(var n in neighboars)
        {
            if(hasNoneEmptyNeighboar(n, beetlePosition))
            {
                res.Add(n);
            }
        }

        return res;
    }

}
