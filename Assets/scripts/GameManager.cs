using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
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

        if(!isDebug)
        {
            return;
        }

        foreach(var l in list)
        {
            var pos = tilemap.CellToWorld(l);

            var obj = Instantiate(hintCircle, pos, Quaternion.identity);
            Destroy(obj, 5);
        }
    }

    private void UpdateHintText()
    {
        Debug.Log("UpdateHintText called");
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

    [SerializeField]
    private UnityEvent temp_event;

    [SerializeField]
    private int temp_token_id = 0;
    [SerializeField]
    private InsectType temp_token_type;
    [SerializeField]
    private bool isDebug = false;

    

    public void OnMouseDown()
    {

        Debug.Log("OnMouseDown called");
        if(!isDebug)
        {
            return;
        }

        Vector3Int tokenWorldPosition = tilemap.WorldToCell(pieces[temp_token_id].transform.position);

        List<Vector3Int> moves = new List<Vector3Int>();
        switch(temp_token_type)
        {
            case InsectType.Ant:

                moves = moveManager.GetAntCandidateDestinations(tokenWorldPosition, temp_token_id);

                break;
            case InsectType.Beetle:

                moves = moveManager.GetBeetleDestinations(tokenWorldPosition);

                break;
            case InsectType.Grasshopper:
                moves = moveManager.GetGrasshopperCandidateDestinations(tokenWorldPosition);

                break;
            case InsectType.Queen:
                moves = moveManager.GetQueenCandidateDestinations(tokenWorldPosition);
                break;
            case InsectType.Spider:
                moves = moveManager.GetSpiderCandidateDestinations(tokenWorldPosition);
                break;
        }

        temp_hint(moves);





    }


    // acurate_position, is_valid_position


    private IEnumerator SmoothLerp(Vector3 intialPos, Vector3 destinationPos, GameObject token,float time, Action onEnd)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            token.transform.position = Vector3.Lerp(intialPos, destinationPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        onEnd();
    }

    public (Vector3, bool) GetAcurratePositionOnTilemap(InsectType type, int userId, Vector3 initialPosition, Vector3 currentPosition, bool isAttachedToBoard, int tokenId, bool isAgentMove)
    {

        if(gameState.currentUserTurnId != userId)
        {
            return (initialPosition, false);
        }

        Vector3Int tilemapPosition = tilemap.WorldToCell(currentPosition);

        Vector3Int initalPositionOnTilemap = tilemap.WorldToCell(initialPosition);
        Debug.Log("line 165");
        if (moveManager.isValidPosition(type, userId, tilemapPosition, !isAttachedToBoard, tokenId, initalPositionOnTilemap))
        {
            Vector3 temp = tilemap.CellToWorld(tilemapPosition);
            temp.z = initialPosition.z;

            if (userId == 0)
            {
                Debug.Log("User turn changed to white");
                gameState.currentUserTurnId = 1;
                gameState.leftTurnsToEnterUsers1Queen -= 1;

            } else
            {

                Debug.Log("User turn changed to black");
                gameState.currentUserTurnId = 0;
                gameState.leftTurnsToEnterUsers2Queen -= 1;
            }

            if (type == InsectType.Queen)
            {

                if (userId == 0)
                {
                    gameState.isUser1QueenEntered = true;
                }
                else
                {
                    gameState.isUser2QueenEntered = true;
                }

            }

            tilemapStorage.Remove(tilemap.WorldToCell(initialPosition), tokenId);
            tilemapStorage.Insert(tilemapPosition, new TileInfo(type, userId, tokenId));

            if (type == InsectType.Beetle)
            {
                var picesOnBoard = tilemapStorage.GetPieces(tilemapPosition);

                if (picesOnBoard != null && picesOnBoard.Count > 1)
                {
                    var gameObject = pieces[picesOnBoard[picesOnBoard.Count - 2].tokenId];

                    pieces[tokenId].GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;

                }

            }

            gameState.totalTrurnsSinceStart += 1;
            UpdateHintText();

            if (!isAttachedToBoard)
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

            if (!isAgentMove)
            {
                handleAgent();
            }

            return (temp, true);
        }

        return (initialPosition, false);
    }

    private void handleAgent()
    {

        Token[] tokens = new Token[22];

        for(int i = 0; i < pieces.Length; i++)
        {
            Vector3Int pos = tilemap.WorldToCell(pieces[i].transform.position);
            Piece p = pieces[i].GetComponent<Piece>();

            tokens[i] = new Token(pos.x, pos.y, 0, p.tokenId, p.type, p.isAttachedToBoard, p.userId);
        }

        Board gameBoard = new Board(tokens ,gameState, moveManager);

        Agent agent = new Agent(moveManager, tilemapStorage, gameBoard);

        Move mv = agent.getRandomMove();


        
        StartCoroutine(SmoothLerp(tilemap.CellToWorld(mv.from), tilemap.CellToWorld(mv.to), pieces[mv.token.tokenId], 4f, () => {
            GetAcurratePositionOnTilemap(mv.token.type, mv.token.userId, tilemap.CellToWorld(mv.from), tilemap.CellToWorld(mv.to), !mv.isOpenningMove, mv.token.tokenId, true);
            Debug.Log("called line 247");
        }));

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
