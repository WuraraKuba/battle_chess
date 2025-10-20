using MapTileGridCreator.HexagonalImplementation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHexGridService
{

    /// <summary>
    /// ��ȡ�����ε��ھӣ��Ǻ�
    /// </summary>
    /// <param name="currentIndex"></param>
    /// <param name="costMap"></param>
    /// <returns></returns>
    List<(float costFactor, Vector3Int index)> GetHexNeighbours(Vector3Int currentIndex, float[,] costMap);

    // ���������ξ��������ʽ
    float CalculateHexDistance(Vector3Int nodeIndex, Vector3Int goalIndex);
}
