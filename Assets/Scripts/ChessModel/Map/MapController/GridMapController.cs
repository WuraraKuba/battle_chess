using MapTileGridCreator.Core;
using MapTileGridCreator.CubeImplementation;
using MapTileGridCreator.HexagonalImplementation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 借助Grid3D之类的创建了地图，现在创建这个Controller去对地图进行管理
/// 目前想到的功能有
/// 获取地图各种属性
/// 坐标的转换——根据真实世界坐标转成地图坐标
/// 地图效果，比如夜战，降水之类的
/// 最基础的，先获取项目中的地图吧
/// </summary>
public class GridMapController : MonoBehaviour
{
    public static GridMapController Instance { get; private set; }

    [SerializeField]
    private GameObject GridBaseMap;
    private List<Vector3Int> hexGridIndexes = new List<Vector3Int>();
    private CubeGrid cubeGrid;
    private HexagonalGrid hexagonalGrid;

    // 寻路部分
    private IHexGridService gridService;
    private HexAStarPathfinding hexAStarPathfinding;
    private Dictionary<Vector3Int, float> costMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 确保它不会被销毁
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        cubeGrid = GridBaseMap.GetComponent<CubeGrid>();
        hexagonalGrid = GridBaseMap.GetComponent<HexagonalGrid>();
        // 遍历其中一个的子对象, 暂时只有hex的
        foreach (Transform child in hexagonalGrid.transform)
        {
            if (child != null)
            {
                Cell childCell = child.gameObject.GetComponent<Cell>();
                hexGridIndexes.Add(childCell.GetIndex());
            }
        }
        gridService = new HexGridService(hexagonalGrid);
        hexAStarPathfinding = new HexAStarPathfinding(gridService);
        InitializeCostMap();
        
    }
    
    public List<Vector3Int> MapGridIndexes()
    {
        return hexGridIndexes;
    }
    /// <summary>
    /// 用于接收现实坐标，然后将其转成地图索引
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
    /// 根据鼠标点击的位置，获取这个位置对应的方块上平面中心位置
    /// </summary>
    /// <param name="position"></param>
    public Vector3 GetCubeTopCenterPositionByClickPosition(Vector3 position)
    {
        // 根据点击坐标获取对应方块的中心坐标
        Vector3Int index = cubeGrid.GetIndexByPosition(ref position);
        Vector3 cubePosition = cubeGrid.GetPositionCell(index);
        // 获取方块尺寸
        float size = cubeGrid.SizeCell;

        // 根据方块尺寸与中心坐标获取上平面坐标
        Vector3 offset = Vector3.up * (size / 2f);
        Vector3 topCenterPosition = cubePosition + offset;

        return topCenterPosition;

    }
    /// <summary>
    ///在格网地图进行寻路
    ///这部分应该就是计算出路径，具体怎么走看Unit自己
    ///感觉怎么走都可以看NevMesh自己
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void CubeMapPathFinder(Vector3 start, Vector3 end)
    {
        // 首先，将这两个坐标都转成格网坐标吧
        Vector3 startGrid = GetCubeTopCenterPositionByClickPosition(start);
        Vector3 endGrid = GetCubeTopCenterPositionByClickPosition(end);
    }
    /// <summary>
    /// 获取地图方块单位基本属性
    /// </summary>
    /// <returns></returns>
    public float GetCubeMapCellSize()
    {
        return cubeGrid.SizeCell;
    }

    // 以下针对六边形战略地图
    public float GetHexMapCellSize()
    {
        return hexagonalGrid.SizeCell;
    }
    public Vector3 GetHexTopCenterPositionByClickPosition(Vector3 position)
    {
        // 根据点击坐标获取对应方块的中心坐标
        Vector3Int index = hexagonalGrid.GetIndexByPosition(ref position);
        Vector3 hexPosition = hexagonalGrid.GetPositionCell(index);
        // 获取方块尺寸
        float size = hexagonalGrid.SizeCell;

        // 根据方块尺寸与中心坐标获取上平面坐标
        Vector3 offset = Vector3.up * (size / 2f);
        Vector3 topCenterPosition = hexPosition + offset;

        return topCenterPosition;

    }

    public void GetNeighbours(Vector3Int index)
    {
        List<(float costFactor, Vector3Int index)> neibors = gridService.GetHexNeighbours(index, costMap);
    }

    /// <summary>
    /// A*寻路算法
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public List<Vector3> FindPath(Vector3 start, Vector3 goal)
    {
        // 确保 costMap 在这里是可用的
        if (costMap == null)
        {
            Debug.LogError("成本地图未初始化！");
            return new List<Vector3>();
        }
        Vector3Int startIndex = hexagonalGrid.GetIndexByPosition(ref start);
        Vector3Int goalIndex = hexagonalGrid.GetIndexByPosition(ref goal);
        List<Vector3Int> pathIndexes = hexAStarPathfinding.FindPath(startIndex, goalIndex, costMap);
        // 将索引转成坐标
        List<Vector3> pathPositions = new List<Vector3>();
        for (int i = 0; i < pathIndexes.Count; i++)
        {
            Vector3Int pathIndex = pathIndexes[i];
            Vector3 position = hexagonalGrid.GetLocalPositionCell(ref pathIndex);
            position.y += 1.5f;  // 暂时措施
            pathPositions.Add(position);
        }

        return pathPositions;
    }

    public void UnitMovementRange(Vector3Int startIndex, float AP)
    {
        if (costMap == null)
        {
            Debug.LogError("成本地图未初始化！");
        }
        List<Vector3Int> rangeIndexes = hexAStarPathfinding.MoveRange(startIndex, AP, costMap);
        // 根据Indexes获取cells
        List<Cell> cells = GetCellByIndex(rangeIndexes);
        // 根据Cell去渲染

        MainRenderController.Instance.MapMoveRangeHighLight(cells);

    }
    public List<Vector3Int> UnitMovementRangeForAI(Vector3 startPosition, float AP)
    {
        Vector3Int startIndex = hexagonalGrid.GetIndexByPosition(ref startPosition);
        if (costMap == null)
        {
            Debug.LogError("成本地图未初始化！");
        }
        List<Vector3Int> rangeIndexes = hexAStarPathfinding.MoveRange(startIndex, AP, costMap);
        // 根据Indexes获取cells
        return rangeIndexes;

    }

    public bool TargetLocReachedAble(Vector3 endPosition, Vector3 startPostion, float AP)
    {
        bool reachedAble = false;
        Vector3Int startIndex = hexagonalGrid.GetIndexByPosition(ref startPostion);
        Vector3Int endIndex = hexagonalGrid.GetIndexByPosition(ref endPosition);
        List<Vector3Int> rangeIndexes = hexAStarPathfinding.MoveRange(startIndex, AP, costMap);
        if (rangeIndexes.Contains(endIndex)){
            reachedAble = true;
        }
        return reachedAble;
    }

    public void SelectedCell(Vector3 mousePosition)
    {
        Vector3Int index = hexagonalGrid.GetIndexByPosition(ref mousePosition);
        // overload
        Cell selectedCell = GetCellByIndex(index);
        MainRenderController.Instance.SingleCellHighLight(selectedCell);
    }

    /// <summary>
    /// 成本地图初始化
    /// </summary>
    /// <returns></returns>
    private void InitializeCostMap()
    {
        costMap = new Dictionary<Vector3Int, float>();
        foreach (Transform child in hexagonalGrid.transform)
        {
            if (child != null)
            {
                Cell childCell = child.gameObject.GetComponent<Cell>();
                Vector3Int index = childCell.GetIndex();
                string name = childCell.name;
                string[] parts = name.Split('_');
                string type = parts.Last();
                float cost;
                switch (type.ToLower()) 
                {
                    case "grass":
                        cost = 2.0f;
                        break;
                    case "ground":
                        cost = 1.0f;
                        break;
                    case "water":
                        cost = 3.0f;
                        break;
                    case "empty":
                        cost = float.MaxValue;
                        break;
                    default:
                        cost = 9.0f;
                        break;
                }
                costMap.Add(index, cost);

            }
        }
    }

    private List<Cell> GetCellByIndex(List<Vector3Int> indexes) 
    {
        List<Cell> cells = new List<Cell>();
        for (int i = 0; i < indexes.Count; i++)
        {
            Vector3Int index = indexes[i];
            cells.Add(hexagonalGrid.GetCellByIndex(ref index));
        }
        return cells;
    }

    private Cell GetCellByIndex(Vector3Int index)
    {
        Cell cell = hexagonalGrid.GetCellByIndex(ref index);
        return cell;
    }





}
