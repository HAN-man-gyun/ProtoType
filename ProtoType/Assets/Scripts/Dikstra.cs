using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Unity.VisualScripting;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEditor.Experimental.GraphView;

public class Dikstra : MonoBehaviour
{
    public Transform player; // �÷��̾��� ��ġ
    public LayerMask unwalkableMask; // �̵��� �� ���� ������ ���̾� ����ũ
    public int cellSize;
    public Vector3 originPosition;
    private GameObject[,] gridTextures;
    private GameObject[,] playerMoveTextures;
    public Node[,] grid; // �׸���
    List<Node> path; // �ִ� ���
    private GameObject gridRootTextures;
    private GameObject MoveTexture;
    public int movingCount;
    public int weaponRange;
    GameObject rangeTexture;
    Queue<GameObject> targetTextures;
    Queue<GameObject> target2Textures;
    private GameObject targetTexture;
    private GameObject target2Texture;


    public bool isMoveReady; // ��ư�� Ŭ���ߴ��� ����Ȯ���ϴº���.
    
    public Transform movePlayer; //�� �÷��̾ �����̸� �ٸ� �÷��̾�� �������̰� �ϱ����Ѻ���.
    public bool playerMoving = false; // �÷��̾��� �̵������� üũ�ϱ����� ����, coroutine�� ����������Ǵ°��� ����
    private void Start()
    {
        InitDikstra(15, 15, 4, Vector3.zero);
        SetGrid();
        CheckAllNodeWalkable();
        SetRangeTexture();
        SetTargetTexture();
    }

    // Dikstra �׸��� �ʱ�ȭ�ϴ� �Լ�
    public void InitDikstra(int x,int z,int cellSize, Vector3 originPosition)
    {
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        unwalkableMask |= 1 << LayerMask.NameToLayer("Obstacles");
        unwalkableMask |= 1 << LayerMask.NameToLayer("Player");
        unwalkableMask |= 1 << LayerMask.NameToLayer("Enemy");
        grid = new Node[x,z];
        gridTextures = new GameObject[x, z];
        playerMoveTextures = new GameObject[x,z];
    }

    // �ִܰŸ� ���ϴ� �Լ�.
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



        // ���� ���� ��ǥ ��带 �׸��� ���� ���� ��ȯ�մϴ�.
        Node startNode = GetNodeFromWorldPoint(startPos);
        Node targetNode = GetNodeFromWorldPoint(targetPos);
        // openSet�� ���� �湮���� ���� ������ �����̸�, closedSet�� �̹� �湮�� ������ �����Դϴ�.
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        // ���� ��带 openSet�� �߰��մϴ�.
        openSet.Add(startNode);

