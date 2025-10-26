using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

    private float yOffset;
/*    public enum GameMode { Deployment, Pathfinding }  // 模式
    private GameMode currentMode = GameMode.Deployment; // 当前地图模式*/
    [Header("Deploy Settings")]
    [SerializeField] private GameObject unitPrefab; // 要部署的单位 Prefab
    [SerializeField] private Button startButton;     // “游戏开始”按钮的引用

    // 简单版本的场景切换， 以索引1，0，5的cell作为战斗触发位置
    // 练习场景切换
    private readonly Vector3Int battleTriggerHex = new Vector3Int(1, 0, 5);
    [SerializeField] private string battleSceneName = "SampleScene";
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

/*        // 将时间绑定到按钮
        startButton.onClick.AddListener(OnStartGameClicked);

        // 确保一开始只有 startButton 是可见的，且处于 Deployment 模式
        currentMode = GameMode.Deployment;*/
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "BoardScene")
        {
            return; // 如果不在地图场景，直接退出 Update
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 检测射线是否接触到物体
        if (Physics.Raycast(ray, out hit, 1000f, RaycastIgnoreLayerMask))
        {
            if (IsPointerOverUIObject())
            {// 检测是否在UI上
                return;
            }
            // 尝试根据这个坐标，获取方块
            if (GridMapController.Instance != null)
            {
                mousePosition = GridMapController.Instance.GetHexTopCenterPositionByClickPosition(hit.point);
                
            }
            // 鼠标左键点击逻辑：意味开始输入起始点
            if (Input.GetMouseButtonDown(0))
            {

                if (GameController.Instance.GetGameStatus() == GameStatus.BeforeGame)
                {
                    // 当前属于部署模式下
                    // 获取UnitDatas
                    List<UnitData> unitData = UnitCoreController.Instance.getOurTeam();
                    // 将打开棋子选择UI
                    MapUIController.Instance.PopulateUnitSelectionUI(mousePosition, unitData);
                    // 目前只能部署一个单位
                    /*if (UnitCoreController.Instance.deployedUnit == null)
                    {
                        // 部署逻辑：直接在点击的格子上实例化 Prefab
                        UnitCoreController.Instance.DeployUnit(mousePosition);
                    }
                    else
                    {
                        // 已经在地图上，视为重新部署：先清除旧的，再生成新的
                        UnitCoreController.Instance.RedeployUnit(mousePosition);
                    }*/

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
                        UnitMapMovementController.Instance.StartMovement(UnitCoreController.Instance.deployedUnit, path);
                        // 移动完成，看看是否触发战斗
                        Vector3 SelectObjectPosion = UnitCoreController.Instance.deployedUnit.transform.position;
                        SelectObjectPosion.y -= 1.5f;  // 暂时方案
                        Vector3Int objectIndex = GridMapController.Instance.Position2HexIndex(SelectObjectPosion);
                        Debug.Log("物体所在的索引" +  objectIndex);   
                        CheckForBattleTrigger(objectIndex);

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

    /// <summary>
    /// 场景切换的临时方案
    /// </summary>
    /// <param name="finalIndex"></param>
    /// <param name="battleTriggerHex"></param>
    public void CheckForBattleTrigger(Vector3Int finalIndex)
    {
        // 比较最终位置是否等于触发位置
        if (finalIndex.Equals(battleTriggerHex))
        {
            Debug.Log($"检测到战斗触发！目标 Hex: {battleTriggerHex}");

            // 启动场景加载
            LoadBattleScene();
        }
    }

    private void LoadBattleScene()
    {
        // 1. 检查场景是否已添加到 Build Settings
        if (Application.CanStreamedLevelBeLoaded(battleSceneName))
        {
            // 2. 加载场景 (使用异步加载更好，但同步加载最简单)
            SceneManager.LoadScene(battleSceneName);
        }
        else
        {
            Debug.LogError($"无法加载战斗场景 '{battleSceneName}'。请确保场景已添加到 File -> Build Settings 中。");
        }
    }

}
