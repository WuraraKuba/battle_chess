using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

// 用于鼠标相关的操作，包括选择，之类的
public class MouseControl : MonoBehaviour
{
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
        Debug.Log("RIght" + RaycastIgnoreLayerMask);
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        // 无论鼠标点击与否，都创建一个从摄影机位置向鼠标位置发射的射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 用于存储射线检测的结果
        RaycastHit hit;
        // 检测射线是否接触到物体
        if (Physics.Raycast(ray, out hit, 1000f, RaycastIgnoreLayerMask))  //, 100f, ~0, QueryTriggerInteraction.Ignore
        {
            
            // 检测鼠标左键是否被按下
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(hit.transform.gameObject.layer);
                if (hit.transform.gameObject.layer == unitLayerIndex)
                {
                    // **情况 A: 点击了角色 (选择)**
                    SelectUnit(hit.transform.gameObject.GetComponent<UnitController>());
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

                /*                if (hit.transform.gameObject.CompareTag("Player"))   // Tag用于处理分类情况
                                {
                                    if (selectedObject != null)
                                    {  // 如果之前选过，那么将原来选过的材质替换为原始材质后再进行下一步操作
                                        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
                                    }
                                    selectedObject = hit.transform.gameObject;
                                    // 获取角色属性
                                    Unit unitProperties = selectedObject.GetComponent<Unit>();
                                    // 角色移动属性
                                    int movementRange = unitProperties.movementRange;
                                    Vector3 startPosition = selectedObject.transform.position;
                                    // 显示移动范围
                                   *//* List<Vector3> reachablePositions = aStarPathfinder.FindReachableNodes(startPosition, movementRange);
                                    foreach (Vector3 cellPosition in gridGenerator.gridCells.Keys)
                                    {
                                        //Debug.Log(cellPosition);
                                    }*//*
                                    // 对处于这些列表中的坐标，修改其材质
                                    *//*foreach (Vector3 position in reachablePositions)
                                    {
                                        if (gridGenerator.gridCells.ContainsKey(position))
                                        {
                                            gridGenerator.gridCells[position].transform.position += new Vector3(0, 1, 0);

                                        }
                                    }*//*
                                    //originalMatrial = selectedObject.GetComponent<Material>();
                                    originalMatrial = selectedObject.GetComponent<MeshRenderer>().material;
                                    selectedObject.GetComponent<MeshRenderer>().material = highlightMaterial;
                                }
                                else  // 选中的不是新的player
                                {
                                    if (selectedObject != null)   // selectedObject 只可能是选中的角色
                                    {

                                        if (hit.transform.gameObject.layer == environmentLayerIndex)  
                                        {
                                            Debug.Log("new");
                                            // 获取移动物体起始坐标
                                            Vector3 startPosition = selectedObject.transform.position;
                                            // 获取这个棋盘格的坐标
                                            Vector3 targetPosition = hit.transform.position;
                                            // 移动选择物体的坐标
                                            //selectedObject.transform.position = targetPosition;
                                            // 调用寻路算法
                                            unitMovement.MoveTo(targetPosition);
                                            //List<Vector3> path = aStarPathfinder.FindPath(startPosition, targetPosition);
                                        *//*    if (path != null)
                                            {
                                                // StartCoroutine : 启动协程
                                                StartCoroutine(MoveAlongPath(path));
                                                //foreach (Vector3 node in path)
                                                //{
                                                    //Debug.Log(node);
                                                    //selectedObject.transform.position = node;
                                                //}
                                                //selectedObject.transform.position = path[path.Count - 1];
                                            }
                                            else
                                            {
                                                selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
                                                selectedObject = null;
                                            }*//*

                                        }
                                    }

                                }*/
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
        Debug.Log(selectedUnit.name + " 已被选中。");
    }

    void TryMoveSelectedUnit(Vector3 hitPoint)
    {
        Debug.Log("run");
        if (selectedUnit == null) return;
        Debug.Log("run2");
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
}
