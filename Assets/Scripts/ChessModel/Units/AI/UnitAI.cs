using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 有限状态机FSM
public class UnitAI : MonoBehaviour
{
    private Vector3 ChooseRandomTarget(List<Vector3> availableMovePositions)
    {
        if (availableMovePositions.Count == 0)
        {
            // 没有地方可去，返回当前位置
            return transform.position;
        }

        // 随机选择一个索引
        int randomIndex = Random.Range(0, availableMovePositions.Count);

        // 返回随机选择的目标世界坐标
        return availableMovePositions[randomIndex];
    }

    // 决策逻辑
/*    public void MakeDecision()
    {
        // 1. 获取所有可移动的位置（假设这是你的寻路方法）
        List<Vector3> reachablePositions = PathfindingManager.CalculateReachableTiles(currentPosition, movementRange);

        // 2. 随机目标点
        Vector3 targetPosition = ChooseRandomTarget(reachablePositions);

        // 3. 调用你的移动逻辑
        StartMoveSequence(targetPosition);

        // 4. 切换状态到 Moving
        SetState(AIState.Moving);
    }*/
}
