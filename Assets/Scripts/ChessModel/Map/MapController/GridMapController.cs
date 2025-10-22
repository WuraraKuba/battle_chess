using MapTileGridCreator.Core;
using MapTileGridCreator.CubeImplementation;
using MapTileGridCreator.HexagonalImplementation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����Grid3D֮��Ĵ����˵�ͼ�����ڴ������Controllerȥ�Ե�ͼ���й���
/// Ŀǰ�뵽�Ĺ�����
/// ��ȡ��ͼ��������
/// �����ת������������ʵ��������ת�ɵ�ͼ����
/// ��ͼЧ��������ҹս����ˮ֮���
/// ������ģ��Ȼ�ȡ��Ŀ�еĵ�ͼ��
/// </summary>
public class GridMapController : MonoBehaviour
{
    public static GridMapController Instance { get; private set; }

    [SerializeField]
    private GameObject GridBaseMap;

    private CubeGrid cubeGrid;
    private HexagonalGrid hexagonalGrid;

    // Ѱ·����
    private IHexGridService gridService;
    private HexAStarPathfinding hexAStarPathfinding;
    private float[,] costMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ȷ�������ᱻ����
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        cubeGrid = GridBaseMap.GetComponent<CubeGrid>();
        hexagonalGrid = GridBaseMap.GetComponent<HexagonalGrid>();
        gridService = new HexGridService(hexagonalGrid);
        hexAStarPathfinding = new HexAStarPathfinding(gridService);
        InitializeCostMap(20, 20);
        
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// ���ڽ�����ʵ���꣬Ȼ����ת�ɵ�ͼ����
    /// </summary>
    /// <param name="position"></param>
    public void Position2CubeIndex(Vector3 position)
    {
        Vector3Int index = cubeGrid.GetIndexByPosition(ref position);
    }
    public Vector3Int Position2HexIndex(Vector3 position)
    {
        Vector3Int index = hexagonalGrid.GetIndexByPosition(ref position);
        return index;

    }
    /// <summary>
    /// �����������λ�ã���ȡ���λ�ö�Ӧ�ķ�����ƽ������λ��
    /// </summary>
    /// <param name="position"></param>
    public Vector3 GetCubeTopCenterPositionByClickPosition(Vector3 position)
    {
        // ���ݵ�������ȡ��Ӧ�������������
        Vector3Int index = cubeGrid.GetIndexByPosition(ref position);
        Vector3 cubePosition = cubeGrid.GetPositionCell(index);
        // ��ȡ����ߴ�
        float size = cubeGrid.SizeCell;

        // ���ݷ���ߴ������������ȡ��ƽ������
        Vector3 offset = Vector3.up * (size / 2f);
        Vector3 topCenterPosition = cubePosition + offset;

        return topCenterPosition;

    }
    /// <summary>
    ///�ڸ�����ͼ����Ѱ·
    ///�ⲿ��Ӧ�þ��Ǽ����·����������ô�߿�Unit�Լ�
    ///�о���ô�߶����Կ�NevMesh�Լ�
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void CubeMapPathFinder(Vector3 start, Vector3 end)
    {
        // ���ȣ������������궼ת�ɸ��������
        Vector3 startGrid = GetCubeTopCenterPositionByClickPosition(start);
        Vector3 endGrid = GetCubeTopCenterPositionByClickPosition(end);
    }
    /// <summary>
    /// ��ȡ��ͼ���鵥λ��������
    /// </summary>
    /// <returns></returns>
    public float GetCubeMapCellSize()
    {
        return cubeGrid.SizeCell;
    }

    // �������������ս�Ե�ͼ
    public float GetHexMapCellSize()
    {
        return hexagonalGrid.SizeCell;
    }
    public Vector3 GetHexTopCenterPositionByClickPosition(Vector3 position)
    {
        // ���ݵ�������ȡ��Ӧ�������������
        Vector3Int index = hexagonalGrid.GetIndexByPosition(ref position);
        Vector3 hexPosition = hexagonalGrid.GetPositionCell(index);
        // ��ȡ����ߴ�
        float size = hexagonalGrid.SizeCell;

        // ���ݷ���ߴ������������ȡ��ƽ������
        Vector3 offset = Vector3.up * (size / 2f);
        Vector3 topCenterPosition = hexPosition + offset;

        return topCenterPosition;

    }

    public void GetNeighbours(Vector3Int index)
    {
        List<(float costFactor, Vector3Int index)> neibors = gridService.GetHexNeighbours(index, costMap);
    }

    /// <summary>
    /// A*Ѱ·�㷨
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public List<Vector3> FindPath(Vector3Int start, Vector3Int goal)
    {
        // ȷ�� costMap �������ǿ��õ�
        if (costMap == null)
        {
            Debug.LogError("�ɱ���ͼδ��ʼ����");
            return new List<Vector3>();
        }
        List<Vector3Int> pathIndexes = hexAStarPathfinding.FindPath(start, goal, costMap);
        // ������ת������
        List<Vector3> pathPositions = new List<Vector3>();
        for (int i = 0; i < pathIndexes.Count; i++)
        {
            Vector3Int pathIndex = pathIndexes[i];
            Vector3 position = hexagonalGrid.GetLocalPositionCell(ref pathIndex);
            position.y += 1.5f;  // ��ʱ��ʩ
            pathPositions.Add(position);
        }

        return pathPositions;
    }

    /// <summary>
    /// �ɱ���ͼ��ʼ��
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private void InitializeCostMap(int width, int height)
    {
        costMap = new float[height, width]; // ϰ���϶�ά������ [��, ��]����Ӧ [Y, X] �� [MapHeight, MapWidth]

        // �������е�Ԫ�����óɱ�Ϊ 1.0
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                costMap[y, x] = 1.0f;
            }
        }
    }



}
