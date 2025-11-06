using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexAStarPathfinding
{
    // A* 算法依赖的网格服务接口
    private readonly IHexGridService _gridService;

    // 构造函数：注入服务
    public HexAStarPathfinding(IHexGridService gridService)
    {
        _gridService = gridService ?? throw new ArgumentNullException(nameof(gridService));
    }
    // 可达范围
    public List<Vector3Int> MoveRange(Vector3Int startIndex, float movementRange, Dictionary<Vector3Int, float> costMap)
    {
        Dictionary<Vector3Int, float> minCosts = new Dictionary<Vector3Int, float>();
        minCosts[startIndex] = 0;
        Queue<Vector3Int> frontier = new Queue<Vector3Int>();
        
        frontier.Enqueue(startIndex);
        HashSet<Vector3Int> gridInRange = new HashSet<Vector3Int>();
        while (frontier.Count > 0)
        {
            Vector3Int currentIndex = frontier.Dequeue();
            float currentCost = minCosts[currentIndex];
            if (currentCost > movementRange) continue;
            gridInRange.Add(currentIndex); // 因为是HashSet，所以不会重复
            // 探索当前索引的邻居
            var neighboursData = _gridService.GetHexNeighbours(currentIndex, costMap);
            foreach (var (costFactor, neighbourIndex) in neighboursData)
            {
                // 将对应cost比较后加入
                float neighborCost = currentCost + costFactor;
                if (neighborCost > movementRange) continue;
                if (!minCosts.ContainsKey(neighbourIndex) || neighborCost < minCosts[neighbourIndex])
                {
                    minCosts[neighbourIndex] = neighborCost;
                    frontier.Enqueue(neighbourIndex);
                }
            }
        }

        return gridInRange.ToList();

    }

    /// <summary>
    /// A* 寻路主要算法
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <param name="costMap"></param>
    /// <returns></returns>
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, Dictionary<Vector3Int, float> costMap)
    {
        // 存储所有已创建的 AStarNode，方便通过索引快速查找
        Dictionary<Vector3Int, HexCellNode> allNodes = new Dictionary<Vector3Int, HexCellNode>();

        // 开放列表 (Open List): 待探索的节点。我们用 List<T> 模拟 Priority Queue。
        List<HexCellNode> openList = new List<HexCellNode>();

        // 封闭列表 (Closed Set): 已经探索完毕的节点。使用 HashSet 保证查找效率。
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        // ------------------------------------------------------
        // B. 初始化起点
        // ------------------------------------------------------
        HexCellNode startNode = new HexCellNode(start);
        startNode.GScore = 0; // 起点到起点的成本为 0

        // HScore 视为黑箱，通过服务获取
        startNode.HScore = _gridService.CalculateHexDistance(start, goal);
        startNode.CalculatePriority(); // F = G + H

        allNodes[start] = startNode;
        openList.Add(startNode);

        // ------------------------------------------------------
        // C. A* 主循环
        // ------------------------------------------------------

        while (openList.Count > 0)
        {
            // 1. 获取 F-Score 最小的节点 (Current Node)
            // 注意：这是 List<T> 的低效实现，但用于学习原理。
            HexCellNode currentNode = GetNodeWithLowestFScore(openList);
            // 2. 检查是否到达终点
            if (currentNode.GridIndex.Equals(goal))
            {
                Debug.Log("A* 寻路成功！");
                return ReconstructPath(currentNode);
            }

            // 3. 移动到封闭列表
            openList.Remove(currentNode);
            closedSet.Add(currentNode.GridIndex);

            // 4. 探索邻居
            var neighboursData = _gridService.GetHexNeighbours(currentNode.GridIndex, costMap);
            
            foreach (var (costFactor, neighbourIndex) in neighboursData)
            {
                
                // 忽略已探索的节点
                if (closedSet.Contains(neighbourIndex))
                {
                    continue;
                }

                // 计算通过当前节点到达邻居的【新 G-Score】
                float newGScore = currentNode.GScore + 1.0f * costFactor;

                // 确保邻居节点存在于 allNodes 字典中
                if (!allNodes.TryGetValue(neighbourIndex, out HexCellNode neighbourNode))
                {
                    // 第一次发现该节点
                    neighbourNode = new HexCellNode(neighbourIndex);
                    allNodes[neighbourIndex] = neighbourNode;

                    // 计算 HScore (只需计算一次)
                    neighbourNode.HScore = _gridService.CalculateHexDistance(neighbourIndex, goal);
                }

                // 5. 更新优化：如果新路径更优 (GScore 更小)
                if (newGScore < neighbourNode.GScore)
                {
                    neighbourNode.Parent = currentNode; // 设置父节点 (记录路径)
                    neighbourNode.GScore = newGScore;    // 更新 GScore (找到了更好的路径)
                    neighbourNode.CalculatePriority();   // 重新计算 FScore

                    // 如果不在开放列表，则加入
                    if (!openList.Contains(neighbourNode))
                    {
                        
                        openList.Add(neighbourNode);
                    }
                    // 如果已经在开放列表，则 FScore 变化后，下次循环会被优先选中
                }
            }
        }

        // D. 循环结束，仍未找到路径
        Debug.Log("A* 寻路失败：无法找到路径。");
        return new List<Vector3Int>();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="openList"></param>
    /// <returns></returns>
    private HexCellNode GetNodeWithLowestFScore(List<HexCellNode> openList)
    {
        // 如果列表为空，则无节点可返回（理论上主循环会处理这种情况）
        if (openList == null || openList.Count == 0)
        {
            return null;
        }

        // 1. 初始化：假设第一个节点就是 F-Score 最小的
        HexCellNode lowestNode = openList[0];

        // 我们直接计算 F-Score 来比较，而不是依赖 HexCellNode.Priority 属性
        // 这样逻辑更清晰，F = G + H
        float lowestFScore = lowestNode.GScore + lowestNode.HScore;

        // 2. 遍历整个列表，寻找更低的 F-Score
        // 从索引 1 开始，因为索引 0 已经被初始化了
        for (int i = 1; i < openList.Count; i++)
        {
            HexCellNode currentNode = openList[i];
            float currentFScore = currentNode.GScore + currentNode.HScore;

            // 3. 比较 F-Score
            if (currentFScore < lowestFScore)
            {
                // 找到了 F-Score 更低的节点，更新记录
                lowestNode = currentNode;
                lowestFScore = currentFScore;
            }

            // 额外的优化：如果 F-Score 相同，我们倾向于选择 G-Score 较高的（即 H-Score 较低的）
            // 这样可以打破平局，让 A* 更快到达终点，因为 H 较低意味着它可能更接近目标。
            /*
            else if (currentFScore == lowestFScore)
            {
                if (currentNode.GScore > lowestNode.GScore)
                {
                    lowestNode = currentNode;
                    lowestFScore = currentFScore; // 保持 F-Score 不变
                }
            }
            */
        }

        // 4. 返回找到的 F-Score 最小的节点
        return lowestNode;
    }

    // 路径回溯
    private List<Vector3Int> ReconstructPath(HexCellNode endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        HexCellNode current = endNode;

        while (current != null)
        {
            path.Add(current.GridIndex);
            current = current.Parent;
        }

        path.Reverse(); // 反转列表，使路径从起点开始
        return path;
    }
    



}
