using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HexMouseController : MonoBehaviour
{
    public static HexMouseController Instance { get; private set; }

    // 依旧还是射线检测
    private Ray ray;   // 射线检测
    private RaycastHit hit;    // 用于存储射线检测的结果
    private int RaycastIgnoreLayerMask;  // 射线检测需要忽略的
    private Vector3 mousePosition;

    private Vector3 lastCellPosition = Vector3.zero;  // 用于存储上次选择的单位坐标

    // 加个？就能让这个数据结构能被赋值为null了
    private Vector3? startPosition = null;
    private Vector3? endPosition = null;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 确保它不会被销毁
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        RaycastIgnoreLayerMask = ~LayerMask.GetMask("trigger", "Ignore Raycast");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 检测射线是否接触到物体
        if (Physics.Raycast(ray, out hit, 1000f, RaycastIgnoreLayerMask))
        {
            // Debug.Log(hit.point);
            // 尝试根据这个坐标，获取方块
            if (GridMapController.Instance != null)
            {
                mousePosition = GridMapController.Instance.GetHexTopCenterPositionByClickPosition(hit.point);
                
            }
            // 鼠标左键点击逻辑：意味开始输入起始点
            if (Input.GetMouseButtonDown(0))
            {
                if (startPosition != null)
                {
                    // 此时这个位置将是终点
                    endPosition = mousePosition;
                    // 根据起点与终点，算出路径吧
                    Vector3Int startIndex = GridMapController.Instance.Position2HexIndex(startPosition.Value);
                    Vector3Int endIndex = GridMapController.Instance.Position2HexIndex(endPosition.Value);
                    List<Vector3Int> path = GridMapController.Instance.FindPath(startIndex, endIndex);
                    // 但在此之前，先把起始点的渲染做好
                }
                else
                {
                    startPosition = mousePosition;
                    
                    
                }
            }
            // 如果是鼠标右键被点击，清除当前起点与终点
            if (Input.GetMouseButtonDown(1))
            {
                startPosition = null;
                endPosition = null;
            }
            // 高亮部分
            if(MainRenderController.Instance != null)
            {
                float cellSize = GridMapController.Instance.GetHexMapCellSize();
                // 如果 position没变，就不要改了
                if (mousePosition != lastCellPosition)
                {
                        MainRenderController.Instance.MapHexCellHighLight(mousePosition, cellSize);
                        lastCellPosition = mousePosition;
                }
               
                if (startPosition != null)  // 点击高亮逻辑
                {
                    Vector3Int startIndex = GridMapController.Instance.Position2HexIndex(startPosition.Value);
                    GridMapController.Instance.GetNeighbours(startIndex);
                    Vector3 actualStart = startPosition.Value;
                    MainRenderController.Instance.MapHexCellKeepHighLight(actualStart, cellSize);

                }
                if (endPosition != null)
                {
                    MainRenderController.Instance.ClearKeepHighlight();
                    MainRenderController.Instance.ClearHighlight();
                    startPosition = null;
                    endPosition = null;

                }
            }
        }
    }
}
