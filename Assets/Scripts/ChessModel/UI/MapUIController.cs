using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUIController : MonoBehaviour
{
    public static MapUIController Instance {  get; private set; }
    [Header("Start Button")]
    [SerializeField] private Button startButton;  // ��ʼ��ť
    [Header("Deploy Button")]
    [SerializeField] private Button deployButton;
    private List<GameObject> activeButtons = new List<GameObject>();
    public GameObject unitButtonPrefab;
    public Action OnDeployStartClicked;
    public GameObject deployOverlayPanel;
    // �������ְ�ť�¼�

    // ����ҵ�����
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
        // ����ȷ�ϰ�ť
/*        deployButton.onClick.AddListener(OnDeployClicked);

        deployButton.interactable = true;*/
    }
    /// <summary>
    /// ��ͼUI��ʼ��
    /// </summary>
    public void MapUIInitialized()
    {
        // ��ʼ��ť����
        startButton.onClick.AddListener(OnStartButtonClick);
        deployOverlayPanel.SetActive(false);
        unitDeployUI = deployOverlayPanel.GetComponent<UnitDeployUI>();
        // ����ť����
        GlobalEvents.OnAnyUnitDeployed += HandleDeploymentComplete;

    }

    private void OnDeployClicked()
    {
        // 1. ֪ͨ���ؽű�������׼��������
        OnDeployStartClicked?.Invoke();

        Debug.Log($"MapUIController����֪ͨ��ʼ�������̡�");

        // 2. ���� UI��׼�����յ�ͼ���
        // ��һ����������ؽű��������� UI ȷʵ��Ҫ������
        gameObject.SetActive(false);
    }

    // ѡ�������
    public void PopulateUnitSelectionUI(Vector3 mouseLoc, List<UnitData> unitDatas)
    {
        deployOverlayPanel.SetActive(true);
        startButton.gameObject.SetActive(false);

        unitDeployUI.DeployButtons(unitDatas, mouseLoc, ref activeButtons);

    }
    private void HandleDeploymentComplete()
    {
        // MapUIController ��ְ�𣺹ر� UI
        deployOverlayPanel.SetActive(false);
        startButton.gameObject.SetActive(true);
        Debug.Log("MapUIController �յ���������źţ�ִ�����ز�����");

    }

    private void OnStartButtonClick()
    {
        int OurNum = UnitCoreController.Instance.GetOurNums();
        int EnemiesNum = UnitCoreController.Instance.GetEnemiesNums();
        if (OurNum > 0)
        {
            startButton.gameObject.SetActive(false);
            // ��Ϸ״̬�л�
            GameController.Instance.StatusChangeToInGameFromBeforeGame();
        }
        else
        {
            Debug.Log("��Ҫ����λ");
        }
    }

   
}
