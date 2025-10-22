using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ������Ϸ״̬
/// </summary>
public class GameController : MonoBehaviour
{
    // ����ģʽ
    public static GameController Instance { get; private set; }

    private GameStatus currentStatus;  // ��ǰ��Ϸ״̬

    // ��ǰ��Ϸ�еĵ�����Ŀ
    private int enemyNums;  // ������Ŀ
    private int OurNums;    // �ҷ���Ŀ
    // ����أ����ڴ洢�������ҷ���Ϸ�����
    [SerializeField] public GameObject EnemyUnitsPool;
    [SerializeField] public GameObject OurUnitsPool;

    // ��ť����
    [SerializeField] private Button startButton;  // ��ʼ��ť
    // [SerializeField] private Button nextTurnButton;   // ��һ�غ�


    /// <summary>
    /// ��ʼ����Ϸ������
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
        // ��Ϸ״̬��ʼ��
        GameStatusInitialized();
        
    }

    private void Update()
    {
        
    }

    public void GameStatusInitialized()
    {
        currentStatus = GameStatus.BeforeGame;
        // ��ȡHierarchy�еĵ�λ�ܶ���
        GameObject unitManagerObject = GameObject.FindGameObjectWithTag("Unit");
        if (unitManagerObject != null) 
        {
            Debug.Log("��Ϸ��ʼ��");
            Transform enemyUnitsPoolTransform = unitManagerObject.transform.Find("EnemyUnitsPool");
            Transform ourUnitPoolTransform = unitManagerObject.transform.Find("OurUnitsPool");
            if (enemyUnitsPoolTransform != null)
            {
                Debug.Log("���˳س�ʼ��");
                EnemyUnitsPool = enemyUnitsPoolTransform.gameObject;
            }
            if (ourUnitPoolTransform != null)
            {
                OurUnitsPool = ourUnitPoolTransform.gameObject;
            }
        }
        
        // ��ʼ��ť����
        startButton.onClick.AddListener(OnStartButtonClick);

    }

    public void StatusChangeToInGameFromBeforeGame()
    {
        if (currentStatus == GameStatus.BeforeGame)
        {
            currentStatus = GameStatus.InGame;
        }
        // ������һ�غϰ�ť
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
            // ��Ϸ״̬�л�
            StatusChangeToInGameFromBeforeGame();
        }
        else
        {
            Debug.Log("��Ҫ����λ");
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
