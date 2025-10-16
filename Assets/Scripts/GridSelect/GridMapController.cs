using MapTileGridCreator.Core;
using MapTileGridCreator.CubeImplementation;
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
    public void Position2Index(Vector3 position)
    {
        Vector3Int index = cubeGrid.GetIndexByPosition(ref position);
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

    public float GetMapCellSize()
    {
        return cubeGrid.SizeCell;
    }

}
