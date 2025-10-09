using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridManager : MonoBehaviour
{
    // === �������� - �� Inspector ������ ===
    public float gridSize = 1f;           // ÿ�����ӵı߳� (���� 1�� x 1��)
    public Vector3 gridOrigin = Vector3.zero; // ���̵���ʼ��������
    public int gridWidth = 200;            // ���� X ��ĸ�����
    public int gridLength = 200;           // ���� Z ��ĸ�����
    public float surfaceCheckRadius = 0.5f; // NavMesh �����뾶
    public GameObject highlightPrefab;       // �� Inspector ������ĸ���Ԥ�Ƽ�
    private GameObject currentHighlight;

    // �洢������Ч�ĸ������ĵ����������
    public List<Vector3> validGridCenters = new List<Vector3>();

    // �洢ת����Ϣ
    public struct GridHitInfo
    {
        public bool isValid;          // �Ƿ��� NavMesh �ϵ���Ч��
        public Vector2Int gridPos;    // �������� (col, row)
        public Vector3 worldCenter;   // �������ĵľ�ȷ�������� (����ʵY��)
    }

    void Start()
    {
        GenerateValidGrid();
    }

    // �����������ɲ���֤���и��ӵĿ�����
    void GenerateValidGrid()
    {
        validGridCenters.Clear();

        // �������п��ܵ��������� (�к���)
        for (int row = -gridLength; row < gridLength; row++)
        {
            for (int col = -gridWidth; col < gridWidth; col++)
            {
                // 1. ������ӵ��������ĵ� (�ڿ��У����ڹ���Ͷ��)
                // Y ����ø�һЩ����ȷ�� Raycast �ܻ����ݶ��͵���
                Vector3 theoreticalCenter = new Vector3(
                    gridOrigin.x + col * gridSize + (gridSize / 2f),
                    10f, // ��Ϊ 10 �׸�
                    gridOrigin.z + row * gridSize + (gridSize / 2f)
                );

                // 2. ʹ�� Raycast �ҵ�ʵ�ʵĵ���/�ݶ� Y ����
                if (Physics.Raycast(theoreticalCenter, Vector3.down, out RaycastHit hit, 20f))
                {
                    Vector3 actualSurfacePoint = hit.point;

                    // 3. ʹ�� NavMesh ��֤������Ƿ������
                    NavMeshHit navHit;
                    // �� Raycast �õ�����ʵ����㿪ʼ����
                    if (NavMesh.SamplePosition(actualSurfacePoint, out navHit, surfaceCheckRadius, NavMesh.AllAreas))
                    {
                        // ��֤�ɹ��������������ɫ�� NavMesh ��
                        validGridCenters.Add(navHit.position);
                    }
                }
            }
        }

    }

    // 1. �������� -> ��������
    public Vector3 WorldToGridCoordinates(Vector3 worldPos)
    {
        // ʹ����֮ǰ���۵� floor ��ʽ����ת��
        int col = Mathf.FloorToInt((worldPos.x - gridOrigin.x) / gridSize);
        int row = Mathf.FloorToInt((worldPos.z - gridOrigin.z) / gridSize);

        // ȷ�����ص�ֵ�ڶ���ķ�Χ��
        col = Mathf.Clamp(col, -gridWidth, gridWidth - 1);
        row = Mathf.Clamp(row, -gridLength, gridLength - 1);

        return new Vector3(col, worldPos.y, row);
    }

    // ��֤ĳ�����������ĵ��Ƿ����
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
    // 2. �������� -> ����������������
    public Vector3 GridToWorldCoordinates(Vector2Int gridPos)
    {
        // ������ӵ����ĵ�
        float x = gridOrigin.x + gridPos.x * gridSize + (gridSize / 2f);
        float z = gridOrigin.z + gridPos.y * gridSize + (gridSize / 2f);

        // �ؼ������� Y �ᱻ NavMesh ��֤�������꣡
        // ���� validGridCenters �б����ƥ��� Y ���꣬������Ҫ�Ż����ҷ�ʽ��
        // ��Ϊ�˼򻯣������ȼ�������֪�����ĸ߶ȣ������� validGridCenters �д洢 GridPos��

        // ���õķ����ǣ������ A*Pathfinder ֪�� validGridCenters �б�
        // ����ļ򻯣�
        return new Vector3(x, 0f /* ��ʱ Y��������Ҫ��ȷ Y */, z);
    }

    // �Ե�ǰ����Ϊ���ģ����Ʒ���
    public void GenerateGrid(Vector3 centerPos)
    {
        // 1. ��ʼ�������ø�������

        // �����������Ĵ�С (��С�� gridSize)
        float scale = gridSize * 0.9f;

        if (currentHighlight == null)
        {
            // ��һ����ͣʱ��ʵ������������
            currentHighlight = Instantiate(highlightPrefab, transform);

            // ���������Ĵ�С
            currentHighlight.transform.localScale = new Vector3(scale, 0.01f, scale);
        }

        // 2. ���÷������ʾλ�� (��ȫ����ȫ������ centerPos)

        // ��������������ĵ��ƶ��� centerPos
        // ̧�� 0.05m ��ֹ����� Z-Fighting (��˸)
        currentHighlight.transform.position = centerPos + Vector3.up * 0.05f;

    }

}
