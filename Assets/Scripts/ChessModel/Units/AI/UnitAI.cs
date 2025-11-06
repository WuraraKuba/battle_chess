using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 有限状态机FSM
// 挂载在敌方单位上
public class UnitAI : MonoBehaviour
{
    private Vector3Int ChooseRandomTarget(List<Vector3Int> availableMovePositions)
    {
        if (availableMovePositions.Count == 0)
        {
            // 没有地方可去，返回当前位置

            // return transform.position;
        }

        // 随机选择一个索引
        int randomIndex = Random.Range(0, availableMovePositions.Count);

        // 返回随机选择的目标世界坐标
        return availableMovePositions[randomIndex];
    }

    // 决策逻辑
    public void MakeDecision()
    {

        // 获取敌方单位上的UnitComponent组件
        UnitComponent _unitComponent = GetComponent<UnitComponent>();
        // 1. 获取所有可移动的位置（假设这是你的寻路方法）
        List<Vector3Int> reachableIndexes = GridMapController.Instance.UnitMovementRangeForAI(_unitComponent.UnitLocation, _unitComponent.AP);

        // 2. 随机目标点
        Vector3Int targetPosition = ChooseRandomTarget(reachableIndexes);

        // 3. 计算移动移动路径
        // 暂时随便一下
  
        List<Vector3> path = GridMapController.Instance.FindPath(_unitComponent.UnitLocation, targetPosition);
        UnitMapMovementController.Instance.StartMovement(transform.gameObject, path);


    }
}
