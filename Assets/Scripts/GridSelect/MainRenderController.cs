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

    // 当前处理的高亮单位
    private GameObject currentHighlightInstance;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void ClearHighlight()
    {
        if (currentHighlightInstance != null)
        {
            Destroy(currentHighlightInstance);
            currentHighlightInstance = null;
        }
    }
}
