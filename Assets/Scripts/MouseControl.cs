using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 用于鼠标相关的操作，包括选择，之类的
public class MouseControl : MonoBehaviour
{
    public AStarPathfinder aStarPathfinder;
    public Material highlightMaterial;  // 拖入你创建的高亮材质
    private GameObject selectedObject;  // 存储当前选中的物体
    private Material originalMatrial;   // 存储原始材质

    private UnitController selectedUnit = null;  // 当前追踪单位
    private int unitLayerIndex;

    private GameObject currentHoverObject; // 当前鼠标悬停的物体
    private Vector3 originalPosition;      // 存储悬停物体的原始位置
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
        // 检查是否在UI上（针对鼠标）

        // if (EventSystem.current.IsPointerOverGameObject(-1))
        int pointerId = Pointer.current != null ? Pointer.current.deviceId : -1;

        bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(pointerId);
        if (IsPointerOverUIObject())
        {
            // 鼠标在UI上，不执行射线逻辑
            return;
        }
        // 无论鼠标点击与否，都创建一个从摄影机位置向鼠标位置发射的射线
        // 这个是用于物理世界检测的
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 用于存储射线检测的结果
        RaycastHit hit;
        // 检测射线是否接触到物体
        if (Physics.Raycast(ray, out hit, 1000f, RaycastIgnoreLayerMask))  //, 100f, ~0, QueryTriggerInteraction.Ignore
        {
            // 检测鼠标左键是否被按下
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.gameObject.layer == unitLayerIndex)
                {
                    // **情况 A: 点击了角色 (选择)**
                    SelectUnit(hit.transform.gameObject.GetComponent<UnitController>());

                    // 高亮

                }
                else if (hit.transform.gameObject.layer == environmentLayerIndex)
                {
                    // **情况 B: 点击了环境/格子 (移动)**
                    if (selectedUnit != null)
                    {
                        TryMoveSelectedUnit(hit.point);
                    }
                    // 否则：没有选中角色，点击格子无效
                }
            }
            // 鼠标左键没有被按下的情况
            else
            {
                if (hit.transform.gameObject) // 不管什么，一律判断,因为不同tag或者layer不同方式
                {
                    // 如果当前悬停的物体的layer属于environment
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
                            // 根据这个originalPosition去获取棋盘坐标
                            Vector3 girdPos =  gridManager.WorldToGridCoordinates(originalPosition);
                            // 判断这个gridPos的中心点是否有效
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
        else  // 没有检测到物体
        {
            if (currentHoverObject != null)
            {
                currentHoverObject.transform.position = originalPosition;
            }
        }
        

    }

    void SelectUnit(UnitController unit)
    {
        // 如果已经有选中的角色，先取消选择
        if (selectedUnit != null)
        {
            selectedUnit.Deselect();
        }

        // 设置新的选中角色
        selectedUnit = unit;
        selectedUnit.Select();
        // 高亮
        selectedUnit.TurnOnSelector();
        Debug.Log(selectedUnit.name + " 已被选中。");
    }

    void TryMoveSelectedUnit(Vector3 hitPoint)
    {
        if (selectedUnit == null) return;
        // 1. 正常的 NavMesh 验证流程
        Vector3 gridInfo = gridManager.WorldToGridCoordinates(hitPoint);

        // 2. 验证格子是否可行走
        if (gridManager.ValidGridCenter(gridInfo))
        {
            // 3. 移动角色

            selectedUnit.MoveTo(gridInfo);

            // 4. (可选) 移动后取消选择，准备下一次选择
            // selectedUnit.Deselect();
            // selectedUnit = null; 
        }
    }


    // 移动协程, 所谓协程
    IEnumerator MoveAlongPath(List<Vector3> path)
    {

        // 确保路径不为空
        if (path == null || path.Count == 0)
        {
            yield break; // 退出协程
        }

        float moveSpeed = 3f; // 移动速度
        // 获取Animator组件
        Animator animator = selectedObject.GetComponent<Animator>();
        // 移动开始
        if (animator != null)
        {
            animator.SetFloat("Speed", moveSpeed);
            animator.SetFloat("MotionSpeed", moveSpeed);
        }
        // 遍历路径中的每个节点
        foreach (Vector3 targetNode in path)
        {
            // 让角色朝向目标点
            selectedObject.transform.LookAt(targetNode);
            // 持续移动，直到到达目标节点
            while (selectedObject.transform.position != targetNode)
            {
                Vector3 oldPos = selectedObject.transform.position;
                // 朝着目标点移动
                selectedObject.transform.position = Vector3.MoveTowards(
                    oldPos,
                    targetNode,
                    moveSpeed * Time.deltaTime
                );
                // 告诉unity， 下一次循环等到下一帧再运行
                yield return null; // 等待下一帧
            }
        }
        // === 移动完成！在这里取消选中状态 ===
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("MotionSpeed", 0f); // 恢复到 Idle 状态
        }
        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
        selectedObject = null;
    }

    private bool IsPointerOverUIObject()
    {
        // 1. 设置 PointerEventData
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        // 2. 获取 Graphic Raycaster (你的 Canvas 上的组件)
        // 假设你的所有 UI 都在一个名为 'MainCanvas' 的 Canvas 下
        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>(); // 或者手动找到它

        if (raycaster == null) return false;

        // 3. 执行射线检测
        raycaster.Raycast(eventData, results);

        // 如果 results 列表不为空，说明击中了 UI 元素
        return results.Count > 0;
    }
}
