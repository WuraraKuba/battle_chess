using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于构建网格状棋盘
public class GridGenerator : MonoBehaviour
{
    public GameObject gridCellPrefab;

    public int width = 48;
    public int height = 48;
    public float cellSize = 1f;
    public Vector3 gridOrigin;

    public Dictionary<Vector3, GameObject> gridCells = new Dictionary<Vector3, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        // 获取原点
        gridOrigin = transform.position;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                // 计算方格位置
                Vector3 relativePosition = new Vector3(i * cellSize, 0, j * cellSize);
                Vector3 worldPosition = gridOrigin + relativePosition;
                // 实例化预制件，并将其作为子物体
                GameObject cell = Instantiate(gridCellPrefab, worldPosition, Quaternion.identity, transform);
                // 设置tag
                cell.tag = "GridCell";
                if (!gridCells.ContainsKey(relativePosition)) 
                {
                    gridCells.Add(worldPosition, cell);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
