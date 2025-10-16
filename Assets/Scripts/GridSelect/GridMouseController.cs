using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于测试以格子为单位进行选择
public class GridMouseController : MonoBehaviour
{
    // 依旧还是射线检测
    private Ray ray;   // 射线检测
    private RaycastHit hit;    // 用于存储射线检测的结果
    private int RaycastIgnoreLayerMask;  // 射线检测需要忽略的

    private Vector3 lastCellPosition = Vector3.zero;  // 用于存储上次选择的单位坐标
    public static GridMouseController Instance { get; private set; }
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
        RaycastIgnoreLayerMask = ~LayerMask.GetMask("trigger", "Ignore Raycast");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 检测射线是否接触到物体
        if (Physics.Raycast(ray, out hit, 1000f, RaycastIgnoreLayerMask))
        {
            // Debug.Log(hit.point);
            // 尝试根据这个坐标，获取方块
            if (GridMapController.Instance != null && MainRenderController.Instance != null)
            {
                Vector3 position = GridMapController.Instance.GetCubeTopCenterPositionByClickPosition(hit.point);
                float cellSize = GridMapController.Instance.GetMapCellSize();
                // 如果 position没变，就不要改了
                if (position != lastCellPosition)
                {
                    MainRenderController.Instance.MapGridCellHighLight(position, cellSize);
                    lastCellPosition = position;
                }
                
            }

        }
    }
}
