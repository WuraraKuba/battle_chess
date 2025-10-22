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
    // ������һ�飬��ͼ��ʵ��
    private readonly HexagonalGrid _hexagonalGridInstance;

    /// <summary>
    /// ʵ�����캯��
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
        // 1. �����������ϵĲ�ֵ
        // q ��Ĳ�ֵ
        int dq = Mathf.Abs(nodeIndex.x - goalIndex.x);
        // r ��Ĳ�ֵ
        int dr = Mathf.Abs(nodeIndex.y - goalIndex.y);
        // s ��Ĳ�ֵ (ע�⣺���ʹ�� Cube ���꣬s = -q - r)
        int ds = Mathf.Abs(nodeIndex.z - goalIndex.z);

        // 2. �����ξ��빫ʽ (Cube Distance)
        // ��������������ֵ�е����ֵ�������������ֵ֮�͵�һ��
        // ���� q+r+s=0�������ʽ����ȷ�ģ������ص�����С���ƶ�����
        return (dq + dr + ds) / 2f;
    }
}
