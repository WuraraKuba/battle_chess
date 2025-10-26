using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUIController : MonoBehaviour
{
    public static MapUIController Instance {  get; private set; }
    [Header("Start Button")]
    [SerializeField] private Button startButton;  // 开始按钮
    [Header("Deploy Button")]
    [SerializeField] private Button deployButton;
    private List<GameObject> activeButtons = new List<GameObject>();
    public GameObject unitButtonPrefab;
    public Action OnDeployStartClicked;
    public GameObject deployOverlayPanel;
    // 监听各种按钮事件

    // 各种业务类吧
    UnitDeployUI unitDeployUI; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // 部署确认按钮
/*        deployButton.onClick.AddListener(OnDeployClicked);

        deployButton.interactable = true;*/
    }
    /// <summary>
    /// 地图UI初始化
    /// </summary>
    public void MapUIInitialized()
    {
        // 开始按钮监听
        startButton.onClick.AddListener(OnStartButtonClick);
        deployOverlayPanel.SetActive(false);
        unitDeployUI = deployOverlayPanel.GetComponent<UnitDeployUI>();
        // 部署按钮监听
        GlobalEvents.OnAnyUnitDeployed += HandleDeploymentComplete;

    }

    private void OnDeployClicked()
    {
        // 1. 通知主控脚本：我们准备部署了
        OnDeployStartClicked?.Invoke();

        Debug.Log($"MapUIController：已通知开始部署流程。");

        // 2. 隐藏 UI，准备接收地图点击
        // 这一步最好由主控脚本来做，但 UI 确实需要先隐藏
        gameObject.SetActive(false);
    }

    // 选择部署界面
    public void PopulateUnitSelectionUI(Vector3 mouseLoc, List<UnitData> unitDatas)
    {
        deployOverlayPanel.SetActive(true);
        startButton.gameObject.SetActive(false);

        unitDeployUI.DeployButtons(unitDatas, mouseLoc, ref activeButtons);

    }
    private void HandleDeploymentComplete()
    {
        // MapUIController 的职责：关闭 UI
        deployOverlayPanel.SetActive(false);
        startButton.gameObject.SetActive(true);
        Debug.Log("MapUIController 收到部署完成信号，执行隐藏操作。");

    }

    private void OnStartButtonClick()
    {
        int OurNum = UnitCoreController.Instance.GetOurNums();
        int EnemiesNum = UnitCoreController.Instance.GetEnemiesNums();
        if (OurNum > 0)
        {
            startButton.gameObject.SetActive(false);
            // 游戏状态切换
            GameController.Instance.StatusChangeToInGameFromBeforeGame();
        }
        else
        {
            Debug.Log("需要部署单位");
        }
    }

   
}
