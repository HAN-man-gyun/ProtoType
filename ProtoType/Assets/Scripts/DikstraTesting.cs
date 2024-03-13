using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DikstraTesting : MonoBehaviour
{
    Dikstra dikstra;
    GameObject testingobject;

    public GameObject prefabToInstantiate;
    private GameObject pointer;

    int movingCount;
    int x, y;
    int cellSize;
    int lastx, lasty;
    // Start is called before the first frame update
    void Start()
    {
        dikstra = FindFirstObjectByType<Dikstra>();
        testingobject = this.gameObject;
        movingCount = 10;
        cellSize =4;
        dikstra.SetPlayer(testingobject.transform,movingCount);
        //마우스포인터
        pointer = Instantiate(prefabToInstantiate);
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            dikstra.MovePlayer();
            
            //dikstra.FindPath(gameObject.transform.position, GetMousePos());
        }
        
        //
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit))
        {
            Vector3 hitPoint = new Vector3(hit.point.x, 0, hit.point.z);
            pointer.transform.position = hitPoint;
            GetNodeFromWorldPoint(hitPoint);
        }
        Debug.Log("무빙카운트" + dikstra.movingCount);
    }

    void GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        
        x = Mathf.FloorToInt(worldPosition.x  / cellSize);
        y = Mathf.FloorToInt(worldPosition.z  / cellSize);

        if(!(lastx == x && lasty ==y))
        {
            if (dikstra.playerMoving == false)
            {
                
                dikstra.FindPath(gameObject.transform.position, GetMousePos());
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
