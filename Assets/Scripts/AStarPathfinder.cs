using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AStarPathfinder;


// 网格地图寻路算法
public class AStarPathfinder : MonoBehaviour
{
    // 临时方案，未来应该尝试做一个参数传入
    public GridGenerator gridGenerator;
    private int width;
    private int height;
    private float cellSize = 1f;
    private Vector3 gridOrigin;
    // 用于存方格的数据结构
    public class GridNode
    {
        public Vector3 position;
        public GridNode parent;

        public GridNode(Vector3 pos, GridNode p)
        {
            position = pos;
            parent = p;
        }
    }
    // 用于获取棋盘的属性
    private void Awake()  // 脚本被实例化后第一个被调用的函数（比start还要前，start在update前），可以理解为实例生命周期的第一个函数
    {
        if (gridGenerator == null)
        {
            // 如果没有手动设置，尝试自动查找
            gridGenerator = FindObjectOfType<GridGenerator>();
        }

        if (gridGenerator != null)
        {
            width = gridGenerator.width;
            height = gridGenerator.height;
            cellSize = gridGenerator.cellSize;

        }
        else
        {
            Debug.LogError("GridGenerator not found! Pathfinding will not work.");
        }
    }
    private void Start()
    {
        if (gridGenerator != null)
        {
            gridOrigin = gridGenerator.gridOrigin;
        }
    }
    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        Queue<GridNode> queue = new Queue<GridNode> ();
        HashSet<Vector3> visited = new HashSet<Vector3>();
        // 将起点和终点分别转换为棋盘坐标
        int start_x = Mathf.RoundToInt((start.x - gridOrigin.x) / cellSize);
        int start_z = Mathf.RoundToInt((start.z - gridOrigin.z) / cellSize);
        start = new Vector3(start_x, gridOrigin.y + cellSize, start_z);
        int end_x = Mathf.RoundToInt((end.x - gridOrigin.x) / cellSize);
        int end_z = Mathf.RoundToInt((end.z - gridOrigin.z) / cellSize);
        end = new Vector3(end_x, gridOrigin.y + cellSize, end_z);
        // 将起点加入队列和访问记录
        GridNode startNode = new GridNode(start, null);
        queue.Enqueue(startNode);
        visited.Add(start);

        while (queue.Count > 0) 
        {
            // 遍历吧
            GridNode currentNode = queue.Dequeue();

            // 如果找到终点
            if (currentNode.position == end) 
            {
                return ReconstructPath(currentNode);
            }
            List < Vector3 > NeiborList = GetNeighbors(currentNode.position);
            foreach (Vector3 neighborPos in GetNeighbors(currentNode.position)) 
            { 
                if (!visited.Contains(neighborPos))
                {
                    visited.Add(neighborPos);
                    GridNode neighborNode = new GridNode(neighborPos, currentNode);
                    queue.Enqueue(neighborNode);
                }
            }
        }
        return new List<Vector3>(); 
    }

    private List<Vector3> GetNeighbors(Vector3 pos) 
    {
        List<Vector3> neighbors = new List<Vector3>();
        Vector3[] directions = new Vector3[]
        {
            new Vector3(0, 0, cellSize),
            new Vector3(0, 0, -cellSize),
            new Vector3(cellSize, 0, 0),
            new Vector3(-cellSize, 0, 0),
        };
        foreach (Vector3 direction in directions) 
        {
            Vector3 neighborPos = pos + direction;
            // 获取邻域点的网格坐标
            int x = Mathf.RoundToInt(neighborPos.x  / cellSize);
            int z = Mathf.RoundToInt(neighborPos.z / cellSize);
            if (x >= 0 && z >= 0 && x < width && z < height)
            {
                neighbors.Add(neighborPos);
            }
        }
        return neighbors;

    }

    private List<Vector3> ReconstructPath(GridNode endNode) 
    {
        Debug.Log("重建路径");
        List<Vector3> path = new List<Vector3>();
        GridNode currentNode = endNode;
        while (currentNode != null) 
        {
            float xPos = gridOrigin.x + currentNode.position.x * cellSize;
            float zPos = gridOrigin.z + currentNode.position.z * cellSize;
            path.Add(new Vector3(xPos, gridOrigin.y + cellSize, zPos));
            //path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
}
