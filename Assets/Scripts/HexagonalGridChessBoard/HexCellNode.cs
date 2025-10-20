using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCellNode : FastPriorityQueueNode
{
    // A* 核心数据
    /// <summary>
    /// G Score: 从起点到该节点的实际移动成本
    /// </summary>
    public float GScore { get; set; }

    /// <summary>
    /// H Score: 从该节点到终点的预估启发式成本（Hex Distance）
    /// </summary>
    public float HScore { get; set; }

    /// <summary>
    /// 父节点，用于路径重建
    /// </summary>
    public HexCellNode Parent { get; set; }

    // 地图引用数据

    /// <summary>
    /// 该节点在 GridMapCreator 中的索引 (q, r, s)
    /// </summary>
    public Vector3Int GridIndex { get; set; }

    /// <summary>
    /// 该地块的地形类型（Grass, Water, etc.）
    /// </summary>
    /*public TerrainType Type { get; set; } // 假设你有一个 TerrainType 枚举*/

    // 构造函数
    public HexCellNode(Vector3Int index)
    {
        this.GridIndex = index;
        this.GScore = float.MaxValue;
        this.HScore = 0;
        // GScore, HScore, Parent 在 A* 寻路开始时初始化
    }

    /// <summary>
    /// A* F Score = G Score + H Score
    /// 在 A* 循环中计算并设置给 Priority 属性
    /// </summary>
    public void CalculatePriority()
    {
        this.Priority = this.GScore + this.HScore;
    }
}
