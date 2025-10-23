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


    // ��ť����
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
        // UnitCoreController ��ʼ��
        UnitCoreController.Instance.UnitCoreControllerInitialized();
        /*// ��ʼ��ť����
        startButton.onClick.AddListener(OnStartButtonClick);*/
        // UI��ʼ��
        MapUIController.Instance.MapUIInitialized();

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

/*    private void OnNextTurnButtonClick()
    {
        
    }*/

    public GameStatus GetGameStatus()
    {
        return currentStatus;
    }

}
