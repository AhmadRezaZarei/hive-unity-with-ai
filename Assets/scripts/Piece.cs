using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    public bool isAttachedToBoard = false;

    private GameManager gamemanager;

    [SerializeField]
    public InsectType type;

    [SerializeField]
    public int tokenId;

    [SerializeField]
    public int userId;

    private Vector3 initialPosition;

    Vector3 offset;
    float initialZ = 0;
    void Start()
    {
        gamemanager = GameObject.FindObjectOfType<GameManager>();
        initialZ = transform.position.z;   
    }

    void Update()
    {

    }


    void OnMouseDown()
    {

        initialPosition = transform.position;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        offset = mousePosition - transform.position;
    }

    void OnMouseDrag()
    {
        Vector3 newPosition =Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset ;
        newPosition.z = initialZ;
        transform.position = newPosition;
    }

    public void updateState(Vector3 position, bool isValidMove)
    {
        transform.position = position;

        if (isValidMove && !isAttachedToBoard)
        {
            isAttachedToBoard = isValidMove;
        }
    }

    

    private void OnMouseUp()
    {
        
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset; 
        
        var (position, isValidMove) = gamemanager.GetAcurratePositionOnTilemap(type, userId, initialPosition, currentPosition, isAttachedToBoard, tokenId, false);

        updateState(position, isValidMove);
    }


}
