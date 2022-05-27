using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{

    private MoveManager moveManager;


    [SerializeField]
    private Text hintText;

    [SerializeField]
    private Tilemap tilemap;

    private GameState gameState;

    private TilemapStorage tilemapStorage;

    [SerializeField]
    private GameObject hintCircle;

    private int totalTrurnsSinceStart = 0;

    [SerializeField]
    private int totalPiecesInGame = 0;
    public bool isMyTurn(int userId)
    {
        return userId == gameState.currentUserTurnId;
    }

    [SerializeField]
    private GameObject[] pieces;

    private void temp_hint(List<Vector3Int> list)
    {

        if(true)
        {
            return;
        }

        foreach(var l in list)
        {
            var pos = tilemap.CellToWorld(l);

            Object.Instantiate(hintCircle, pos, Quaternion.identity);

        }
    }

    private void UpdateHintText()
    {
        if(gameState.currentUserTurnId == 0)
        {
            hintText.text = "Black player turn";
            return;
        }

        hintText.text = "White player turn";
    }

    [SerializeField]
    private Vector3 guiOFfset = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        gameState = new GameState();
        tilemapStorage = new TilemapStorage(tilemap.cellBounds);
        moveManager = new MoveManager(tilemapStorage);
        UpdateHintText();
    }

    public bool isValidPosition(InsectType type, int userId, Vector3Int tilemapPosition, bool isOpenningMove, int tokenId, Vector3 initialPosition)
    {


        Debug.Log("isValidPosition 79");

        if(!isOpenningMove)
        {
            if(!tilemapStorage.canMove(tilemap.WorldToCell(initialPosition), tokenId))
            {
                return false;
            }
        }


        Debug.Log("isValidPosition 90");


        int leftTurnsToEnterQueen = userId == 0 ? gameState.leftTurnsToEnterUsers1Queen : gameState.leftTurnsToEnterUsers2Queen;
        bool isQueenEntered = userId == 0 ? gameState.isUser1QueenEntered : gameState.isUser2QueenEntered;


        if(!isQueenEntered && !isOpenningMove)
        {
            return false;
        }


        Debug.Log("isValidPosition 103");

        if (leftTurnsToEnterQueen == 1 && !isQueenEntered && type != InsectType.Queen)
        {
            return false;
        }


        Debug.Log("isValidPosition 111");

        if (type == InsectType.Queen && isOpenningMove)
        {

            if(userId == 0)
            {
                gameState.isUser1QueenEntered = true;
            } else
            {
                gameState.isUser2QueenEntered = true;
            }

        }


        Debug.Log("isValidPosition 126");

        if (totalTrurnsSinceStart == 0)
        {
            return true;
        }


        Debug.Log("isValidPosition 135");

        if (totalTrurnsSinceStart == 1)

        {
            var neighoars = tilemapStorage.GetSurronudingTilePieces(tilemapPosition, 1, tilemap);

            if(neighoars.Count == 0)
            {
                return false;
            }


            Debug.Log("isValidPosition 148");

            if (neighoars[0].userId == userId)
            {
                return false;
            }


            Debug.Log("isValidPosition 156");

            return true;
        }

        if(isOpenningMove)
        {
            return isValidForOpenningMove(type, userId, tilemapPosition);
        }



        Debug.Log("isValidPosition 168 isMovable: " + isOpenningMove);

        if (!isMovable(initialPosition, type == InsectType.Beetle) && !isOpenningMove)
        {
            return false;
        }


        Debug.Log("isValidPosition 176");

        if (type == InsectType.Ant && !isOpenningMove)
        {
            return isValidMoveForAnt(userId, tilemapPosition, tokenId);
        }


        Debug.Log("isValidPosition 184");

        if (type == InsectType.Spider && !isOpenningMove)
        {

            var candidatePositions = GetSpiderCandidatePositions(tilemap.WorldToCell(initialPosition));

            return ContainsItem(candidatePositions, tilemapPosition);
            
        }


        Debug.Log("isValidPosition 196");

        if (type == InsectType.Grasshopper && !isOpenningMove)
        {
            var candidatePositions = moveManager.GetGrasshopperCandidateDestinations(tilemap.WorldToCell(initialPosition));

            return ContainsItem(candidatePositions, tilemapPosition);
        }


        Debug.Log("isValidPosition 206");

        if (type == InsectType.Queen && !isOpenningMove)
        {
            var candidatePositions = moveManager.GetQueenCandidateDestinations(tilemap.WorldToCell(initialPosition));

            return ContainsItem(candidatePositions, tilemapPosition);
        }


        Debug.Log("isValidPosition 216");

        if (type == InsectType.Beetle && !isOpenningMove)
        {
            var candidatePositions = moveManager.GetBeetleDestinations(tilemap.WorldToCell(initialPosition));

            return ContainsItem(candidatePositions, tilemapPosition);

        }

        Debug.Log("isValidPosition 228");


        return false;
    }

    /*
    private (bool, int) isGameOver()
    {

        GameObject queen = pieces[10];

        var neighbors = tilemapStorage.GetSurronudingTilePieces(tilemap.WorldToCell(queen.transform.position), 1, null);

        if(neighbors.Count == 6)
        {
            return (true, 1);
        }


         queen = pieces[21];

         neighbors = tilemapStorage.GetSurronudingTilePieces(tilemap.WorldToCell(queen.transform.position), 1, null);

        if (neighbors.Count == 6)
        {
            return (true, 0);
        }

        return (false, -1);
    }

    */
    private bool ContainsItem(List<Vector3Int> list, Vector3Int toSearch)
    {

        foreach(var v in list)
        {
            if(v.GetUniqueKey() == toSearch.GetUniqueKey())
            {
                return true;
            }
        }

        return false;
    }

    private List<Vector3Int> GetSpiderCandidatePositions(Vector3Int spiderPosition)
    {

        var list = DepthLimitedDistinitions(spiderPosition, spiderPosition, new HashSet<string>(), new HashSet<string>(), 0);

        temp_hint(list);


        return list;
    }

    private bool hasNoneEmptyNeighboar(Vector3Int position, Vector3Int except)
    {

        var list = tilemapStorage.GetSurroundingNoneEmptyTiles(position, 1, null);

        if(list.Count == 0)
        {
            return false;
        }

        if(list.Count > 1)
        {
            return true;
        }

        if(list[0].GetUniqueKey().Equals(except.GetUniqueKey()))
        {
            return false;
        }
        
        return true;


    }

    private List<Vector3Int> DepthLimitedDistinitions(Vector3Int position, Vector3Int except, HashSet<string> visited,  HashSet<string> reachedPositionsSet, int currentDepth)
    {

        List<Vector3Int> res = new List<Vector3Int>();

        var emptyNeighbors = tilemapStorage.GetSurroundingEmptyTiles(position, 1);

        foreach(var pos in emptyNeighbors)
        {
            if (pos.GetUniqueKey() != except.GetUniqueKey())
            {

                if (!reachedPositionsSet.Contains(pos.GetUniqueKey()))
                {

                    if(visited.Contains(pos.GetUniqueKey()))
                    {
                        continue;
                    }

                    if(!hasNoneEmptyNeighboar(pos, except))
                    {
                        continue;
                    }

                    visited.Add(pos.GetUniqueKey());

                    if(currentDepth == 2)
                    {
                        reachedPositionsSet.Add(pos.GetUniqueKey());
                        res.Add(pos);
                    } else
                    {
                        res.AddRange(DepthLimitedDistinitions(pos, except, visited ,reachedPositionsSet, currentDepth + 1));
                    }

                }

            }
        }
        
        return res;
    }
    



    private bool isMovable(Vector3 initialPosition, bool isBeetle)
    {
        HashSet<int> visited = new HashSet<int>();

        List<Vector3Int> neighbors = tilemapStorage.GetSurroundingNoneEmptyTiles(tilemap.WorldToCell(initialPosition), 1, tilemap);

        if(neighbors.Count == 0)
        {
            return false;
        }

        Vector3Int exceptPosition = tilemap.WorldToCell(initialPosition);

        if(isBeetle)
        {
            if(tilemapStorage.GetPieces(tilemap.WorldToCell(initialPosition)).Count > 1)
            {
                return true;
            }
        }

        movableHelperDFS(neighbors[0], exceptPosition, visited);

        Debug.Log("Line 367 count " + visited.Count);


        if(visited.Count != totalPiecesInGame - 1)
        {
            return false;
        }


        return true;
    }

    private void movableHelperDFS(Vector3Int center, Vector3Int except, HashSet<int> visited)
    {

        var pieces = tilemapStorage.GetPieces(center);

        foreach(var p in pieces)
        {
            if (!visited.Contains(p.tokenId))
            {
                visited.Add(p.tokenId);
            }
        }

       

        List<Vector3Int> neighbors = tilemapStorage.GetSurroundingNoneEmptyTiles(center, 1, tilemap);

        foreach(Vector3Int v in neighbors)
        {
            if(!v.GetUniqueKey().Equals(except.GetUniqueKey()))
            {

                var nPieces = tilemapStorage.GetPieces(v);

                if(nPieces != null && nPieces.Count != 0)
                {

                    bool isVisitedAll = true;

                    foreach(var p in nPieces)
                    {
                        if(!visited.Contains(p.tokenId))
                        {
                            isVisitedAll = false;
                            visited.Add(p.tokenId);
                        }

                       

                    }

                    if (!isVisitedAll)
                    {
                        movableHelperDFS(v, except, visited);
                    }
                }

                
               
            }
           
        }

    }

    private bool isValidMoveForAnt(int userId, Vector3Int tilemapPosition, int tokenId) {


        if (!tilemapStorage.isEmptyTile(tilemapPosition))
        {
            return false;
        }

        var neighboars = tilemapStorage.GetSurronudingTilePieces(tilemapPosition, 1, tilemap);


        bool isCurrentPiesIncluded = false;
        foreach(TileInfo t in neighboars)
        {
            if(t.tokenId == tokenId)
            {
                isCurrentPiesIncluded = true;
                break;
            }
        }


        if(isCurrentPiesIncluded)
        {
            if (neighboars.Count > 5 || neighboars.Count == 0)
            {
                return false;
            }


            return true;
        }

        if(neighboars.Count > 4 || neighboars.Count == 0)
        {
            return false;
        }
       


        return true;
    }

    private void OnMouseDown()
    {

        var pos = Input.mousePosition;

        var tile_pos = tilemap.WorldToCell(pos);

        Debug.Log("tile_pos: " +  tile_pos);

    }

    private bool isValidForOpenningMove(InsectType type, int userId, Vector3Int tilemapPosition)
    {

        var neighboars = tilemapStorage.GetSurronudingTilePieces(tilemapPosition, 1, tilemap);

        if(neighboars.Count == 0)
        {
            return false;
        }

        foreach(TileInfo t in neighboars)
        {
            if(t.userId != userId)
            {
                return false;
            }
        }

        return true;
    }

    // acurate_position, is_valid_position
    public (Vector3, bool) GetAcurratePositionOnTilemap(InsectType type, int userId, Vector3 initialPosition, Vector3 currentPosition, bool isAttachedToBoard, int tokenId)
    {

        if(gameState.currentUserTurnId != userId)
        {
            return (initialPosition, false);
        }

        Vector3Int tilemapPosition = tilemap.WorldToCell(currentPosition);

        if(isValidPosition(type, userId, tilemapPosition, !isAttachedToBoard, tokenId, initialPosition))
        {
            Vector3 temp = tilemap.CellToWorld(tilemapPosition);
            temp.z = initialPosition.z;

            if(userId == 0)
            {
                gameState.currentUserTurnId = 1;
                gameState.leftTurnsToEnterUsers1Queen -= 1;

            } else
            {
                gameState.currentUserTurnId = 0;
                gameState.leftTurnsToEnterUsers2Queen -= 1;
            }

            tilemapStorage.Remove(tilemap.WorldToCell(initialPosition), tokenId);
            tilemapStorage.Insert(tilemapPosition, new TileInfo(type, userId, tokenId));

            if(type == InsectType.Beetle)
            {
                var picesOnBoard = tilemapStorage.GetPieces(tilemapPosition);

                if(picesOnBoard != null && picesOnBoard.Count > 1)
                {
                    var gameObject = pieces[picesOnBoard[picesOnBoard.Count - 2].tokenId];

                    pieces[tokenId].GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1; 

                }

            }

            totalTrurnsSinceStart += 1;
            UpdateHintText();
            
            if(!isAttachedToBoard)
            {
                totalPiecesInGame += 1;
            }


            Vector3Int blackQueenPosition = tilemap.WorldToCell(pieces[10].transform.position);
            Vector3Int whiteQueenPosition = tilemap.WorldToCell(pieces[21].transform.position);

            var (isgameOVer, winner) = moveManager.isGameOver(blackQueenPosition, whiteQueenPosition);


            if (isgameOVer)
            {
                hintText.text = winner == 0 ? "Black player won" : "White player won";
            }

            return (temp, true);
        }

        


        return (initialPosition, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnGUI()
    {

        var layoutBounds = tilemap.cellBounds;

        if(true)
        {
            return;
        }

        Debug.Log(layoutBounds.xMax + " - " + layoutBounds.xMin);

        for (int i = -10; i <= 10; i++)
        {
            for (int j = -10; j <= 10; j++)
            {

                var s = new GUIStyle();
                s.normal.textColor = Color.black;
                GUI.Label(new Rect(Camera.main.WorldToScreenPoint(tilemap.CellToWorld(new Vector3Int(i, j, 0))) + guiOFfset, new Vector2(15, 15)), "" + i + "," + j, s); ;
            }
        }

    }
}
