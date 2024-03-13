using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
public class Grid
{
    //���� ����
    private int width;
    //���� ����
    private int height;
    //��ĭ�� ũ��
    private float cellSize;
    //�׸��� �迭
    private int[,] gridArray;
    //�ؽ�Ʈ �迭
    private TextMesh[,] debugTextArray;
    //�׸��尡 �׻� 0,0���� ���������� �ʱ⿡ �ʿ��� ������ǥ
    private Vector3 originPosition;

    //�׸��� ��ü�� �����ڷ� ����, ����, ��ĭ�� ũ�⸦ �Է¹޾Ƽ� �ʱ�ȭ��.
    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[height, width];
        debugTextArray = new TextMesh[height, width];
        //Debug.Log(width + " " + height);


        //text��ü�� ���� �׸������� for��
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                // �� �׸��� ĭ�� �߾ӿ� text��ü�� �����ϰ� debugTextArray�� �� �迭�� ���� ������Ŵ.
                //debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z].ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * .5f, 5, Color.blue, TextAnchor.MiddleCenter);
                // ���Ƿ� �����Լ�
                debugTextArray[x, z] = CreateWorldTextObject(gridArray[x,z].ToString(), GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * .5f,5,Color.blue,TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.blue, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.blue, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, width), GetWorldPosition(height, width), Color.blue, 100f); ;
        Debug.DrawLine(GetWorldPosition(height, 0), GetWorldPosition(height, width), Color.blue, 100f);
        //1,2 �迭�� �ִ� ���� 56�� ����.
        SetValue(2, 1, 56);
    }

    #region �ؽ�Ʈ ��ü ���� �޼���
    public TextMesh CreateWorldTextObject(string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, Transform parent = null, TextAlignment textAlignment = TextAlignment.Center, int sortingOrder = 5000)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;

        //���Ƿ� �߰����ڵ�
        transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();

        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
    #endregion
    #region ���������ǿ��� �׸��� �ε����� ���� �޼���� �׹ݴ�
    //������������ ���ϴ� �Լ�.
    //��� ���� cellsize�� ���� ���ϰ� ������ �������� ���ؼ� �������������� �������ְ��ִ�.
    private Vector3 GetWorldPosition(int x, int z)
    {
        // �������������� ������쵵 ����ؼ� ���ߴ�.
        return new Vector3(x,0,z)* cellSize + originPosition;
    }

    //���� �����ǿ��� x�� z�� cellsize�� ����� Int������ �ٲپ �Ҽ��� �Ʒ� ������.
    //�� �� �Լ��� ��� ���� �ε����� �����ְ��ִ�.
    private void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }
    #endregion
    #region gridArray,debugTextArray�� ���� �ִ��Լ� Ȥ�� �ݴ�� ���� ��ȯ�ϴ��Լ�
    //gridArray�� ���� �ְ�, debugTextArray�� ���� �ִ� �Լ�.
    public void SetValue(int x, int z, int value)
    {
        if(x>=0 && z>=0 && x < height && z< width)
        {
            gridArray[x, z] = value;
            debugTextArray[x,z].text = gridArray[x,z].ToString();
        }
    }
    //gridArray�� ���ΰ��� ��ȯ�ϴ� �Լ�
    public int GetValue(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x, z];
        }
        else
        {
            return 0;
        }
    }
    #endregion
    #region ���콺Ŭ��
    //���콺�� Ŭ���ؼ� ���� �ٲٱ����� �Լ���.
    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetValue(x, z, value);
    }
    //���콺�� ������ Ŭ������ �� grid������ ������� �޼���.
    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXZ(worldPosition, out x, out y);
        return GetValue(x, y);
    }
    //���콺��ġ�� �ν��ϱ����� ī�޶󿡼� �Ʒ��� ����ĳ��Ʈ�� ��� �޼���
    public Vector3 GetMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
        return hit.point;
    }
    #endregion ���콺Ŭ��
}
