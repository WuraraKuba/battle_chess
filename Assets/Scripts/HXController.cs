using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HXController : MonoBehaviour
{
    public static HXController Instance { get; private set; }

    public LineRenderer lineRenderer;
    public int segmentCount = 30;
    public float initialSpeed = 15f;
    public float maxThrowRange = 100f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // ȷ���ڳ����л�ʱҲ�ܳ�������
            // DontDestroyOnLoad(this.gameObject); 
        }

        // ȷ�� LineRenderer �������
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogError("HXController ȱ�� LineRenderer ���������ӡ�");
            }
        }
        HidePath(); // ��ʼ��ʱ����
    }

    // �յ��������
    public Vector3 GetRestrictedEndPoint(Vector3 startPoint, Vector3 endPoint, float maxRange)
    {
        // A. ����ˮƽ����ͻ�����
        Vector3 startFlat = new Vector3(startPoint.x, 0, startPoint.z);
        Vector3 endFlat = new Vector3(endPoint.x, 0, endPoint.z);
        float horizontalDistanceTarget = Vector3.Distance(startFlat, endFlat);
        float deltaYTarget = endPoint.y - startPoint.y;

        if (horizontalDistanceTarget <= maxRange)
        {
            // Ŀ���ڷ�Χ�ڣ���������
            return endPoint;
        }

        // Ŀ�곬����Χ����������

        // 1. ��ȡˮƽ��������
        Vector3 directionToTargetFlat = (endFlat - startFlat).normalized;

        // 2. �������ƺ��ˮƽ�յ�
        Vector3 restrictedFlatEnd = startFlat + directionToTargetFlat * maxRange;

        // 3. �����µ��յ㣺���ƺ��ˮƽ���� + (���Y + ԭʼ�߶Ȳ�)
        float restrictedEndY = startPoint.y + deltaYTarget;

        return new Vector3(restrictedFlatEnd.x, restrictedEndY, restrictedFlatEnd.z);
    }

    // ----------------------------------------
    // �������߻����߼���
    // ----------------------------------------
    public void DrawSkillPath(Vector3 startPoint, Vector3 endPoint)
    {

        if (lineRenderer == null) return;

        // ע�⣺���� endPoint �Ѿ��Ǿ����ⲿ���ƺ�ĵ㣡
        Vector3 actualEndPoint = endPoint;

        // ** ע�⣺������Ҫȷ���㽫 maxAngleRad, initialSpeed, maxThrowRange 
        //         ��Ϊ��Ĺ������� (public float xxx) �����Ǿֲ����� **
        float maxAngleRad = 30f * Mathf.Deg2Rad; // ������Щֵ�������Ա
        float g = Physics.gravity.magnitude;
        float velocity = initialSpeed;

        // --- 1. ����켣������� ---
        Vector3 startFlat = new Vector3(startPoint.x, 0, startPoint.z);
        Vector3 endFlat = new Vector3(actualEndPoint.x, 0, actualEndPoint.z); // ʹ����������յ�
        float actualHorizontalDistance = Vector3.Distance(startFlat, endFlat);
        float actualDeltaY = actualEndPoint.y - startPoint.y;

        // ��ȡˮƽ��������
        Vector3 directionToTargetFlat = (endFlat - startFlat).normalized;

        // ----------------------------------------------------
        // ������������������ȷ����ת��
        // ----------------------------------------------------
        Quaternion trajectoryRotation = Quaternion.FromToRotation(Vector3.right, directionToTargetFlat);

        // ----------------------------------------------------
        // ��2D �������㣨�� X-Y �ֲ�ƽ���ϣ���
        // ----------------------------------------------------

        // 1. �̶� Vx (ʹ�����Ա initialSpeed)
        float Vx = velocity * Mathf.Cos(maxAngleRad);
        if (Vx < 0.001f) Vx = 0.001f;

        // 2. ȷ�� timeToHit
        float timeToHit = actualHorizontalDistance / Vx;
        timeToHit = Mathf.Max(0.1f, timeToHit);

        // 3. ���� Vy
        float Vy = (actualDeltaY / timeToHit) + (0.5f * g * timeToHit);

        // ----------------------------------------------------
        // ������ѭ����
        // ----------------------------------------------------

        lineRenderer.enabled = true;
        lineRenderer.positionCount = segmentCount;
        Vector3[] points = new Vector3[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1) * timeToHit;

            // 1. ����ֲ�����ϵ�µ� 2D �켣�� (x, y)
            float localX = Vx * t; // ˮƽλ��
            float localY = Vy * t - (0.5f * g * t * t); // ��ֱ�߶�

            // �ֲ� 3D ���� (����תǰ��Z ��Ϊ 0)
            Vector3 localTrajectoryPoint = new Vector3(localX, localY, 0);

            // 2. ת�����������꣺ 
            // Ӧ��֮ǰ��������ת (���ֲ� X ����뵽Ŀ�귽��)
            // Ӧ��ƽ�� (���� startPoint)
            Vector3 worldPoint = startPoint + (trajectoryRotation * localTrajectoryPoint);

            points[i] = worldPoint;

            // 3. ȷ���յ㾫ȷ���� (ʼ��ǿ�ƣ����ֲ��������)
            if (i == segmentCount - 1)
            {
                points[i] = actualEndPoint;
            }
        }

        lineRenderer.SetPositions(points);
    }

    // ����·��
    public void HidePath()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
}
