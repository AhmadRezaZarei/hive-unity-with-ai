using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    private bool isAttachedToBoard = false;

    private GameManager gamemanager;

    [SerializeField]
    private InsectType type;

    [SerializeField]
    private int tokenId;

    [SerializeField]
    private int userId;

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


    private void OnMouseUp()
    {
        
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset; 
        
        var inf = gamemanager.GetAcurratePositionOnTilemap(type, userId, initialPosition, currentPosition, isAttachedToBoard, tokenId);

        transform.position = inf.Item1;

        if(inf.Item2 && !isAttachedToBoard)
        {
            isAttachedToBoard = inf.Item2;
        }

    }


}
