using MapTileGridCreator.HexagonalImplementation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHexGridService
{

    /// <summary>
    /// 获取六边形的邻居，呵呵
    /// </summary>
    /// <param name="currentIndex"></param>
    /// <param name="costMap"></param>
    /// <returns></returns>
    List<(float costFactor, Vector3Int index)> GetHexNeighbours(Vector3Int currentIndex, float[,] costMap);

    // 计算六边形距离的启发式
    float CalculateHexDistance(Vector3Int nodeIndex, Vector3Int goalIndex);
}
