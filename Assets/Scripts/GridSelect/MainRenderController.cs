using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
/// <summary>
/// ���ָ�������֮�����Ⱦ��ص�
/// </summary>
public class MainRenderController : MonoBehaviour
{
    public static MainRenderController Instance {  get; private set; }
    // ���ڵ�ͼ��Ԫ�ĸ���Ԥ�Ƽ�
    [SerializeField]
    private GameObject mapCellHighlightPrefab;

    // ��ǰ����ĸ�����λ
    private GameObject currentHighlightInstance;

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
        // ����ɵĸ���
        ClearHighlight();
        // ʵ����Ԥ�Ƽ�
        currentHighlightInstance = Instantiate(mapCellHighlightPrefab, transform);
        // ���ø���λ��
        currentHighlightInstance.transform.position = position;
        // ���ø����ߴ�
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
