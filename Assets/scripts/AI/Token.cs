using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Token: IComparable<Token>
{
    public int x;
    public int y;
    public int hIndex;
    public int tokenId;
    public InsectType type;
    public bool isInTheBoard;
    public int userId;
    
    public Vector3Int GetPositionInTilemap()
    {
        return new Vector3Int(x, y, 0);
    }
    public Token(int x, int y, int hIndex, int tokenId, InsectType type, bool isInTheBoard, int userId)
    {
        this.x = x;
        this.y = y;
        this.tokenId = tokenId;
        this.hIndex = hIndex;
        this.type = type;
        this.isInTheBoard = isInTheBoard;
        this.userId = userId;
    }

    public int CompareTo(Token other)
    {
        return hIndex.CompareTo(other.hIndex);
    }

    public TileInfo getCorespondingTileInfo()
    {
       return new TileInfo(type, userId, tokenId);
    }

}