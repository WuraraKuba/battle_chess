using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// 用于鼠标相关的操作，包括选择，之类的
public class MouseControl : MonoBehaviour
{
    public AStarPathfinder aStarPathfinder;
    public Material highlightMaterial;  // 拖入你创建的高亮材质
    private GameObject selectedObject;  // 存储当前选中的物体
    private Material originalMatrial;   // 存储原始材质

    private GameObject currentHoverObject; // 当前鼠标悬停的物体
    private Vector3 originalPosition;      // 存储悬停物体的原始位置

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
        if (Physics.Raycast(ray, out hit))
        {
            // 检测鼠标左键是否被按下
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.gameObject.CompareTag("Player"))   // Tag用于处理分类情况
                {
                    if (selectedObject != null)
                    {  // 如果之前选过，那么将原来选过的材质替换为原始材质后再进行下一步操作
                        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
                    }
                    selectedObject = hit.transform.gameObject;
                    //originalMatrial = selectedObject.GetComponent<Material>();
                    originalMatrial = selectedObject.GetComponent<MeshRenderer>().material;
                    selectedObject.GetComponent<MeshRenderer>().material = highlightMaterial;
                }
                else  // 选中的不是新的player
                {
                    if (selectedObject != null)   // selectedObject 只可能是选中的角色
                    {
                        if (hit.transform.gameObject.CompareTag("GridCell"))  // 再次点击的是一个棋盘格
                        {
                            // 获取移动物体起始坐标
                            Vector3 startPosition = selectedObject.transform.position;
                            // 获取这个棋盘格的坐标
                            Vector3 targetPosition = hit.transform.position;
                            // 移动选择物体的坐标
                            //selectedObject.transform.position = targetPosition;
                            // 调用寻路算法
                            List<Vector3> path = aStarPathfinder.FindPath(startPosition, targetPosition);
                            if (path != null)
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
                            }
                           
                        }
                    }
                    
                }
            }
            // 鼠标左键没有被按下的情况
            else
            {
                if (hit.transform.gameObject.CompareTag("GridCell"))
                {
                    if (currentHoverObject != hit.transform.gameObject)
                    {
                        if (currentHoverObject != null)
                        {
                            currentHoverObject.transform.position = originalPosition;
                        }
                            currentHoverObject = hit.transform.gameObject;
                            originalPosition = hit.transform.position;
                            currentHoverObject.transform.position += new Vector3(0, 0.5f, 0);
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

    // 移动协程, 所谓协程
    IEnumerator MoveAlongPath(List<Vector3> path)
    {
        // 确保路径不为空
        if (path == null || path.Count == 0)
        {
            yield break; // 退出协程
        }

        float moveSpeed = 5f; // 移动速度

        // 遍历路径中的每个节点
        foreach (Vector3 targetNode in path)
        {
            // 持续移动，直到到达目标节点
            while (selectedObject.transform.position != targetNode)
            {
                
                // 朝着目标点移动
                selectedObject.transform.position = Vector3.MoveTowards(
                    selectedObject.transform.position,
                    targetNode,
                    moveSpeed * Time.deltaTime
                );

                // 告诉unity， 下一次循环等到下一帧再运行
                yield return null; // 等待下一帧
            }
        }
        // === 移动完成！在这里取消选中状态 ===
        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
        selectedObject = null;
    }
}
