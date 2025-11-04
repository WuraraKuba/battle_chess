using MapTileGridCreator.Core;
using MapTileGridCreator.CubeImplementation;
using MapTileGridCreator.HexagonalImplementation;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
/// <summary>
/// 各种高亮啊，之类的渲染相关的
/// </summary>
public class MainRenderController : MonoBehaviour
{
    public static MainRenderController Instance {  get; private set; }
    // 用于地图单元的高亮预制件
    [SerializeField]
    private GameObject mapCellHighlightPrefab;
    [SerializeField]
    private GameObject mapCellKeepHighlightPrefab;

    // 当前处理的高亮单位
    private GameObject currentHighlightInstance;
    private GameObject currentKeepHighlightInstance;
    private List<Cell> currentHighlightedCells = new List<Cell>();
    private Cell currentHighlightCell;

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
    }

    // 是用于正方格
    public void MapGridCellHighLight(Vector3 position, float cellSize)
    {
        // 清除旧的高亮
        ClearHighlight();
        // 实例化预制件
        currentHighlightInstance = Instantiate(mapCellHighlightPrefab, transform);
        // 设置高亮位置
        currentHighlightInstance.transform.position = position;
        // 设置高亮尺寸
        float scaleSize = cellSize * 0.95f;
        currentHighlightInstance.transform.localScale = new Vector3(scaleSize, 0.01f, scaleSize);
    }
    public void MapHexCellHighLight(Vector3 position, float cellSize)
    {
        // 清除旧的高亮
        ClearHighlight();
        // 实例化预制件
        currentHighlightInstance = Instantiate(mapCellHighlightPrefab, transform);
        // 设置高亮位置
        currentHighlightInstance.transform.position = position;
        // 设置高亮尺寸
        float scaleSize = cellSize * 0.95f;
        currentHighlightInstance.transform.localScale = new Vector3(scaleSize, 0.01f, scaleSize);
    }

    public void MapHexCellKeepHighLight(Vector3 position, float cellSize)
    {
        // 清除旧的高亮
        ClearKeepHighlight();
        // 实例化预制件
        currentKeepHighlightInstance = Instantiate(mapCellKeepHighlightPrefab, transform);
        // 设置高亮位置
        currentKeepHighlightInstance.transform.position = position;
        // 设置高亮尺寸
        float scaleSize = cellSize * 0.95f;
        currentKeepHighlightInstance.transform.localScale = new Vector3(scaleSize, 0.01f, scaleSize);
    }
    public void MapMoveRangeHighLight(List<Cell> cells)
    {
        if (currentHighlightedCells != null) 
        {
            currentHighlightedCells.Clear();
        }
        
        currentHighlightedCells = cells;
        int layerIndex = LayerMask.NameToLayer("outline");
        layerIndex = 8;
        if (layerIndex != -1)
        {
            uint outlineMask = 1u << layerIndex;
            foreach (Cell cell in currentHighlightedCells)
            {
                MeshRenderer meshRenderer = cell.gameObject.GetComponentInChildren<MeshRenderer>();
                // 修改mask
                if (meshRenderer != null)
                {
                    // 将 MeshRenderer 的 Rendering Layer Mask 设置为 OUTLINE_MASK
                    meshRenderer.renderingLayerMask = outlineMask;
                }
            }
        }
    }
    public void SingleCellHighLight(Cell selectCell)
    {
        if (currentHighlightCell != null)
        {
            ClearSingleCellHighlight();
            currentHighlightCell = null;
        }

        currentHighlightCell = selectCell;
        int layerIndex = LayerMask.NameToLayer("outline");
        layerIndex = 9;
        if (layerIndex != -1)
        {
            uint outlineMask = 1u << layerIndex;
            MeshRenderer meshRenderer = currentHighlightCell.gameObject.GetComponentInChildren<MeshRenderer>();
            meshRenderer.renderingLayerMask = outlineMask;
        }
    }
    public void ClearMoveRangeHighlights()
    {
        if (currentHighlightedCells == null || currentHighlightedCells.Count == 0)
        {
            return;
        }

        foreach (Cell cell in currentHighlightedCells)
        {
            // 确保对象没有被销毁
            if (cell != null)
            {
                MeshRenderer renderer = cell.GetComponentInChildren<MeshRenderer>(true);

                if (renderer != null)
                {
                    // 将 Mask 设置为 0，即不再被 Outline 相机渲染
                    renderer.renderingLayerMask = 12;
                }
            }
        }
        currentHighlightedCells.Clear();
    }
    public void ClearSingleCellHighlight()
    {
        if (currentHighlightCell == null)
        {
            return;
        }
        MeshRenderer renderer = currentHighlightCell.GetComponentInChildren<MeshRenderer>(true);
        renderer.renderingLayerMask = 12;
        currentHighlightCell = null;
    }

    public void ClearHighlight()
    {
        if (currentHighlightInstance != null)
        {
            Destroy(currentHighlightInstance);
            currentHighlightInstance = null;
        }
    }

    public void ClearKeepHighlight()
    {
        if (currentKeepHighlightInstance != null)
        {
            Destroy(currentKeepHighlightInstance);
            currentKeepHighlightInstance = null;
        }
    }
}