        // openSet�� �� ������ �ݺ��մϴ�.
        while (openSet.Count > 0)
        {
            // openSet �߿��� ���� ���� ����� ���� ��带 �����մϴ�.
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].gCost < currentNode.gCost)
                {
                    currentNode = openSet[i];
                }
            }

            // ���� ��带 openSet���� �����ϰ� closedSet�� �߰��մϴ�.
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // ���� ��尡 ��ǥ ����� ���, �ִ� ��θ� �����ϰ� �Լ��� �����մϴ�.
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            // ���� ����� �̿� ������ �˻��մϴ�.
            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                // �̿� ��尡 �̵� �Ұ����ϰų� �̹� closedSet�� �ִ� ��� ��ŵ�մϴ�.
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                // ���ο� ��η� �̿� �������� ����� ����մϴ�.
                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                // ���ο� ��ΰ� �̿� �������� ���� ��뺸�� ���ų�, �̿� ��尡 ���� openSet�� ���� ��쿡 ���� ó���մϴ�.
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    // �̿� ����� ���� �θ� ��带 ������Ʈ�մϴ�.
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.parent = currentNode;

                    // �̿� ��尡 ���� openSet�� ���� ���, openSet�� �߰��մϴ�.
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
    }
    //�ִܰŸ��� ���ؼ�. �ؽ��ĸ� ������ �Լ�.
    void RetracePath(Node startNode, Node endNode)
    {
        //���� �����ִ� �׸��� �ؽ��ĵ��� False��.
        MakeGridTextureFalse();
        //����ī��Ʈ�� ���̻� ���ٸ� ����.
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
        }
        path.Reverse();
        this.path = path;


        for (int i=0; i< Mathf.Min(movingCount, path.Count); i++)
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
        
    }

    // �ִܰŸ� ��꿡�� ���¸���Ʈ�� �������� �Լ�.
    List<Node> GetNeighbors(Node node)
    {
        /*
        List<Node> neighbors = new List<Node>();

        // �����¿� ������ �̿� ��� ����
        Vector2Int[] directions = {
        new Vector2Int(0, 1),  // ����
        new Vector2Int(0, -1), // �Ʒ���
        new Vector2Int(-1, 0), // ����
        new Vector2Int(1, 0)   // ������
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
        //�밢��
        
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

    // ���������ǿ��� ���[,]�� ���������Լ�
    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        int y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
        return grid[y, x];
    }

    // ��忡�� ���� �������� �������� �Լ�.
    public Vector3 GetWorldPosition (int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }
    // �� ��� ������ �Ÿ� ���
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return dstX + dstY;
    }

    // ����ĳ��Ʈ�� ���� ��� ��尡 Walkable�� false����Ȯ���ϴ� �Լ�. 
    public void CheckNodeWalkable(Node node)
    {
        RaycastHit hit;
        Vector3 nodePos = node.worldPosition + new Vector3 (cellSize/2, 50, cellSize/2);
        if (Physics.Raycast(nodePos, Vector3.down, out hit,Mathf.Infinity,unwalkableMask))
        {
            node.walkable = false;

            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                node.state = Node.State.Enemy;
            }
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                node.state = Node.State.Player;
            }
        }
    }

    public void CheckAllNodeWalkable()
    {
        for(int i = 0; i< grid.GetLength(0); i++)
        {
            for(int j = 0; j< grid.GetLength(1); j++)
            {
                CheckNodeWalkable(grid[i,j]);
            }
        }
    }
    
    // ��带 �ʱ�ȭ(��ġ�ϴ�) �Լ�.
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
                //�׽�Ʈ�� ��� ���� �ڵ�
                GameObject prefab = Resources.Load<GameObject>("First");
                GameObject newObject = GameObject.Instantiate(prefab);
                newObject.transform.position = GetWorldPosition(i, j);

                SetGridTexture(i, j, cellSize);
                
            }
        }
        Debug.DrawLine(GetWorldPosition(0, grid.GetLength(1)), GetWorldPosition(grid.GetLength(0), grid.GetLength(1)), Color.blue, 100f); ;
        Debug.DrawLine(GetWorldPosition(grid.GetLength(0), 0), GetWorldPosition(grid.GetLength(0), grid.GetLength(1)), Color.blue, 100f);
    }

    #region �ؽ��� �Լ�
    // �׸��� �ؽ��ĸ� ������ �Լ�
    public void SetGridTexture(int i, int j, int cellSize)
    {
        //������ �ؽ���
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

        //���� �ؽ���
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

    // ������ �ؽ��ĸ� �ʱ�ȭ �ϴ��Լ�
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
    // �÷��̾� �̵����� �ؽ��ĸ� �ʱ�ȭ �ϴ��Լ�
    public void MakeFalsePlayerGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                playerMoveTextures[i, j].SetActive(false); // ���� ���� �ִٸ� ���� ����
            }
        }
    }
    #endregion �ؽ��� �Լ�

    #region �÷��̾�����Լ�.
    //�÷��̾ ���ϴ� �Լ�
    public void SetPlayer(Transform player, int movingCount, int weaponRange)
    {
        this.player = player;
        this.weaponRange = weaponRange;
        if (movePlayer == null)
        {
            this.movingCount = movingCount;
        }
    }
    //�÷��̾ �����̴� �Լ�
    public IEnumerator MovePlayer(List<Node> path, int cellSize)
    {
        if (path == null || path.Count == 0)
            yield break;
        if (path.Count>movingCount)
            yield break;
        //�÷��̾� ����ǥ�� �׸��带 �� False�� �ʱ�ȭ�����ִ��Լ�
        MakeFalsePlayerGrid();
        MakeGridTextureFalse();
        organism unit = player.gameObject.GetComponent<organism>();
        unit.Move();
        playerMoving = true;
        foreach (Node node in path)
        {
            Vector3 centerPos = node.worldPosition + new Vector3(cellSize / 2, 0, cellSize / 2);
            
            Vector3 direction = centerPos - player.transform.position;
            player.transform.rotation = Quaternion.LookRotation(direction);
            while (player.transform.position != centerPos)
            {
                // �̵�
                player.transform.position = Vector3.MoveTowards(player.transform.position, centerPos, 10f * Time.deltaTime);
                
                
                yield return null;
                
            }
            movingCount--;
        }
        unit.MoveStop();
        playerMoving = false;
        isMoveReady = false;
        //�̵��� ���� �� �ٽ� ����ǥ�� �׸��带 True�� ������ִ� �Լ�.
        PlayerGrid();
        CheckAllNodeWalkable();
        Vector3 enemyPos = CheckInPlayerAttackRange();
        if (enemyPos != Vector3.zero)
        {
            Vector3 direction = enemyPos - player.position;
            float rotationSpeed = 2f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float elapsedTime = 0f;
            while (elapsedTime < 0.5f)
            {
                player.rotation = Quaternion.Lerp(player.rotation, targetRotation, elapsedTime);
                elapsedTime += Time.deltaTime * rotationSpeed;
                yield return null;
            }
        }

    }
    public void MovePlayer()
    {
        if (playerMoving == false && (movePlayer == null ||movePlayer.name == player.name))
        {
            StartCoroutine(MovePlayer(path, cellSize));

            movePlayer = player;
        }
    }

    public void PlayerGrid()
    {
        int x = GetNodeFromWorldPoint(player.transform.position).gridX;
        int y = GetNodeFromWorldPoint(player.transform.position).gridY;
        // �÷��̾� �ֺ����� ���̾Ƹ�� ������� �̵� ������ ��ġ�� ǥ���մϴ�.
        /*
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // ���̾Ƹ�� ���ο� �ִ� ��ǥ���� Ȯ��

                if (Math.Abs(i - x) + Math.Abs(j - y) <= movingCount)
                {
                    playerMoveTextures[i, j].SetActive(true); // ���� ���� �ִٸ� ���� ����
                }
 

            }
        }
        */

        //�簢���ΰ��
        int minX = Mathf.Max(0, x - movingCount);
        int maxX = Mathf.Min(grid.GetLength(0) - 1, x + movingCount);
        int minY = Mathf.Max(0, y - movingCount);
        int maxY = Mathf.Min(grid.GetLength(1) - 1, y + movingCount);

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                playerMoveTextures[i, j].SetActive(true); // ���� ���� �ִٸ� ���� ����
            }
        }
    }
    #endregion �÷��̾� �����Լ�

    //�����ȿ� �ִ��� üũ�ϴ� �Լ�.
    public bool CheckInPlayerRange(Vector3 position)
    {
        int monX =GetNodeFromWorldPoint(position).gridX;
        int monY =GetNodeFromWorldPoint (position).gridY;
        int playerX = GetNodeFromWorldPoint(player.transform.position).gridX;
        int playerY = GetNodeFromWorldPoint(player.transform.position).gridY;

        //�簢���ΰ��
        int minX = Mathf.Max(0, playerX - weaponRange);
        int maxX = Mathf.Min(grid.GetLength(0) - 1, playerX + weaponRange);
        int minY = Mathf.Max(0, playerY - weaponRange);
        int maxY = Mathf.Min(grid.GetLength(1) - 1, playerY + weaponRange);

        bool isInRange = true;
        if((monX>=minX && monX<=maxX)&&(monY>=minY && monY<=maxY))
        {
        }
        else
        {
            isInRange = false;
        }

        return isInRange;
    }

    public Vector3 CheckInPlayerAttackRange()
    {
        int playerX = GetNodeFromWorldPoint(player.transform.position).gridX;
        int playerY = GetNodeFromWorldPoint(player.transform.position).gridY;

        int minX = Mathf.Max(0, playerX - 1);
        int maxX = Mathf.Min(grid.GetLength(0) - 1, playerX + 1);
        int minY = Mathf.Max(0, playerY - 1);
        int maxY = Mathf.Min(grid.GetLength(1) - 1, playerY + 1);


        for(int i=minX; i<=maxX; i++)
        {
            for(int j=minY; j<maxY; j++)
            {
                if(grid[i,j].state == Node.State.monster)
                {
                    return grid[i, j].worldPosition;
                }
            }
        }
        return Vector3.zero;
    }

    public void SetRangeTexture()
    {
        Texture2D rangePrefab = Resources.Load<Texture2D>("Range");
        rangeTexture = new GameObject("RangeTexture");
        SpriteRenderer rangeRenderer = rangeTexture.AddComponent<SpriteRenderer>();
        rangeRenderer.sprite = Sprite.Create(rangePrefab, new Rect(0, 0, rangePrefab.width, rangePrefab.height), Vector2.one * 0.5f);
        rangeTexture.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        rangeTexture.transform.localScale = new Vector3(1f, 1f, 0);
        rangeTexture.SetActive(false);
    }
    public void ActiveRangeTexture()
    {
        rangeTexture.SetActive(true);
        rangeTexture.transform.position = player.position;
    }

    public void SetTargetTexture()
    {
        targetTextures = new Queue<GameObject>();
        GameObject targetTexture = new GameObject("targetTextures");
        Texture2D targetPrefab = Resources.Load<Texture2D>("Cross1");
        GameObject texture = new GameObject("TargetTexture");
        SpriteRenderer targetRenderer = texture.AddComponent<SpriteRenderer>();
        targetRenderer.sprite = Sprite.Create(targetPrefab, new Rect(0, 0, targetPrefab.width, targetPrefab.height), Vector2.one * 0.5f);
        targetTextures.Enqueue(texture);
        texture.transform.SetParent(targetTexture.transform);
        texture.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        texture.transform.localScale = new Vector3(1f, 1f, 0);
        texture.SetActive(false);

        target2Textures = new Queue<GameObject>();
        GameObject targetTexture2 = new GameObject("targetTextures2");
        Texture2D target2Prefab = Resources.Load<Texture2D>("Cross");
        GameObject texture2 = new GameObject("Target2Texture");
        SpriteRenderer target2Renderer = texture2.AddComponent<SpriteRenderer>();
        target2Renderer.sprite = Sprite.Create(target2Prefab, new Rect(0, 0, target2Prefab.width, target2Prefab.height), Vector2.one * 0.5f);
        target2Textures.Enqueue(texture2);
        texture2.transform.SetParent(targetTexture2.transform);
        texture2.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        texture2.transform.localScale = new Vector3(1f, 1f, 0);
        texture2.SetActive(false);
    }

    public void ActiveTargetTexture(Vector3 position)
    {
        GameObject targetTexture = targetTextures.Dequeue();
        targetTexture.SetActive(true);
        targetTexture.transform.position = position;
    }

    public void ActiveTarget2Texture(Vector3 position)
    {
        GameObject target2Texture = target2Textures.Dequeue();
        target2Texture.SetActive(true);
        target2Texture.transform.position = position;
    }
    
}


// �� ��忡 ���� ������ �����ϴ� Ŭ����
public class Node
{
    public bool walkable; // �̵� ���� ����
    public Vector3 worldPosition; // ����� ���� ��ǥ
    public int gridX; // �׸��� ���� x �ε���
    public int gridY; // �׸��� ���� y �ε���
    public int gCost; // ���� ���κ����� �Ÿ�
    public Node parent; // ��� ������ ���� �θ� ���
    public State state;
    public enum State { Enemy, Player, Obstacle, Bin }// ���Ͱ� �ش��忡 �ִ��� ����.

    // ��� ������
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}