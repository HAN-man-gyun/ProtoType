using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mouse : MonoBehaviour
{
    
    Dikstra dikstra;

    int movingCount;
    int x, y;
    int cellSize;
    int lastx, lasty;

    public Vector3 hitPoint;

    bool outOfGrid;
    // Start is called before the first frame update
    void Start()
    {
        
        dikstra = FindFirstObjectByType<Dikstra>();
        movingCount = 10;
        cellSize = dikstra.cellSize;
        outOfGrid = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            hitPoint = new Vector3(hit.point.x, 0, hit.point.z);
            CheckMouseInGrid(hitPoint);
        }

        if (dikstra.isMoveReady)
        {
            if (BattleSystem.BattleSystem1.dikstra.movePlayer == null || 
                BattleSystem.BattleSystem1.dikstra.movePlayer.name == BattleSystem.BattleSystem1.dikstra.player.name)
            {
                ReCalculatePath(hitPoint);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            string hitTag = hit.collider.gameObject.tag.ToString();
            if (hitTag =="Player"&& BattleSystem.BattleSystem1.dikstra.playerMoving == false)
            {
                Debug.Log("플레이어 지정완료");
                BattleSystem.BattleSystem1.dikstra.MakeFalsePlayerGrid();
                BattleSystem.BattleSystem1.dikstra.MakeGridTextureFalse();
                BattleSystem.BattleSystem1.dikstra.isMoveReady = false;
                BattleSystem.BattleSystem1.dikstra.SetPlayer(hit.collider.gameObject.transform, movingCount);
            }
          
            if (hitTag == "Floor" && outOfGrid == false&& BattleSystem.BattleSystem1.dikstra.playerMoving == false)
            {
                BattleSystem.BattleSystem1.fsm.ChangeState(BattleSystem.BattleSystem1.fsm._myTurnState);
                BattleSystem.BattleSystem1.dikstra.MovePlayer();
            }
        }
    }

    public void CanMove()
    {
        Debug.Log(BattleSystem.BattleSystem1.dikstra.player.name);
        if (BattleSystem.BattleSystem1.dikstra.player !=null)
        {
            BattleSystem.BattleSystem1.dikstra.isMoveReady = true;
        }
    } 

    public void EndTurn()
    {
        BattleSystem.BattleSystem1.dikstra.player = null;
        BattleSystem.BattleSystem1.dikstra.movingCount = 0;
        BattleSystem.BattleSystem1.dikstra.isMoveReady = false; // 버튼을 클릭했는지 여부확인하는변수.
        BattleSystem.BattleSystem1.dikstra.movePlayer = null;
    }
    public void CheckMouseInGrid(Vector3 mousePos)
    {
        int x = Mathf.FloorToInt((mousePos).x / cellSize);
        int y = Mathf.FloorToInt((mousePos).z / cellSize);

        if(x<= BattleSystem.BattleSystem1.dikstra.grid.GetLength(0)&& x >= 0 &&
            y<= BattleSystem.BattleSystem1.dikstra.grid.GetLength(1) && y >= 0)
        {
            outOfGrid = false;
        }
        else
        {
            outOfGrid = true;
        }
    }
    void ReCalculatePath(Vector3 worldPosition)
    {
        cellSize = dikstra.cellSize;
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.z / cellSize);
        

        if (!(lastx == x && lasty == y))
        {
            if (dikstra.playerMoving == false)
            {
                dikstra.FindPath(dikstra.player.position, GetMousePos());
            }
        }
        lastx = x;
        lasty = y;
    }

    public Vector3 GetMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
        return hit.point;
    }

}
