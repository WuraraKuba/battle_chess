using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// 操作基本类，用于定义基本的如射线检测之类的东西
public class BaseMouseController : MonoBehaviour
{
    public static BaseMouseController Instance { get; private set; }
    // 射线检测 相关
    private Ray ray;   // 射线检测
    private RaycastHit hit;    // 用于存储射线检测的结果
    private int RaycastIgnoreLayerMask;  // 射线检测需要忽略的
    private Vector3 mousePosition;  // 当前指向位置

    private IInputHandler inputHandler;
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
        inputHandler = new InputHandler();
        RaycastIgnoreLayerMask = ~LayerMask.GetMask("trigger", "Ignore Raycast");
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "BoardScene") // 这个后面要接口化
        {
            return; // 如果不在地图场景，直接退出 Update
        }
        if (GameController.Instance.GetGameStatus() == GameStatus.InGameEnemy)  // 肯定也是要接口化的
        {  // 只在自己的回合内能操作
            return;
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000f, RaycastIgnoreLayerMask))
        {
            // UI检测
            if (IsPointerOverUIObject())  // 也要接口化，乌拉
            {// 检测是否在UI上
                return;
            }
            GameObject hitObject = hit.transform.gameObject;
            // 加问号是为了防止空引用异常而崩溃
            inputHandler?.HandleHover(mousePosition, hitObject);

  
            if (Input.GetMouseButtonDown(0))  // 鼠标左键被点击
            {
                // 当前游戏状态检查
                if (GameController.Instance.GetGameStatus() == GameStatus.BeforeGame)
                {
                    // 部署接口
                }
                else
                {
                    // 选中 + 移动
                }

            }
            if (Input.GetMouseButtonDown(1))  // 鼠标右键被点击
            {
                inputHandler?.HandleRightClick();
            }

        }

    }
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
