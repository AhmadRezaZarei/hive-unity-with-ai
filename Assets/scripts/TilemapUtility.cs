using UnityEngine;
using System;
using System.Collections;

public class TilemapUtility
{



    public static readonly Vector3Int[,] directions =
    {
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(0, 1, 0),
        },{
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0),
        }
    };


    public static int Distance(Vector3Int a, Vector3Int b)
    {
        var ca = ConvertOddRCordinateToOffsetCordinate(a);
        var cb = ConvertOddRCordinateToOffsetCordinate(b);

        return (Math.Abs(ca.x - cb.x) + Math.Abs(ca.y - cb.y) + Math.Abs(ca.z - cb.z)) / 2;
    }

    private static Vector3Int ConvertOddRCordinateToOffsetCordinate(Vector3Int oddr)
    {
        var x = oddr.x - (oddr.y - (oddr.y & 1)) / 2;
        var z = oddr.y;
        var y = -x - z;
        return new Vector3Int(x, y, z);
    }

    public static Vector3Int NeighborTilePosition(Vector3Int center, int directionIndex)
    {
        var dir = directions[center.y & 1, directionIndex];
        return center + dir;
    }

    public static Vector3Int[] FindRing(Vector3Int center, int radius)
    {
        Vector3Int startTile = NeighborTilePosition(center, 4);

        for (int i = 1; i < radius; i++)
        {
            startTile = NeighborTilePosition(startTile, 4);
        }

        Vector3Int[] result = new Vector3Int[6 * radius];

        int startIndex = 0;

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                result[startIndex] = startTile;
                startTile = NeighborTilePosition(startTile, i);
                startIndex++;
            }
        }

        return result;

    }


}
