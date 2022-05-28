using System;
using UnityEngine;
public static class Extensions
{
    public static string GetUniqueKey(this Vector3Int vector)
    {
        return vector.x + "," + vector.y;
    }

    public static Vector3Int toVector3Int(this string text)
    {
        Vector3Int v = Vector3Int.zero;
        string[] splited = text.Split(new string[] { "," }, 2, StringSplitOptions.RemoveEmptyEntries);
        v.x = int.Parse(splited[0]);
        v.y = int.Parse(splited[1]);
        v.z = 0;
        return v;
    }

    public static bool isXYEqual(this Vector3Int o1, Vector3Int o2)
    {
        return o1.x == o2.x && o1.y == o2.y; 
    }

}