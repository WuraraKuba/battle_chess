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

    // 当前游戏中的敌我数目
    private int enemyNums;  // 敌人数目
    private int OurNums;    // 我方数目
    // 对象池，用于存储敌人与我方游戏对象的
    [SerializeField] public GameObject EnemyUnitsPool;
    [SerializeField] public GameObject OurUnitsPool;

    // 按钮监听
    [SerializeField] private Button startButton;  // 开始按钮
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
        // 获取Hierarchy中的单位总对象
        GameObject unitManagerObject = GameObject.FindGameObjectWithTag("Unit");
        if (unitManagerObject != null) 
        {
            Debug.Log("游戏初始化");
            Transform enemyUnitsPoolTransform = unitManagerObject.transform.Find("EnemyUnitsPool");
            Transform ourUnitPoolTransform = unitManagerObject.transform.Find("OurUnitsPool");
            if (enemyUnitsPoolTransform != null)
            {
                Debug.Log("敌人池初始化");
                EnemyUnitsPool = enemyUnitsPoolTransform.gameObject;
            }
            if (ourUnitPoolTransform != null)
            {
                OurUnitsPool = ourUnitPoolTransform.gameObject;
            }
        }
        
        // 开始按钮监听
        startButton.onClick.AddListener(OnStartButtonClick);

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

    public void GetEnemiesNums()
    {
        if (EnemyUnitsPool != null)
        {
            enemyNums = EnemyUnitsPool.transform.childCount;
        }
    }

    private void GetOurNums()
    {
        if (OurUnitsPool != null)
        {
            OurNums = OurUnitsPool.transform.childCount;
        }
        
    }

    private void OnStartButtonClick()
    {
        GetOurNums();
        GetEnemiesNums();
        if (OurNums > 0)
        {
            startButton.gameObject.SetActive(false);
            // 游戏状态切换
            StatusChangeToInGameFromBeforeGame();
        }
        else
        {
            Debug.Log("需要部署单位");
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
