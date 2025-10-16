using MapTileGridCreator.Core;
using MapTileGridCreator.CubeImplementation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private CubeGrid cubeGrid;
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
    /// 用于接收现实坐标，然后将其转成地图索引
    /// </summary>
    /// <param name="position"></param>
    public void Position2Index(Vector3 position)
    {
        Vector3Int index = cubeGrid.GetIndexByPosition(ref position);
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

    public float GetMapCellSize()
    {
        return cubeGrid.SizeCell;
    }

}
