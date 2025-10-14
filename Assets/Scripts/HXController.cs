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
            // 确保在场景切换时也能持续工作
            // DontDestroyOnLoad(this.gameObject); 
        }

        // 确保 LineRenderer 组件存在
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogError("HXController 缺少 LineRenderer 组件！请添加。");
            }
        }
        HidePath(); // 初始化时隐藏
    }

    // 终点距离限制
    public Vector3 GetRestrictedEndPoint(Vector3 startPoint, Vector3 endPoint, float maxRange)
    {
        // A. 计算水平距离和基础点
        Vector3 startFlat = new Vector3(startPoint.x, 0, startPoint.z);
        Vector3 endFlat = new Vector3(endPoint.x, 0, endPoint.z);
        float horizontalDistanceTarget = Vector3.Distance(startFlat, endFlat);
        float deltaYTarget = endPoint.y - startPoint.y;

        if (horizontalDistanceTarget <= maxRange)
        {
            // 目标在范围内，无需修正
            return endPoint;
        }

        // 目标超出范围，进行修正

        // 1. 获取水平方向向量
        Vector3 directionToTargetFlat = (endFlat - startFlat).normalized;

        // 2. 计算限制后的水平终点
        Vector3 restrictedFlatEnd = startFlat + directionToTargetFlat * maxRange;

        // 3. 构建新的终点：限制后的水平坐标 + (起点Y + 原始高度差)
        float restrictedEndY = startPoint.y + deltaYTarget;

        return new Vector3(restrictedFlatEnd.x, restrictedEndY, restrictedFlatEnd.z);
    }

    // ----------------------------------------
    // 【抛物线绘制逻辑】
    // ----------------------------------------
    public void DrawSkillPath(Vector3 startPoint, Vector3 endPoint)
    {

        if (lineRenderer == null) return;

        // 注意：这里 endPoint 已经是经过外部限制后的点！
        Vector3 actualEndPoint = endPoint;

        // ** 注意：这里需要确保你将 maxAngleRad, initialSpeed, maxThrowRange 
        //         作为类的公共变量 (public float xxx) 而不是局部变量 **
        float maxAngleRad = 30f * Mathf.Deg2Rad; // 假设这些值来自类成员
        float g = Physics.gravity.magnitude;
        float velocity = initialSpeed;

        // --- 1. 计算轨迹所需参数 ---
        Vector3 startFlat = new Vector3(startPoint.x, 0, startPoint.z);
        Vector3 endFlat = new Vector3(actualEndPoint.x, 0, actualEndPoint.z); // 使用修正后的终点
        float actualHorizontalDistance = Vector3.Distance(startFlat, endFlat);
        float actualDeltaY = actualEndPoint.y - startPoint.y;

        // 获取水平方向向量
        Vector3 directionToTargetFlat = (endFlat - startFlat).normalized;

        // ----------------------------------------------------
        // 【核心修正：创建正确的旋转】
        // ----------------------------------------------------
        Quaternion trajectoryRotation = Quaternion.FromToRotation(Vector3.right, directionToTargetFlat);

        // ----------------------------------------------------
        // 【2D 弹道解算（在 X-Y 局部平面上）】
        // ----------------------------------------------------

        // 1. 固定 Vx (使用类成员 initialSpeed)
        float Vx = velocity * Mathf.Cos(maxAngleRad);
        if (Vx < 0.001f) Vx = 0.001f;

        // 2. 确定 timeToHit
        float timeToHit = actualHorizontalDistance / Vx;
        timeToHit = Mathf.Max(0.1f, timeToHit);

        // 3. 反求 Vy
        float Vy = (actualDeltaY / timeToHit) + (0.5f * g * timeToHit);

        // ----------------------------------------------------
        // 【绘制循环】
        // ----------------------------------------------------

        lineRenderer.enabled = true;
        lineRenderer.positionCount = segmentCount;
        Vector3[] points = new Vector3[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1) * timeToHit;

            // 1. 计算局部坐标系下的 2D 轨迹点 (x, y)
            float localX = Vx * t; // 水平位移
            float localY = Vy * t - (0.5f * g * t * t); // 垂直高度

            // 局部 3D 向量 (在旋转前，Z 轴为 0)
            Vector3 localTrajectoryPoint = new Vector3(localX, localY, 0);

            // 2. 转换回世界坐标： 
            // 应用之前创建的旋转 (将局部 X 轴对齐到目标方向)
            // 应用平移 (加上 startPoint)
            Vector3 worldPoint = startPoint + (trajectoryRotation * localTrajectoryPoint);

            points[i] = worldPoint;

            // 3. 确保终点精确对齐 (始终强制，以弥补浮点误差)
            if (i == segmentCount - 1)
            {
                points[i] = actualEndPoint;
            }
        }

        lineRenderer.SetPositions(points);
    }

    // 隐藏路径
    public void HidePath()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
}
