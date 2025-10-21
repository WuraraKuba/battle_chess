using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    // 部署与战略模式，简单版本
    private GameObject deployedUnit = null;  // 单位
    private float yOffset;
    public enum GameMode { Deployment, Pathfinding }  // 模式
    private GameMode currentMode = GameMode.Deployment; // 当前地图模式
    [Header("Deploy Settings")]
    [SerializeField] private GameObject unitPrefab; // 要部署的单位 Prefab
    [SerializeField] private Button startButton;     // “游戏开始”按钮的引用

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
        yOffset = unitPrefab.GetComponent<CapsuleCollider>().height / 2;

        // 将时间绑定到按钮
        startButton.onClick.AddListener(OnStartGameClicked);

        // 确保一开始只有 startButton 是可见的，且处于 Deployment 模式
        currentMode = GameMode.Deployment;
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
                if (IsPointerOverUIObject())
                {
                    // 如果点击了 UI 元素 (比如按钮)，则立即退出，不进行后续的 3D 射线检测
                    // 按钮事件本身会处理点击
                    return;
                }
                if (currentMode == GameMode.Deployment)
                {
                    // 当前属于部署模式下
                    // 只能部署一个单位
                    if (deployedUnit == null)
                    {
                        // 部署逻辑：直接在点击的格子上实例化 Prefab
                        DeployUnit(mousePosition);
                    }
                    else
                    {
                        // 已经在地图上，视为重新部署：先清除旧的，再生成新的
                        RedeployUnit(mousePosition);
                    }

                }
                else
                {
                    if (startPosition != null)
                    {
                        // 此时这个位置将是终点
                        endPosition = mousePosition;
                        // 根据起点与终点，算出路径吧
                        Vector3Int startIndex = GridMapController.Instance.Position2HexIndex(startPosition.Value);
                        Vector3Int endIndex = GridMapController.Instance.Position2HexIndex(endPosition.Value);
                        List<Vector3> path = GridMapController.Instance.FindPath(startIndex, endIndex);
                        // 但在此之前，先把起始点的渲染做好
                        UnitMapMovementController.Instance.StartMovement(deployedUnit, path);
                    }
                    else
                    {
                        mousePosition.y -= yOffset;
                        startPosition = mousePosition;
                        Vector3Int startIndex = GridMapController.Instance.Position2HexIndex(startPosition.Value);
                        Debug.Log("当前对应的格子" + startIndex);

                    }
                }
                
            }
            // 如果是鼠标右键被点击，清除当前起点与终点
            if (Input.GetMouseButtonDown(1))
            {
                startPosition = null;
                endPosition = null;
                MainRenderController.Instance.ClearKeepHighlight();
                MainRenderController.Instance.ClearHighlight();
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

    // “游戏开始”按钮的点击处理方法
    private void OnStartGameClicked()
    {
        if (deployedUnit != null)
        {
            currentMode = GameMode.Pathfinding;
            startButton.gameObject.SetActive(false); // 隐藏按钮，锁定寻路模式
            Debug.Log("部署完成，进入寻路模式。");
        }
        else
        {
            Debug.LogWarning("请先点击地图部署一个单位！");
        }
    }

    private void DeployUnit(Vector3 deployPosition)
    {

        deployPosition.y += yOffset;
        // 2. 实例化单位
        GameObject newUnit = Instantiate(unitPrefab, deployPosition, Quaternion.identity);

        // 3. 存储引用
        deployedUnit = newUnit;

        Debug.Log($"单位已部署在 {deployPosition}。请点击 '游戏开始' 按钮切换模式。");
    }

    private void RedeployUnit(Vector3 deployNewPosition)
    {
        // 销毁旧单位
        Destroy(deployedUnit);
        // 部署新单位
        DeployUnit(deployNewPosition);
    }

    /// <summary>
    /// 检查当前鼠标或触摸位置是否正被一个 UI 元素覆盖。
    /// </summary>
    /// <returns>如果鼠标位于 UI 元素之上，则返回 true。</returns>
    private bool IsPointerOverUIObject()
    {
        // 1. 获取当前 EventSystem
        EventSystem eventSystem = EventSystem.current;

        // 2. 检查 EventSystem 是否存在
        if (eventSystem == null)
        {
            return false; // 如果没有事件系统，则无法检查
        }

        // 3. 创建 PointerEventData 并设置当前鼠标位置
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        // 4. 对所有 UI 元素进行射线投射
        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(eventData, results);

        // 5. 检查结果列表是否包含任何 UI 元素
        // 注意：这里的 Count > 0 包含了所有 Canvas 上的可交互 UI
        return results.Count > 0;
    }

}
