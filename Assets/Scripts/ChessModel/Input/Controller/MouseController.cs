using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MouseController : MonoBehaviour
{
    public static MouseController Instance;

    public static MouseEntity mouseEntity;
    private GameConfig _gameConfig;
    private IInputHandler inputHandler;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 确保它不会被销毁
            DontDestroyOnLoad(gameObject);
            // 加载全局配置文件
            _gameConfig = Resources.Load<GameConfig>("GameConfig");
            int RaycastIgnoredLayerMask = ~LayerMask.GetMask("trigger", "Ignore Raycast"); 
            mouseEntity = new MouseEntity(RaycastIgnoredLayerMask);
            inputHandler = new InputHandler(mouseEntity);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "BoardScene")
        {
            return; // 如果不在地图场景，直接退出 Update
        }
        if (GameController.Instance.GetGameStatus() == GameStatus.InGameEnemy)
        {  // 只在自己的回合内能操作
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, mouseEntity.RaycastIgnoreLayerMask))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (IsPointerOverUIObject())
            {// 检测是否在UI上
                return;
            }
            // 调用坐标转换接口
            // 为mouseEntity.mousePosition赋值
            mouseEntity.mousePosition = GridMapController.Instance.GetHexTopCenterPositionByClickPosition(hitObject.transform.position);
            // 高亮接口

            // 点击接口
            if (Input.GetMouseButtonDown(0))  // 鼠标左键被点击
            {
                Debug.Log("Hey");
                // 当前游戏状态检查
                if (GameController.Instance.GetGameStatus() == GameStatus.BeforeGame)
                {
                    // 部署接口，打算传入entity的引用
                    List<UnitData> unitData = UnitCoreController.Instance.getOurTeam();
                    // 将打开棋子选择UI
                    MapUIController.Instance.PopulateUnitSelectionUI(mouseEntity.mousePosition, unitData);
                }
                else
                {
                    // 移动接口
                    inputHandler?.HandleLeftClick(hitObject);
                }
             }
        }



    }
    // 还没想好你放在哪呢
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
