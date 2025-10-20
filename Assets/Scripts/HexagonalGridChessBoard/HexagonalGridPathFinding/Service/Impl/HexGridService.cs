using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapTileGridCreator.HexagonalImplementation;
using MapTileGridCreator.Core;
using System.Linq;

public class HexGridService : IHexGridService
{
    // Start is called before the first frame update
    HexagonalGrid hexagonalGrid;
    // 不懂这一块，地图的实例
    private readonly HexagonalGrid _hexagonalGridInstance;

    /// <summary>
    /// 实例构造函数
    /// </summary>
    /// <param name="grid"></param>
    public HexGridService(HexagonalGrid grid)
    {
        _hexagonalGridInstance = grid;
    }

    public List<(float costFactor, Vector3Int index)> GetHexNeighbours(Vector3Int currentIndex, float[,] costMap)
    {
        List<(float costFactor, Vector3Int index)> neighboursData = new List<(float costFactor, Vector3Int index)>();
        List<Cell> neighbourCells = _hexagonalGridInstance.GetNeighboursCell(ref currentIndex);
        List<Vector3Int> neighborIndexes = new List<Vector3Int>();
        foreach (Cell neighbourCell in neighbourCells) 
        {
            neighborIndexes.Add(neighbourCell.GetIndex());
        }
        //List<Vector3Int> neighborIndexes = _hexagonalGridInstance.GetNeighboursIndex(ref currentIndex);
        foreach (Vector3Int neighborIndex in neighborIndexes)
        {
            neighboursData.Add((1.0f, neighborIndex));
        }
        return neighboursData;
    }

    public float CalculateHexDistance(Vector3Int nodeIndex, Vector3Int goalIndex)
    {
        // 1. 计算三个轴上的差值
        // q 轴的差值
        int dq = Mathf.Abs(nodeIndex.x - goalIndex.x);
        // r 轴的差值
        int dr = Mathf.Abs(nodeIndex.y - goalIndex.y);
        // s 轴的差值 (注意：如果使用 Cube 坐标，s = -q - r)
        int ds = Mathf.Abs(nodeIndex.z - goalIndex.z);

        // 2. 六边形距离公式 (Cube Distance)
        // 距离等于三个轴差值中的最大值，或者三个轴差值之和的一半
        // 由于 q+r+s=0，这个公式是正确的，它返回的是最小的移动步数
        return (dq + dr + ds) / 2f;
    }
}
