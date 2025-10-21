using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexAStarPathfinding
{
    // A* �㷨�������������ӿ�
    private readonly IHexGridService _gridService;

    // ���캯����ע�����
    public HexAStarPathfinding(IHexGridService gridService)
    {
        _gridService = gridService ?? throw new ArgumentNullException(nameof(gridService));
    }

    /// <summary>
    /// A* Ѱ·��Ҫ�㷨
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <param name="costMap"></param>
    /// <returns></returns>
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, float[,] costMap)
    {
        // �洢�����Ѵ����� AStarNode������ͨ���������ٲ���
        Dictionary<Vector3Int, HexCellNode> allNodes = new Dictionary<Vector3Int, HexCellNode>();

        // �����б� (Open List): ��̽���Ľڵ㡣������ List<T> ģ�� Priority Queue��
        List<HexCellNode> openList = new List<HexCellNode>();

        // ����б� (Closed Set): �Ѿ�̽����ϵĽڵ㡣ʹ�� HashSet ��֤����Ч�ʡ�
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        // ------------------------------------------------------
        // B. ��ʼ�����
        // ------------------------------------------------------
        Debug.Log("��� " + start);
        Debug.Log("�յ� " +  goal);
        HexCellNode startNode = new HexCellNode(start);
        startNode.GScore = 0; // ��㵽���ĳɱ�Ϊ 0

        // HScore ��Ϊ���䣬ͨ�������ȡ
        startNode.HScore = _gridService.CalculateHexDistance(start, goal);
        startNode.CalculatePriority(); // F = G + H

        allNodes[start] = startNode;
        openList.Add(startNode);

        // ------------------------------------------------------
        // C. A* ��ѭ��
        // ------------------------------------------------------

        while (openList.Count > 0)
        {
            // 1. ��ȡ F-Score ��С�Ľڵ� (Current Node)
            // ע�⣺���� List<T> �ĵ�Чʵ�֣�������ѧϰԭ��
            HexCellNode currentNode = GetNodeWithLowestFScore(openList);
            // 2. ����Ƿ񵽴��յ�
            if (currentNode.GridIndex.Equals(goal))
            {
                Debug.Log("A* Ѱ·�ɹ���");
                return ReconstructPath(currentNode);
            }

            // 3. �ƶ�������б�
            openList.Remove(currentNode);
            closedSet.Add(currentNode.GridIndex);

            // 4. ̽���ھ�
            var neighboursData = _gridService.GetHexNeighbours(currentNode.GridIndex, costMap);
            
            foreach (var (costFactor, neighbourIndex) in neighboursData)
            {
                
                // ������̽���Ľڵ�
                if (closedSet.Contains(neighbourIndex))
                {
                    continue;
                }

                // ����ͨ����ǰ�ڵ㵽���ھӵġ��� G-Score��
                // 1.0f �ǻ����ƶ��ɱ���costFactor �ǵ��γɱ����ӣ����磺������ 3.0��
                float newGScore = currentNode.GScore + 1.0f * costFactor;

                // ȷ���ھӽڵ������ allNodes �ֵ���
                if (!allNodes.TryGetValue(neighbourIndex, out HexCellNode neighbourNode))
                {
                    // ��һ�η��ָýڵ�
                    neighbourNode = new HexCellNode(neighbourIndex);
                    allNodes[neighbourIndex] = neighbourNode;

                    // ���� HScore (ֻ�����һ��)
                    neighbourNode.HScore = _gridService.CalculateHexDistance(neighbourIndex, goal);
                }

                // 5. �����Ż��������·������ (GScore ��С)
                if (newGScore < neighbourNode.GScore)
                {
                    neighbourNode.Parent = currentNode; // ���ø��ڵ� (��¼·��)
                    neighbourNode.GScore = newGScore;    // ���� GScore (�ҵ��˸��õ�·��)
                    neighbourNode.CalculatePriority();   // ���¼��� FScore

                    // ������ڿ����б������
                    if (!openList.Contains(neighbourNode))
                    {
                        
                        openList.Add(neighbourNode);
                    }
                    // ����Ѿ��ڿ����б��� FScore �仯���´�ѭ���ᱻ����ѡ��
                }
            }
        }

        // D. ѭ����������δ�ҵ�·��
        Debug.Log("A* Ѱ·ʧ�ܣ��޷��ҵ�·����");
        return new List<Vector3Int>();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="openList"></param>
    /// <returns></returns>
    private HexCellNode GetNodeWithLowestFScore(List<HexCellNode> openList)
    {
        // ����б�Ϊ�գ����޽ڵ�ɷ��أ���������ѭ���ᴦ�����������
        if (openList == null || openList.Count == 0)
        {
            return null;
        }

        // 1. ��ʼ���������һ���ڵ���� F-Score ��С��
        HexCellNode lowestNode = openList[0];

        // ����ֱ�Ӽ��� F-Score ���Ƚϣ����������� HexCellNode.Priority ����
        // �����߼���������F = G + H
        float lowestFScore = lowestNode.GScore + lowestNode.HScore;

        // 2. ���������б�Ѱ�Ҹ��͵� F-Score
        // ������ 1 ��ʼ����Ϊ���� 0 �Ѿ�����ʼ����
        for (int i = 1; i < openList.Count; i++)
        {
            HexCellNode currentNode = openList[i];
            float currentFScore = currentNode.GScore + currentNode.HScore;

            // 3. �Ƚ� F-Score
            if (currentFScore < lowestFScore)
            {
                // �ҵ��� F-Score ���͵Ľڵ㣬���¼�¼
                lowestNode = currentNode;
                lowestFScore = currentFScore;
            }

            // ������Ż������ F-Score ��ͬ������������ѡ�� G-Score �ϸߵģ��� H-Score �ϵ͵ģ�
            // �������Դ���ƽ�֣��� A* ���쵽���յ㣬��Ϊ H �ϵ���ζ�������ܸ��ӽ�Ŀ�ꡣ
            /*
            else if (currentFScore == lowestFScore)
            {
                if (currentNode.GScore > lowestNode.GScore)
                {
                    lowestNode = currentNode;
                    lowestFScore = currentFScore; // ���� F-Score ����
                }
            }
            */
        }

        // 4. �����ҵ��� F-Score ��С�Ľڵ�
        return lowestNode;
    }

    // ·������
    private List<Vector3Int> ReconstructPath(HexCellNode endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        HexCellNode current = endNode;

        while (current != null)
        {
            path.Add(current.GridIndex);
            current = current.Parent;
        }

        path.Reverse(); // ��ת�б�ʹ·������㿪ʼ
        return path;
    }


}
