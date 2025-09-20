using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ڹ�������״����
public class GridGenerator : MonoBehaviour
{
    public GameObject gridCellPrefab;

    public int width = 48;
    public int height = 48;
    public float cellSize = 1f;
    public Vector3 gridOrigin;
    // Start is called before the first frame update
    void Start()
    {
        // ��ȡԭ��
        gridOrigin = transform.position;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                // ���㷽��λ��
                Vector3 relativePosition = new Vector3(i * cellSize, 0, j * cellSize);
                Vector3 worldPosition = gridOrigin + relativePosition;
                // ʵ����Ԥ�Ƽ�����������Ϊ������
                GameObject cell = Instantiate(gridCellPrefab, worldPosition, Quaternion.identity, transform);
                // ����tag
                cell.tag = "GridCell";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
