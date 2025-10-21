using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMouseController : MonoBehaviour
{
    public static HexMouseController Instance { get; private set; }

    // ���ɻ������߼��
    private Ray ray;   // ���߼��
    private RaycastHit hit;    // ���ڴ洢���߼��Ľ��
    private int RaycastIgnoreLayerMask;  // ���߼����Ҫ���Ե�
    private Vector3 mousePosition;

    private Vector3 lastCellPosition = Vector3.zero;  // ���ڴ洢�ϴ�ѡ��ĵ�λ����

    // �Ӹ���������������ݽṹ�ܱ���ֵΪnull��
    private Vector3? startPosition = null;
    private Vector3? endPosition = null;

    // ������ս��ģʽ���򵥰汾
    private GameObject deployedUnit = null;  // ��λ
    private float yOffset;
    public enum GameMode { Deployment, Pathfinding }  // ģʽ
    private GameMode currentMode = GameMode.Deployment; // ��ǰ��ͼģʽ
    [Header("Deploy Settings")]
    [SerializeField] private GameObject unitPrefab; // Ҫ����ĵ�λ Prefab
    [SerializeField] private Button startButton;     // ����Ϸ��ʼ����ť������

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ȷ�������ᱻ����
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        RaycastIgnoreLayerMask = ~LayerMask.GetMask("trigger", "Ignore Raycast");
        yOffset = unitPrefab.GetComponent<CapsuleCollider>().height / 2;

        // ��ʱ��󶨵���ť
        startButton.onClick.AddListener(OnStartGameClicked);

        // ȷ��һ��ʼֻ�� startButton �ǿɼ��ģ��Ҵ��� Deployment ģʽ
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
        // ��������Ƿ�Ӵ�������
        if (Physics.Raycast(ray, out hit, 1000f, RaycastIgnoreLayerMask))
        {
            // Debug.Log(hit.point);
            // ���Ը���������꣬��ȡ����
            if (GridMapController.Instance != null)
            {
                mousePosition = GridMapController.Instance.GetHexTopCenterPositionByClickPosition(hit.point);
                
            }
            // ����������߼�����ζ��ʼ������ʼ��
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUIObject())
                {
                    // �������� UI Ԫ�� (���簴ť)���������˳��������к����� 3D ���߼��
                    // ��ť�¼�����ᴦ����
                    return;
                }
                if (currentMode == GameMode.Deployment)
                {
                    // ��ǰ���ڲ���ģʽ��
                    // ֻ�ܲ���һ����λ
                    if (deployedUnit == null)
                    {
                        // �����߼���ֱ���ڵ���ĸ�����ʵ���� Prefab
                        DeployUnit(mousePosition);
                    }
                    else
                    {
                        // �Ѿ��ڵ�ͼ�ϣ���Ϊ���²���������ɵģ��������µ�
                        RedeployUnit(mousePosition);
                    }

                }
                else
                {
                    if (startPosition != null)
                    {
                        // ��ʱ���λ�ý����յ�
                        endPosition = mousePosition;
                        // ����������յ㣬���·����
                        Vector3Int startIndex = GridMapController.Instance.Position2HexIndex(startPosition.Value);
                        Vector3Int endIndex = GridMapController.Instance.Position2HexIndex(endPosition.Value);
                        List<Vector3> path = GridMapController.Instance.FindPath(startIndex, endIndex);
                        // ���ڴ�֮ǰ���Ȱ���ʼ�����Ⱦ����
                        UnitMapMovementController.Instance.StartMovement(deployedUnit, path);
                    }
                    else
                    {
                        mousePosition.y -= yOffset;
                        startPosition = mousePosition;
                        Vector3Int startIndex = GridMapController.Instance.Position2HexIndex(startPosition.Value);
                        Debug.Log("��ǰ��Ӧ�ĸ���" + startIndex);

                    }
                }
                
            }
            // ���������Ҽ�������������ǰ������յ�
            if (Input.GetMouseButtonDown(1))
            {
                startPosition = null;
                endPosition = null;
                MainRenderController.Instance.ClearKeepHighlight();
                MainRenderController.Instance.ClearHighlight();
            }
            // ��������
            if(MainRenderController.Instance != null)
            {
                float cellSize = GridMapController.Instance.GetHexMapCellSize();
                // ��� positionû�䣬�Ͳ�Ҫ����
                if (mousePosition != lastCellPosition)
                {
                        MainRenderController.Instance.MapHexCellHighLight(mousePosition, cellSize);
                        lastCellPosition = mousePosition;
                }
               
                if (startPosition != null)  // ��������߼�
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

    // ����Ϸ��ʼ����ť�ĵ��������
    private void OnStartGameClicked()
    {
        if (deployedUnit != null)
        {
            currentMode = GameMode.Pathfinding;
            startButton.gameObject.SetActive(false); // ���ذ�ť������Ѱ·ģʽ
            Debug.Log("������ɣ�����Ѱ·ģʽ��");
        }
        else
        {
            Debug.LogWarning("���ȵ����ͼ����һ����λ��");
        }
    }

    private void DeployUnit(Vector3 deployPosition)
    {

        deployPosition.y += yOffset;
        // 2. ʵ������λ
        GameObject newUnit = Instantiate(unitPrefab, deployPosition, Quaternion.identity);

        // 3. �洢����
        deployedUnit = newUnit;

        Debug.Log($"��λ�Ѳ����� {deployPosition}������ '��Ϸ��ʼ' ��ť�л�ģʽ��");
    }

    private void RedeployUnit(Vector3 deployNewPosition)
    {
        // ���پɵ�λ
        Destroy(deployedUnit);
        // �����µ�λ
        DeployUnit(deployNewPosition);
    }

    /// <summary>
    /// ��鵱ǰ������λ���Ƿ�����һ�� UI Ԫ�ظ��ǡ�
    /// </summary>
    /// <returns>������λ�� UI Ԫ��֮�ϣ��򷵻� true��</returns>
    private bool IsPointerOverUIObject()
    {
        // 1. ��ȡ��ǰ EventSystem
        EventSystem eventSystem = EventSystem.current;

        // 2. ��� EventSystem �Ƿ����
        if (eventSystem == null)
        {
            return false; // ���û���¼�ϵͳ�����޷����
        }

        // 3. ���� PointerEventData �����õ�ǰ���λ��
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        // 4. ������ UI Ԫ�ؽ�������Ͷ��
        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(eventData, results);

        // 5. ������б��Ƿ�����κ� UI Ԫ��
        // ע�⣺����� Count > 0 ���������� Canvas �ϵĿɽ��� UI
        return results.Count > 0;
    }

}
