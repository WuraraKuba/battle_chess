using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

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
                if (startPosition != null)
                {
                    // ��ʱ���λ�ý����յ�
                    endPosition = mousePosition;
                    // ����������յ㣬���·����
                    Vector3Int startIndex = GridMapController.Instance.Position2HexIndex(startPosition.Value);
                    Vector3Int endIndex = GridMapController.Instance.Position2HexIndex(endPosition.Value);
                    List<Vector3Int> path = GridMapController.Instance.FindPath(startIndex, endIndex);
                    // ���ڴ�֮ǰ���Ȱ���ʼ�����Ⱦ����
                }
                else
                {
                    startPosition = mousePosition;
                    
                    
                }
            }
            // ���������Ҽ�������������ǰ������յ�
            if (Input.GetMouseButtonDown(1))
            {
                startPosition = null;
                endPosition = null;
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
}
