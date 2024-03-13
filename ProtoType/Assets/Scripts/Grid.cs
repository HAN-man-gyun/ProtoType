using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
public class Grid
{
    //넓이 변수
    private int width;
    //높이 변수
    private int height;
    //한칸의 크기
    private float cellSize;
    //그리드 배열
    private int[,] gridArray;
    //텍스트 배열
    private TextMesh[,] debugTextArray;
    //그리드가 항상 0,0에서 시작하지는 않기에 필요한 시작좌표
    private Vector3 originPosition;

    //그리드 객체는 생성자로 넓이, 높이, 한칸의 크기를 입력받아서 초기화함.
    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[height, width];
        debugTextArray = new TextMesh[height, width];
        //Debug.Log(width + " " + height);


        //text객체와 선을 그리기위한 for문
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                // 각 그리드 칸의 중앙에 text객체를 생성하고 debugTextArray에 각 배열에 값을 참조시킴.
                //debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z].ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * .5f, 5, Color.blue, TextAnchor.MiddleCenter);
                // 임의로 만든함수
                debugTextArray[x, z] = CreateWorldTextObject(gridArray[x,z].ToString(), GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * .5f,5,Color.blue,TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.blue, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.blue, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, width), GetWorldPosition(height, width), Color.blue, 100f); ;
        Debug.DrawLine(GetWorldPosition(height, 0), GetWorldPosition(height, width), Color.blue, 100f);
        //1,2 배열에 있는 값에 56을 넣음.
        SetValue(2, 1, 56);
    }

    #region 텍스트 객체 생성 메서드
    public TextMesh CreateWorldTextObject(string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, Transform parent = null, TextAlignment textAlignment = TextAlignment.Center, int sortingOrder = 5000)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;

        //임의로 추가한코드
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
    #region 월드포지션에서 그리드 인덱스를 구한 메서드와 그반대
    //월드포지션을 구하는 함수.
    //행과 열을 cellsize를 각각 곱하고 오리진 포지션을 더해서 월드포지션으로 변경해주고있다.
    private Vector3 GetWorldPosition(int x, int z)
    {
        // 오리진포지션이 있을경우도 계산해서 더했다.
        return new Vector3(x,0,z)* cellSize + originPosition;
    }

    //월드 포지션에서 x와 z를 cellsize로 나누어서 Int형으로 바꾸어서 소수점 아래 버리기.
    //즉 이 함수는 행과 열의 인덱스를 구해주고있다.
    private void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }
    #endregion
    #region gridArray,debugTextArray에 값을 넣는함수 혹은 반대로 값을 반환하는함수
    //gridArray에 값을 넣고, debugTextArray에 값을 넣는 함수.
    public void SetValue(int x, int z, int value)
    {
        if(x>=0 && z>=0 && x < height && z< width)
        {
            gridArray[x, z] = value;
            debugTextArray[x,z].text = gridArray[x,z].ToString();
        }
    }
    //gridArray의 내부값을 반환하는 함수
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
    #region 마우스클릭
    //마우스로 클릭해서 값을 바꾸기위한 함수들.
    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetValue(x, z, value);
    }
    //마우스로 오른쪽 클릭했을 때 grid정보를 얻기위한 메서드.
    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXZ(worldPosition, out x, out y);
        return GetValue(x, y);
    }
    //마우스위치를 인식하기위해 카메라에서 아래로 레이캐스트를 쏘는 메서드
    public Vector3 GetMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
        return hit.point;
    }
    #endregion 마우스클릭
}
