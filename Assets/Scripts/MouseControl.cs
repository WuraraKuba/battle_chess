using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// ���������صĲ���������ѡ��֮���
public class MouseControl : MonoBehaviour
{
    public AStarPathfinder aStarPathfinder;
    public Material highlightMaterial;  // �����㴴���ĸ�������
    private GameObject selectedObject;  // �洢��ǰѡ�е�����
    private Material originalMatrial;   // �洢ԭʼ����

    private UnitController selectedUnit = null;  // ��ǰ׷�ٵ�λ
    private int unitLayerIndex;

    private GameObject currentHoverObject; // ��ǰ�����ͣ������
    private Vector3 originalPosition;      // �洢��ͣ�����ԭʼλ��
    private int environmentLayerIndex;
    private int RaycastIgnoreLayerMask;

    public GridGenerator gridGenerator;
    public GridManager gridManager;

    private void Awake()
    {
        RaycastIgnoreLayerMask = ~LayerMask.GetMask("trigger", "Ignore Raycast");
        environmentLayerIndex = LayerMask.NameToLayer("environment");
        unitLayerIndex = LayerMask.NameToLayer("unit");
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        // ����Ƿ���UI�ϣ������꣩

        // if (EventSystem.current.IsPointerOverGameObject(-1))
        int pointerId = Pointer.current != null ? Pointer.current.deviceId : -1;

        bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(pointerId);
        if (IsPointerOverUIObject())
        {
            // �����UI�ϣ���ִ�������߼�
            return;
        }
        // �����������񣬶�����һ������Ӱ��λ�������λ�÷��������
        // ��������������������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // ���ڴ洢���߼��Ľ��
        RaycastHit hit;
        // ��������Ƿ�Ӵ�������
        if (Physics.Raycast(ray, out hit, 1000f, RaycastIgnoreLayerMask))  //, 100f, ~0, QueryTriggerInteraction.Ignore
        {
            // ����������Ƿ񱻰���
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.gameObject.layer == unitLayerIndex)
                {
                    // **��� A: ����˽�ɫ (ѡ��)**
                    SelectUnit(hit.transform.gameObject.GetComponent<UnitController>());

                    // ����

                }
                else if (hit.transform.gameObject.layer == environmentLayerIndex)
                {
                    // **��� B: ����˻���/���� (�ƶ�)**
                    if (selectedUnit != null)
                    {
                        TryMoveSelectedUnit(hit.point);
                    }
                    // ����û��ѡ�н�ɫ�����������Ч
                }
            }
            // ������û�б����µ����
            else
            {
                if (hit.transform.gameObject) // ����ʲô��һ���ж�,��Ϊ��ͬtag����layer��ͬ��ʽ
                {
                    // �����ǰ��ͣ�������layer����environment
                    if (hit.transform.gameObject.layer == environmentLayerIndex)
                    {
                        if (currentHoverObject != hit.transform.gameObject)
                        {
                            if (currentHoverObject != null)
                            {
                                currentHoverObject.transform.position = originalPosition;
                            }

                            // currentHoverObject = hit.transform.gameObject;
                            //Debug.Log("grid_position"+originalPosition);
                            originalPosition = hit.point;
                            // �������originalPositionȥ��ȡ��������
                            Vector3 girdPos =  gridManager.WorldToGridCoordinates(originalPosition);
                            // �ж����gridPos�����ĵ��Ƿ���Ч
                            if (gridManager.ValidGridCenter(girdPos))
                            {
                                gridManager.GenerateGrid(girdPos);
                            }
                            else
                            {
                               // Debug.Log("invalid");
                            }
                            //currentHoverObject.transform.position += new Vector3(0, 0.5f, 0);
                        }
                    }
                    
                }
            }
        }
        else  // û�м�⵽����
        {
            if (currentHoverObject != null)
            {
                currentHoverObject.transform.position = originalPosition;
            }
        }
        

    }

    void SelectUnit(UnitController unit)
    {
        // ����Ѿ���ѡ�еĽ�ɫ����ȡ��ѡ��
        if (selectedUnit != null)
        {
            selectedUnit.Deselect();
        }

        // �����µ�ѡ�н�ɫ
        selectedUnit = unit;
        selectedUnit.Select();
        // ����
        selectedUnit.TurnOnSelector();
        Debug.Log(selectedUnit.name + " �ѱ�ѡ�С�");
    }

    void TryMoveSelectedUnit(Vector3 hitPoint)
    {
        if (selectedUnit == null) return;
        // 1. ������ NavMesh ��֤����
        Vector3 gridInfo = gridManager.WorldToGridCoordinates(hitPoint);

        // 2. ��֤�����Ƿ������
        if (gridManager.ValidGridCenter(gridInfo))
        {
            // 3. �ƶ���ɫ

            selectedUnit.MoveTo(gridInfo);

            // 4. (��ѡ) �ƶ���ȡ��ѡ��׼����һ��ѡ��
            // selectedUnit.Deselect();
            // selectedUnit = null; 
        }
    }


    // �ƶ�Э��, ��νЭ��
    IEnumerator MoveAlongPath(List<Vector3> path)
    {

        // ȷ��·����Ϊ��
        if (path == null || path.Count == 0)
        {
            yield break; // �˳�Э��
        }

        float moveSpeed = 3f; // �ƶ��ٶ�
        // ��ȡAnimator���
        Animator animator = selectedObject.GetComponent<Animator>();
        // �ƶ���ʼ
        if (animator != null)
        {
            animator.SetFloat("Speed", moveSpeed);
            animator.SetFloat("MotionSpeed", moveSpeed);
        }
        // ����·���е�ÿ���ڵ�
        foreach (Vector3 targetNode in path)
        {
            // �ý�ɫ����Ŀ���
            selectedObject.transform.LookAt(targetNode);
            // �����ƶ���ֱ������Ŀ��ڵ�
            while (selectedObject.transform.position != targetNode)
            {
                Vector3 oldPos = selectedObject.transform.position;
                // ����Ŀ����ƶ�
                selectedObject.transform.position = Vector3.MoveTowards(
                    oldPos,
                    targetNode,
                    moveSpeed * Time.deltaTime
                );
                // ����unity�� ��һ��ѭ���ȵ���һ֡������
                yield return null; // �ȴ���һ֡
            }
        }
        // === �ƶ���ɣ�������ȡ��ѡ��״̬ ===
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("MotionSpeed", 0f); // �ָ��� Idle ״̬
        }
        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
        selectedObject = null;
    }

    private bool IsPointerOverUIObject()
    {
        // 1. ���� PointerEventData
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        // 2. ��ȡ Graphic Raycaster (��� Canvas �ϵ����)
        // ����������� UI ����һ����Ϊ 'MainCanvas' �� Canvas ��
        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>(); // �����ֶ��ҵ���

        if (raycaster == null) return false;

        // 3. ִ�����߼��
        raycaster.Raycast(eventData, results);

        // ��� results �б�Ϊ�գ�˵�������� UI Ԫ��
        return results.Count > 0;
    }
}
