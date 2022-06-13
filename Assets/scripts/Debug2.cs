using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug2 : MonoBehaviour
{
    public static bool show = false;
    public static void Log(string s)
    {
        if(show)
        {
            Debug.Log(s);
        }
    }
}
