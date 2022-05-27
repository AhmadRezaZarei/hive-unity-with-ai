using System;
using UnityEngine;
public static class Extensions
{
    public static string GetUniqueKey(this Vector3Int vector)
    {
        return vector.x + "," + vector.y;
    }
}