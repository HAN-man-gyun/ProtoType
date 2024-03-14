using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DikstraTesting : MonoBehaviour
{
    Dikstra dikstra;

    public GameObject prefabToInstantiate;
    public GameObject pointer;
    int movingCount;
    int x, y;
    int cellSize;
    int lastx, lasty;

    Vector3 hitPoint;
    // Start is called before the first frame update
    void Start()
    {
        dikstra = FindFirstObjectByType<Dikstra>();
        movingCount = 10;
        cellSize = 4;
        pointer = Instantiate(prefabToInstantiate);
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            hitPoint = new Vector3(hit.point.x, 0, hit.point.z);
        }

        if (dikstra.isMoveReady)
        {
            GetNodeFromWorldPoint(hitPoint);
        }

        if (Input.GetMouseButtonDown(0))
        {
            string hitTag = hit.collider.gameObject.tag.ToString();
            if (hitTag =="Player")
            {
                BattleSystem.BattleSystem1.dikstra.MakeFalsePlayerGrid();
                BattleSystem.BattleSystem1.dikstra.MakeGridTextureFalse();
                BattleSystem.BattleSystem1.dikstra.isMoveReady = false;
                BattleSystem.BattleSystem1.dikstra.SetPlayer(hit.collider.gameObject.transform, movingCount);
            }

            if (hitTag == "Floor")
            {
                hitPoint
                BattleSystem.BattleSystem1.dikstra.MovePlayer();
            }
        }
    }

    public void CanMove()
    {
        if(BattleSystem.BattleSystem1.dikstra.player !=null)
        {
            BattleSystem.BattleSystem1.dikstra.isMoveReady = true;
        }
        
    }

    void GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        
        if( hitPoint.x > BattleSystem.BattleSystem1.dikstra.grid[0,0].gridX && hitPoint.y > BattleSystem.BattleSystem1.dikstra.grid[0,0].gridY)
    }
    void GetNodeFromWorldPoint2(Vector3 worldPosition)
    {

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
