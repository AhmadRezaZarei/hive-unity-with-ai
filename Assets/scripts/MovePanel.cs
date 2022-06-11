using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePanel : MonoBehaviour
{

    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private GameObject moveDisplayer;

    private void ClearList()
    {
        int childCount = transform.childCount;

        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

    }
    public void DisplayMoves(List<Move> moves)
    {
        
        ClearList();

        if(moves == null)
        {
            return;
        }
        
        for (int i = 0; i < moves.Count; i++)
        {
            Move m = moves[i];
            GameObject cell = getCell(i, m);
            cell.transform.SetParent(this.transform);
        }
    }

    private GameObject getCell(int index,Move move)
    {
        GameObject cell = Instantiate(cellPrefab, transform.position, Quaternion.identity, this.transform);

        Button btn = cell.GetComponent<Button>();

        btn.onClick.AddListener(() => {
            moveDisplayer.GetComponent<GameManager>().Display(move);
            Debug.Log("MovePanel:=> " + move.from.GetUniqueKey() + " | " + move.to.GetUniqueKey());
        });


        Text text = cell.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        text.text = ("#" + index +" ") + move.token.type;
        return cell;
    }
}
