using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public Token token;
    public Vector3Int from;
    public Vector3Int to;
    public bool isOpenningMove;
    public Move(Token token, Vector3Int from, Vector3Int to, bool isOpenningMove)
    {
        this.token = token;
        this.from = from;
        this.to = to;
        this.isOpenningMove = isOpenningMove;
    }

    

}
