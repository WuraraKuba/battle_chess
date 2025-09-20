using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridManager : MonoBehaviour
{
    // === 公共变量 - 在 Inspector 中设置 ===
    public float gridSize = 1f;           // 每个格子的边长 (例如 1米 x 1米)
    public Vector3 gridOrigin = Vector3.zero; // 棋盘的起始世界坐标
    public int gridWidth = 200;            // 棋盘 X 轴的格子数
    public int gridLength = 200;           // 棋盘 Z 轴的格子数
    public float surfaceCheckRadius = 0.5f; // NavMesh 采样半径
    public GameObject highlightPrefab;       // 在 Inspector 中拖入的高亮预制件
    private GameObject currentHighlight;

    // 存储所有有效的格子中心点的世界坐标
    public List<Vector3> validGridCenters = new List<Vector3>();

    // 存储转换信息
    public struct GridHitInfo
    {
        public bool isValid;          // 是否是 NavMesh 上的有效点
        public Vector2Int gridPos;    // 棋盘坐标 (col, row)
        public Vector3 worldCenter;   // 格子中心的精确世界坐标 (带真实Y轴)
    }

    void Start()
    {
        GenerateValidGrid();
    }

    // 主函数：生成并验证所有格子的可行性
    void GenerateValidGrid()
    {
        validGridCenters.Clear();

        // 遍历所有可能的网格坐标 (行和列)
        for (int row = -gridLength; row < gridLength; row++)
        {
            for (int col = -gridWidth; col < gridWidth; col++)
            {
                // 1. 计算格子的理论中心点 (在空中，用于光线投射)
                // Y 轴设得高一些，以确保 Raycast 能击中屋顶和地面
                Vector3 theoreticalCenter = new Vector3(
                    gridOrigin.x + col * gridSize + (gridSize / 2f),
                    10f, // 设为 10 米高
                    gridOrigin.z + row * gridSize + (gridSize / 2f)
                );

                // 2. 使用 Raycast 找到实际的地面/屋顶 Y 坐标
                if (Physics.Raycast(theoreticalCenter, Vector3.down, out RaycastHit hit, 20f))
                {
                    Vector3 actualSurfacePoint = hit.point;

                    // 3. 使用 NavMesh 验证这个点是否可行走
                    NavMeshHit navHit;
                    // 从 Raycast 得到的真实表面点开始采样
                    if (NavMesh.SamplePosition(actualSurfacePoint, out navHit, surfaceCheckRadius, NavMesh.AllAreas))
                    {
                        // 验证成功！这个点落在蓝色的 NavMesh 上
                        validGridCenters.Add(navHit.position);
                    }
                }
            }
        }

    }

    // 1. 世界坐标 -> 棋盘坐标
    public Vector3 WorldToGridCoordinates(Vector3 worldPos)
    {
        // 使用你之前讨论的 floor 公式进行转换
        int col = Mathf.FloorToInt((worldPos.x - gridOrigin.x) / gridSize);
        int row = Mathf.FloorToInt((worldPos.z - gridOrigin.z) / gridSize);

        // 确保返回的值在定义的范围内
        col = Mathf.Clamp(col, -gridWidth, gridWidth - 1);
        row = Mathf.Clamp(row, -gridLength, gridLength - 1);

        return new Vector3(col, worldPos.y, row);
    }

    // 验证某棋盘网格中心点是否可走
    public bool ValidGridCenter(Vector3 gridPos)
    {
        Vector3 theoreticalCenter = new Vector3(
            gridOrigin.x + gridPos.x * gridSize + (gridSize / 2f),
            gridPos.y,
            gridOrigin.z + gridPos.z * gridSize + (gridSize / 2f));

        NavMeshHit navhit;
        if (NavMesh.SamplePosition(theoreticalCenter, out navhit, 1.0f, NavMesh.AllAreas)) 
        {
            return true;
        }
        else
        {
            return false;
        }
        
            
    }
    // 2. 棋盘坐标 -> 格子中心世界坐标
    public Vector3 GridToWorldCoordinates(Vector2Int gridPos)
    {
        // 计算格子的中心点
        float x = gridOrigin.x + gridPos.x * gridSize + (gridSize / 2f);
        float z = gridOrigin.z + gridPos.y * gridSize + (gridSize / 2f);

        // 关键：返回 Y 轴被 NavMesh 验证过的坐标！
        // 遍历 validGridCenters 列表查找匹配的 Y 坐标，这里需要优化查找方式，
        // 但为了简化，我们先假设我们知道它的高度（或者在 validGridCenters 中存储 GridPos）

        // 更好的方法是：让你的 A*Pathfinder 知道 validGridCenters 列表
        // 这里的简化：
        return new Vector3(x, 0f /* 临时 Y，最终需要精确 Y */, z);
    }

    // 以当前坐标为中心，绘制方格
    public void GenerateGrid(Vector3 centerPos)
    {
        // 1. 初始化或重用高亮对象

        // 计算高亮对象的大小 (略小于 gridSize)
        float scale = gridSize * 0.9f;

        if (currentHighlight == null)
        {
            // 第一次悬停时：实例化高亮对象
            currentHighlight = Instantiate(highlightPrefab, transform);

            // 调整高亮的大小
            currentHighlight.transform.localScale = new Vector3(scale, 0.01f, scale);
        }

        // 2. 设置方格的显示位置 (完全基于全局坐标 centerPos)

        // 将高亮对象的中心点移动到 centerPos
        // 抬高 0.05m 防止与地面 Z-Fighting (闪烁)
        currentHighlight.transform.position = centerPos + Vector3.up * 0.05f;

    }

}
