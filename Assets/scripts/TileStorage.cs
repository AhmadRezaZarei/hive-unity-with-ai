using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapStorage
{
    
    public delegate void Iterator(Vector3Int position, TileInfo ti, int hIndex);

    private Dictionary<String, List<TileInfo>> tiles;

    private BoundsInt cellBounds;
    public TilemapStorage(BoundsInt cellBounds)
    {
        this.cellBounds = cellBounds;
        tiles = new Dictionary<String, List<TileInfo>>();
    }

    public void ForEeachTiles(Iterator iterator)
    {
        foreach(KeyValuePair<String, List<TileInfo>> entry in tiles) {

            Vector3Int tilePosition = entry.Key.toVector3Int();

            for(int i = 0; i < entry.Value.Count; i++)
            {
                iterator(tilePosition, entry.Value[i], i);
            }

        }
    }


    public TilemapStorage()
    {
        tiles = new Dictionary<String, List<TileInfo>>();
    }

    public void Insert(Vector3Int pos, TileInfo data)
    {

        if(tiles.ContainsKey(pos.GetUniqueKey()))
        {
            var temp = tiles[pos.GetUniqueKey()];

            temp.Add(data);

            tiles[pos.GetUniqueKey()] = temp;

            return;
        }

        var lst = new List<TileInfo>();
        lst.Add(data);
        tiles.Add(pos.GetUniqueKey(), lst);

    }

    public List<TileInfo> GetPieces(Vector3Int pos)
    {
        if(!tiles.ContainsKey(pos.GetUniqueKey()))
        {
            return null;
        }

        return tiles[pos.GetUniqueKey()];
    } 

    public bool Remove(Vector3Int pos, int tokenId)
    {

        if(!tiles.ContainsKey(pos.GetUniqueKey()))
        {
            return true;
        }

        var list = tiles[pos.GetUniqueKey()];

        if(list.Count == 1)
        {
            tiles.Remove(pos.GetUniqueKey());
            return true;
        }

        if(list[list.Count - 1].tokenId == tokenId)
        {
            list.RemoveAt(list.Count - 1);
            tiles[pos.GetUniqueKey()] = list;
            return true;
        }

        return false;
    }

    public List<Vector3Int> GetSurroundingEmptyTiles(Vector3Int center, int radius)
    {

        Vector3Int[] tiles = TilemapUtility.FindRing(center, radius);

        List<Vector3Int> res = new List<Vector3Int>();

        foreach (Vector3Int t in tiles)
        {
            var (isEmptyPlace, isOutOfRange) = IsEmpty(t);

            if (isEmptyPlace)
            {
                res.Add(t);
            }

        }

        return res;
    }

    public List<Vector3Int> GetSurroundingNoneEmptyTiles(Vector3Int center, int radius)
    {

        Vector3Int[] tiles = TilemapUtility.FindRing(center, radius);

        List<Vector3Int> res = new List<Vector3Int>();

        foreach (Vector3Int t in tiles)
        {
            var (isEmptyPlace, isOutOfRange) = IsEmpty(t);

            if (!isEmptyPlace)
            {
                res.Add(t);
            }

        }

        return res;
    }

    public bool canMove(Vector3Int pos, int tokenId)
    {

        if (tiles.ContainsKey(pos.GetUniqueKey()))
        {

            var list = tiles[pos.GetUniqueKey()];

            if (list.Count > 1 && list[list.Count - 1].tokenId != tokenId)
            {
                return false;
            }

        }

        return true;
    }

    public List<TileInfo> GetSurronudingTilePieces(Vector3Int center, int radius)
    {
        var list = GetSurroundingNoneEmptyTiles(center, radius);

        if(list.Count == 0)
        {
            return new List<TileInfo>();
        }
        

        var res = new List<TileInfo>();

        foreach(Vector3Int v in list)
        {
            if (tiles.ContainsKey(v.GetUniqueKey()))
            {
                res.Add(tiles[v.GetUniqueKey()][0]);
            }
        }

        return res;

    }

    //// nearest , isFind
    public (Vector3Int, Boolean) FindNearestEmptyTile(Vector3Int position, Tilemap tm)
    {
        int ringRaduis = 1;

        while (true)
        {

            var rTiles = TilemapUtility.FindRing(position, ringRaduis);
            int outOfRangeCount = 0;

            foreach (Vector3Int t in rTiles)
            {
                var (isEmptyPlace, isOutOfRange) = IsEmpty(t);

                if (isOutOfRange)
                {
                    outOfRangeCount++;
                }

                if (isEmptyPlace)
                {
                    return (t, true);
                }
            }

            if (outOfRangeCount == rTiles.Length)
            {
                return (position, false);
            }

            ringRaduis++;
        }
    }


    //public List<Vector3Int> GetNeighbors(Vector3Int position, Tilemap tm)
    //{
    //    var possibleNeighbors = TilemapUtility.FindRing(position, 1);
    //    var res = new List<Vector3Int>();

    //    foreach (Vector3Int n in possibleNeighbors)
    //    {
    //        var (isEmptyPlace, isOutOfRange) = IsEmpty(n, tm);

    //        if (isEmptyPlace)
    //        {
    //            res.Add(n);
    //        }

    //    }

    //    return res;
    //}

    // isEmptyPlace, isOutOfRange
    private (Boolean, Boolean) IsEmpty(Vector3Int position)
    {
        //if (cellBounds.xMin > position.x || cellBounds.yMin > position.y || cellBounds.zMin > position.z || cellBounds.xMax < position.x || cellBounds.yMax < position.y || cellBounds.zMax < position.z)
        //{
        //    return (false, true);
        //}

        return (isEmptyTile(position), false);
    }

    public bool isEmptyTile(Vector3Int tilemapPosition)
    {
        return !tiles.ContainsKey(tilemapPosition.GetUniqueKey());
    }

    private void InsertBeetle(Vector3Int pos, TileInfo data)
    {

    }

    

}
