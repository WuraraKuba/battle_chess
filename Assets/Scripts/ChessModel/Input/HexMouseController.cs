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

    // ���ɻ������߼��
    private Ray ray;   // ���߼��
    private RaycastHit hit;    // ���ڴ洢���߼��Ľ��
    private int RaycastIgnoreLayerMask;  // ���߼����Ҫ���Ե�
    private Vector3 mousePosition;

    private Vector3 lastCellPosition = Vector3.zero;  // ���ڴ洢�ϴ�ѡ��ĵ�λ����

    // �Ӹ���������������ݽṹ�ܱ���ֵΪnull��
    private Vector3? startPosition = null;
    private Vector3? endPosition = null;

    private float yOffset;
/*    public enum GameMode { Deployment, Pathfinding }  // ģʽ
    private GameMode currentMode = GameMode.Deployment; // ��ǰ��ͼģʽ*/
    [Header("Deploy Settings")]
    [SerializeField] private GameObject unitPrefab; // Ҫ����ĵ�λ Prefab
    [SerializeField] private Button startButton;     // ����Ϸ��ʼ����ť������

    // �򵥰汾�ĳ����л��� ������1��0��5��cell��Ϊս������λ��
    // ��ϰ�����л�
    private readonly Vector3Int battleTriggerHex = new Vector3Int(1, 0, 5);
    [SerializeField] private string battleSceneName = "SampleScene";
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

/*        // ��ʱ��󶨵���ť
        startButton.onClick.AddListener(OnStartGameClicked);

        // ȷ��һ��ʼֻ�� startButton �ǿɼ��ģ��Ҵ��� Deployment ģʽ
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
            return; // ������ڵ�ͼ������ֱ���˳� Update
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // ��������Ƿ�Ӵ�������
        if (Physics.Raycast(ray, out hit, 1000f, RaycastIgnoreLayerMask))
        {
            if (IsPointerOverUIObject())
            {// ����Ƿ���UI��
                return;
            }
            // ���Ը���������꣬��ȡ����
            if (GridMapController.Instance != null)
            {
                mousePosition = GridMapController.Instance.GetHexTopCenterPositionByClickPosition(hit.point);
                
            }
            // ����������߼�����ζ��ʼ������ʼ��
            if (Input.GetMouseButtonDown(0))
            {

                if (GameController.Instance.GetGameStatus() == GameStatus.BeforeGame)
                {
                    // ��ǰ���ڲ���ģʽ��
                    // ��ȡUnitDatas
                    List<UnitData> unitData = UnitCoreController.Instance.getOurTeam();
                    // ��������ѡ��UI
                    MapUIController.Instance.PopulateUnitSelectionUI(mousePosition, unitData);
                    // Ŀǰֻ�ܲ���һ����λ
                    /*if (UnitCoreController.Instance.deployedUnit == null)
                    {
                        // �����߼���ֱ���ڵ���ĸ�����ʵ���� Prefab
                        UnitCoreController.Instance.DeployUnit(mousePosition);
                    }
                    else
                    {
                        // �Ѿ��ڵ�ͼ�ϣ���Ϊ���²���������ɵģ��������µ�
                        UnitCoreController.Instance.RedeployUnit(mousePosition);
                    }*/

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
                        UnitMapMovementController.Instance.StartMovement(UnitCoreController.Instance.deployedUnit, path);
                        // �ƶ���ɣ������Ƿ񴥷�ս��
                        Vector3 SelectObjectPosion = UnitCoreController.Instance.deployedUnit.transform.position;
                        SelectObjectPosion.y -= 1.5f;  // ��ʱ����
                        Vector3Int objectIndex = GridMapController.Instance.Position2HexIndex(SelectObjectPosion);
                        Debug.Log("�������ڵ�����" +  objectIndex);   
                        CheckForBattleTrigger(objectIndex);

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

    /// <summary>
    /// �����л�����ʱ����
    /// </summary>
    /// <param name="finalIndex"></param>
    /// <param name="battleTriggerHex"></param>
    public void CheckForBattleTrigger(Vector3Int finalIndex)
    {
        // �Ƚ�����λ���Ƿ���ڴ���λ��
        if (finalIndex.Equals(battleTriggerHex))
        {
            Debug.Log($"��⵽ս��������Ŀ�� Hex: {battleTriggerHex}");

            // ������������
            LoadBattleScene();
        }
    }

    private void LoadBattleScene()
    {
        // 1. ��鳡���Ƿ�����ӵ� Build Settings
        if (Application.CanStreamedLevelBeLoaded(battleSceneName))
        {
            // 2. ���س��� (ʹ���첽���ظ��ã���ͬ���������)
            SceneManager.LoadScene(battleSceneName);
        }
        else
        {
            Debug.LogError($"�޷�����ս������ '{battleSceneName}'����ȷ����������ӵ� File -> Build Settings �С�");
        }
    }

}
