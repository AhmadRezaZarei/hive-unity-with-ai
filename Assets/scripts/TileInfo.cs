using System;

public struct TileInfo
{

    public TileInfo(InsectType type, int userId, int tokenId)
    {
        this.type = type;
        this.userId = userId;
        this.tokenId = tokenId;
    }
    public InsectType type;
    public int userId;
    public int tokenId;
}