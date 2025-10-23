using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 管理游戏状态
/// </summary>
public class GameController : MonoBehaviour
{
    // 单例模式
    public static GameController Instance { get; private set; }

    private GameStatus currentStatus;  // 当前游戏状态


    // 按钮监听
    // [SerializeField] private Button nextTurnButton;   // 下一回合


    /// <summary>
    /// 初始化游戏控制器
    /// </summary>
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // 游戏状态初始化
        GameStatusInitialized();
        
    }

    private void Update()
    {
        
    }

    public void GameStatusInitialized()
    {
        currentStatus = GameStatus.BeforeGame;
        // UnitCoreController 初始化
        UnitCoreController.Instance.UnitCoreControllerInitialized();
        /*// 开始按钮监听
        startButton.onClick.AddListener(OnStartButtonClick);*/
        // UI初始化
        MapUIController.Instance.MapUIInitialized();

    }

    public void StatusChangeToInGameFromBeforeGame()
    {
        if (currentStatus == GameStatus.BeforeGame)
        {
            currentStatus = GameStatus.InGame;
        }
        // 监听下一回合按钮
        // nextTurnButton.onClick.AddListener(OnNextTurnButtonClick);
    }

    public void StatusChangeToVictoryFromInGame()
    {
        if (currentStatus == GameStatus.InGame)
        {
            currentStatus = GameStatus.Victory;
        }
    }
    public void StatusChangeToFailureFromInGame()
    {
        if (currentStatus == GameStatus.InGame)
        {
            currentStatus = GameStatus.Failure;
        }
    }

/*    private void OnNextTurnButtonClick()
    {
        
    }*/

    public GameStatus GetGameStatus()
    {
        return currentStatus;
    }

}
