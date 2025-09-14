using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于构建网格状棋盘
public class GridGenerator : MonoBehaviour
{
    public GameObject gridCellPrefab;

    public int width = 48;
    public int height = 48;
    // Start is called before the first frame update
    void Start()
    {
        // 获取原点
        Vector3 gridOrigin = transform.position;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                // 计算方格位置
                Vector3 relativePosition = new Vector3(i, 0, j);
                Vector3 worldPosition = gridOrigin + relativePosition;
                // 实例化预制件，并将其作为子物体
                GameObject cell = Instantiate(gridCellPrefab, worldPosition, Quaternion.identity, transform);
                // 设置tag
                cell.tag = "GridCell";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
