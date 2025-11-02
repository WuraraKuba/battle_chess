using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitMapMovementController : MonoBehaviour
{
    public static UnitMapMovementController Instance {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 确保它不会被销毁
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "BoardScene")
        {
            return; // 如果不在地图场景，直接退出 Update
        }
    }
    // 公共启动方法：方便外部调用
    public void StartMovement(GameObject selectedObject, List<Vector3> path)
    {
        // 关键：在一个 MonoBehaviour 实例上调用 StartCoroutine
        StartCoroutine(MoveAlongPath(selectedObject, path));
    }

    public IEnumerator MoveAlongPath(GameObject selectedObject, List<Vector3> path)
    {
        CapsuleCollider collider = selectedObject.GetComponent<CapsuleCollider>();
        float yOffset = collider.height / 2;
        // 确保路径不为空
        if (path == null || path.Count == 0)
        {
            yield break; // 退出协程
        }
/*        // 确保选择物体的位置与路径起点相同
        if (selectedObject.transform.position.x != path[0].x || selectedObject.transform.position.z != path[0].z)
        {
            Debug.Log("起始点不对？");
            yield break;
        }*/

        float moveSpeed = 2f; // 移动速度
        /*        // 获取Animator组件
                Animator animator = selectedObject.GetComponent<Animator>();*/
        /*        // 移动开始
                if (animator != null)
                {
                    animator.SetFloat("Speed", moveSpeed);
                    animator.SetFloat("MotionSpeed", moveSpeed);
                }*/
        // 遍历路径中的每个节点
        foreach (Vector3 targetNode in path)
        {
            
            // 让角色朝向目标点
            selectedObject.transform.LookAt(targetNode);
            // 持续移动，直到到达目标节点
            while (Vector3.Distance(selectedObject.transform.position, targetNode) > 0.01f)
            {
                Vector3 oldPos = selectedObject.transform.position;
                // 朝着目标点移动
                selectedObject.transform.position = Vector3.MoveTowards(
                    oldPos,
                    targetNode,
                    moveSpeed * Time.deltaTime
                );
                UnitComponent unitComponent = selectedObject.GetComponent<UnitComponent>();
                Vector3 currenLocation = targetNode;
                currenLocation.y -= yOffset;
                unitComponent.ChangeLocation(currenLocation);
                // 告诉unity， 下一次循环等到下一帧再运行
                yield return null; // 等待下一帧
            }
        }
/*        // === 移动完成！在这里取消选中状态 ===
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("MotionSpeed", 0f); // 恢复到 Idle 状态
        }
        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
        selectedObject = null;*/
    }


}
