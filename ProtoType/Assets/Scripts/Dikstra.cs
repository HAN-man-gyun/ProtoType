using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Dikstra : MonoBehaviour
{
    public Transform player; // 플레이어의 위치
    public LayerMask unwalkableMask; // 이동할 수 없는 영역의 레이어 마스크
    public int cellSize = 0;
    public Vector3 originPosition;
    private GameObject[,] gridTextures;
    private GameObject[,] playerMoveTextures;
    public Node[,] grid; // 그리드
    List<Node> path; // 최단 경로
    private GameObject gridRootTextures;
    private GameObject MoveTexture;
    public int movingCount;


    public bool isMoveReady;
    

    public bool playerMoving = false;
    private void Start()
    {
        InitDikstra(15, 15, 4, Vector3.zero, "Obstacles");
        SetGrid();
    }

    public void InitDikstra(int x,int z,int cellSize, Vector3 originPosition, string layerName)
    {
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        unwalkableMask |= 1 << LayerMask.NameToLayer(layerName);
        grid = new Node[x,z];
        gridTextures = new GameObject[x, z];
        playerMoveTextures = new GameObject[x,z];
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        if (Mathf.FloorToInt((targetPos - originPosition).x / cellSize)>= grid.GetLength(0) ||
            Mathf.FloorToInt((targetPos - originPosition).z / cellSize)>= grid.GetLength(1))
        {
            return;
        }
        if (Mathf.FloorToInt((targetPos - originPosition).x / cellSize) < 0 ||
            Mathf.FloorToInt((targetPos - originPosition).z / cellSize) < 0)
        {
            return;
        }



        // 시작 노드와 목표 노드를 그리드 상의 노드로 변환합니다.
        Node startNode = GetNodeFromWorldPoint(startPos);
        Node targetNode = GetNodeFromWorldPoint(targetPos);
        // openSet은 아직 방문하지 않은 노드들의 집합이며, closedSet은 이미 방문한 노드들의 집합입니다.
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        // 시작 노드를 openSet에 추가합니다.
        openSet.Add(startNode);

        // openSet이 빌 때까지 반복합니다.
        while (openSet.Count > 0)
        {
            // openSet 중에서 가장 낮은 비용을 가진 노드를 선택합니다.
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].gCost < currentNode.gCost)
                {
                    currentNode = openSet[i];
                }
            }

            // 현재 노드를 openSet에서 제거하고 closedSet에 추가합니다.
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // 현재 노드가 목표 노드인 경우, 최단 경로를 추적하고 함수를 종료합니다.
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            // 현재 노드의 이웃 노드들을 검사합니다.
            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                // 이웃 노드가 이동 불가능하거나 이미 closedSet에 있는 경우 스킵합니다.
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                // 새로운 경로로 이웃 노드까지의 비용을 계산합니다.
                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                // 새로운 경로가 이웃 노드까지의 기존 비용보다 낮거나, 이웃 노드가 아직 openSet에 없는 경우에 대해 처리합니다.
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    // 이웃 노드의 비용과 부모 노드를 업데이트합니다.
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.parent = currentNode;

                    // 이웃 노드가 아직 openSet에 없는 경우, openSet에 추가합니다.
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
    }
    void RetracePath(Node startNode, Node endNode)
    {
        //먼저 전에있던 그리드 텍스쳐들을 False함.
        MakeGridTextureFalse();
        if (movingCount <= 0)
        {
            return;
        }
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
            //gridTextures[currentNode.gridX,currentNode.gridY].SetActive(true);
        }

        
        path.Reverse();
        for(int i=0; i< Mathf.Min(movingCount, path.Count); i++)
        {
            gridTextures[path[i].gridX, path[i].gridY].SetActive(true);
        }
        if(movingCount < path.Count)
        {

        }
        else
        {
            gridTextures[endNode.gridX, endNode.gridY].SetActive(true);
        }
        
        this.path = path;
        
        //MovePlayer();
    }

    List<Node> GetNeighbors(Node node)
    {
        /*
        List<Node> neighbors = new List<Node>();

        // 상하좌우 방향의 이웃 노드 생성
        Vector2Int[] directions = {
        new Vector2Int(0, 1),  // 위쪽
        new Vector2Int(0, -1), // 아래쪽
        new Vector2Int(-1, 0), // 왼쪽
        new Vector2Int(1, 0)   // 오른쪽
        };

        foreach (Vector2Int dir in directions)
        {
            int checkX = node.gridX + dir.x;
            int checkY = node.gridY + dir.y;

            if (checkX >= 0 && checkX < grid.GetLength(0) && checkY >= 0 && checkY < grid.GetLength(1))
            {
                neighbors.Add(grid[checkX, checkY]);
            }
        }
        */
        //대각선
        
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < grid.GetLength(0) && checkY >= 0 && checkY < grid.GetLength(1))
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        
        return neighbors;
    }

    Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        int y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
        return grid[y, x];
    }

    public Vector3 GetWorldPosition (int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }
    // 두 노드 사이의 거리 계산
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return dstX + dstY;
    }

    // 레이캐스트를 쏴서 어느 노드가 Walkable이 false인지확인하는 함수. 
    public void CheckNodeWalkable(Node node)
    {
        RaycastHit hit;
        if (Physics.Raycast(node.worldPosition + new Vector3(0,100,0), Vector3.down, out hit,Mathf.Infinity,unwalkableMask))
        {
            node.walkable = false;
        }
    }

    // 노드를 초기화(설치하는) 함수.
    public void SetGrid()
    {
        gridRootTextures = new GameObject("Textures");
        MoveTexture = new GameObject("MoveTextures");
        for(int i=0; i < grid.GetLength(0); i++)
        {
            for(int j=0; j<grid.GetLength(1); j++)
            {
                grid[i, j] = new Node(true, originPosition + new Vector3(cellSize * j,0,(cellSize * i)), i, j);
                CheckNodeWalkable(grid[i, j]);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.blue, 100f);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.blue, 100f);
                //테스트용 노드 생성 코드
                GameObject prefab = Resources.Load<GameObject>("First");
                GameObject newObject = GameObject.Instantiate(prefab);
                newObject.transform.position = GetWorldPosition(i, j);

                SetGridTexture(i, j, cellSize);
                
            }
        }
        Debug.DrawLine(GetWorldPosition(0, grid.GetLength(1)), GetWorldPosition(grid.GetLength(0), grid.GetLength(1)), Color.blue, 100f); ;
        Debug.DrawLine(GetWorldPosition(grid.GetLength(0), 0), GetWorldPosition(grid.GetLength(0), grid.GetLength(1)), Color.blue, 100f);
    }
    // 그리드 텍스쳐를 입히는 함수
    public void SetGridTexture(int i, int j, int cellSize)
    {
        Texture2D prefab = Resources.Load<Texture2D>("grid");
        GameObject gridTexture = new GameObject("girdTexture");
        SpriteRenderer renderer = gridTexture.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(prefab, new Rect(0, 0, prefab.width, prefab.height), Vector2.one * 0.5f);
        gridTextures[i, j] = gridTexture;
        gridTexture.transform.SetParent(gridRootTextures.transform);
        gridTexture.transform.position = grid[i, j].worldPosition + new Vector3(cellSize / 2, 0, cellSize / 2);
        gridTexture.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        gridTexture.transform.localScale = new Vector3(5.5f, 5.5f, 0);
        gridTexture.SetActive(false);

        //범위 텍스쳐
        Texture2D Moveprefab = Resources.Load<Texture2D>("blue");
        GameObject moveTexture = new GameObject("MoveTexture");
        SpriteRenderer moveRenderer = moveTexture.AddComponent<SpriteRenderer>();
        moveRenderer.sprite = Sprite.Create(Moveprefab, new Rect(0, 0, Moveprefab.width, Moveprefab.height), Vector2.one * 0.5f);
        playerMoveTextures[i, j] = moveTexture;
        moveTexture.transform.SetParent(MoveTexture.transform);
        moveTexture.transform.position = grid[i, j].worldPosition + new Vector3(cellSize / 2, 0, cellSize / 2);
        moveTexture.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        moveTexture.transform.localScale = new Vector3(1f, 1f, 0);
        moveTexture.SetActive(false);
    }

    public void MakeGridTextureFalse()
    {
        for (int i = 0; i< gridTextures.GetLength(0); i++)
        {
            for(int j=0; j<gridTextures.GetLength(1); j++)
            {
                gridTextures[i,j].SetActive(false);
            }
        }
    }
    //플레이어를 셋하는 함수
    public void SetPlayer(Transform player, int movingCount)
    {
        this.player = player;
        this.movingCount = movingCount;
    }
    //플레이어가 움직이는 함수
    public IEnumerator MovePlayer(List<Node> path, int cellSize)
    {
        if (path == null || path.Count == 0)
            yield break;
        if (path.Count>movingCount)
            yield break;
        //플레이어 범위표시 그리드를 다 False로 초기화시켜주는함수
        MakeFalsePlayerGrid();
        MakeGridTextureFalse();

        playerMoving = true;
        foreach (Node node in path)
        {
            Vector3 centerPos = node.worldPosition + new Vector3(cellSize / 2, 0, cellSize / 2);

            while (player.transform.position != centerPos)
            {
                // 이동
                player.transform.position = Vector3.MoveTowards(player.transform.position, centerPos, 10f * Time.deltaTime);
                yield return null;
                
            }
            movingCount--;
        }
        playerMoving = false;

        if(movingCount<= 0)
            isMoveReady = false;
        //이동을 끝낸 후 다시 범위표시 그리드를 True로 만들어주는 함수.
        PlayerGrid();
    }
    public void MovePlayer()
    {
        if (playerMoving == false)
        {
            StartCoroutine(MovePlayer(path, cellSize));
        }
    }

    public void PlayerGrid()
    {
        int x = GetNodeFromWorldPoint(player.transform.position).gridX;
        int y = GetNodeFromWorldPoint(player.transform.position).gridY;
        // 플레이어 주변으로 다이아몬드 모양으로 이동 가능한 위치를 표시합니다.
        /*
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // 다이아몬드 내부에 있는 좌표인지 확인

                if (Math.Abs(i - x) + Math.Abs(j - y) <= movingCount)
                {
                    playerMoveTextures[i, j].SetActive(true); // 범위 내에 있다면 값을 설정
                }
 

            }
        }
        */

//사각형인경우
int minX = Mathf.Max(0, x - movingCount);
        int maxX = Mathf.Min(grid.GetLength(0) - 1, x + movingCount);
        int minY = Mathf.Max(0, y - movingCount);
        int maxY = Mathf.Min(grid.GetLength(1) - 1, y + movingCount);

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                playerMoveTextures[i, j].SetActive(true); // 범위 내에 있다면 값을 설정
            }
        }
    }

    public void MakeFalsePlayerGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                playerMoveTextures[i, j].SetActive(false); // 범위 내에 있다면 값을 설정
            }
        }
    }
}


// 각 노드에 대한 정보를 저장하는 클래스
public class Node
{
    public bool walkable; // 이동 가능 여부
    public Vector3 worldPosition; // 노드의 월드 좌표
    public int gridX; // 그리드 상의 x 인덱스
    public int gridY; // 그리드 상의 y 인덱스
    public int gCost; // 시작 노드로부터의 거리
    public Node parent; // 경로 추적을 위한 부모 노드


    // 노드 생성자
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

   
}