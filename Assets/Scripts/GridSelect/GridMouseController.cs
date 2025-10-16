using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ڲ����Ը���Ϊ��λ����ѡ��
public class GridMouseController : MonoBehaviour
{
    // ���ɻ������߼��
    private Ray ray;   // ���߼��
    private RaycastHit hit;    // ���ڴ洢���߼��Ľ��
    private int RaycastIgnoreLayerMask;  // ���߼����Ҫ���Ե�

    private Vector3 lastCellPosition = Vector3.zero;  // ���ڴ洢�ϴ�ѡ��ĵ�λ����
    public static GridMouseController Instance { get; private set; }
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
            if (GridMapController.Instance != null && MainRenderController.Instance != null)
            {
                Vector3 position = GridMapController.Instance.GetCubeTopCenterPositionByClickPosition(hit.point);
                float cellSize = GridMapController.Instance.GetMapCellSize();
                // ��� positionû�䣬�Ͳ�Ҫ����
                if (position != lastCellPosition)
                {
                    MainRenderController.Instance.MapGridCellHighLight(position, cellSize);
                    lastCellPosition = position;
                }
                
            }

        }
    }
}
