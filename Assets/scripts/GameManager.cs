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
        moveManager = new MoveManager(tilemapStorage, gameState);
        UpdateHintText();
    }

    private void OnMouseDown()
    {

        var pos = Input.mousePosition;

        var tile_pos = tilemap.WorldToCell(pos);

    }

    
    // acurate_position, is_valid_position

    public (Vector3, bool) GetAcurratePositionOnTilemap(InsectType type, int userId, Vector3 initialPosition, Vector3 currentPosition, bool isAttachedToBoard, int tokenId)
    {

        if(gameState.currentUserTurnId != userId)
        {
            return (initialPosition, false);
        }

        Vector3Int tilemapPosition = tilemap.WorldToCell(currentPosition);

        Vector3Int initalPositionOnTilemap = tilemap.WorldToCell(initialPosition);

        if(moveManager.isValidPosition(type, userId, tilemapPosition, !isAttachedToBoard, tokenId, initalPositionOnTilemap))
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

            gameState.totalTrurnsSinceStart += 1;
            UpdateHintText();
            
            if(!isAttachedToBoard)
            {
                gameState.totalPiecesInGame += 1;
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