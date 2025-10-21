using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCellNode : FastPriorityQueueNode
{
    // A* ��������
    /// <summary>
    /// G Score: ����㵽�ýڵ��ʵ���ƶ��ɱ�
    /// </summary>
    public float GScore { get; set; }

    /// <summary>
    /// H Score: �Ӹýڵ㵽�յ��Ԥ������ʽ�ɱ���Hex Distance��
    /// </summary>
    public float HScore { get; set; }

    /// <summary>
    /// ���ڵ㣬����·���ؽ�
    /// </summary>
    public HexCellNode Parent { get; set; }

    // ��ͼ��������

    /// <summary>
    /// �ýڵ��� GridMapCreator �е����� (q, r, s)
    /// </summary>
    public Vector3Int GridIndex { get; set; }

    /// <summary>
    /// �õؿ�ĵ������ͣ�Grass, Water, etc.��
    /// </summary>
    /*public TerrainType Type { get; set; } // ��������һ�� TerrainType ö��*/

    // ���캯��
    public HexCellNode(Vector3Int index)
    {
        this.GridIndex = index;
        this.GScore = float.MaxValue;
        this.HScore = 0;
        // GScore, HScore, Parent �� A* Ѱ·��ʼʱ��ʼ��
    }

    /// <summary>
    /// A* F Score = G Score + H Score
    /// �� A* ѭ���м��㲢���ø� Priority ����
    /// </summary>
    public void CalculatePriority()
    {
        this.Priority = this.GScore + this.HScore;
    }
}
