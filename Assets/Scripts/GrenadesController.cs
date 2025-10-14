using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrenadesController : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 使用计算出的速度和时间发射手雷
    /// </summary>
    /// <param name="startPoint">世界起始点</param>
    /// <param name="targetPoint">世界目标点</param>
    /// <param name="timeToHit">到达目标所需的时间</param>
    /// <param name="launchVelocity">计算出的总初速度</param>
    /// <param name="launchAngleRad">计算出的发射角度（弧度）</param>
    public void Launch(Vector3 startPoint, Vector3 targetPoint, float launchVelocity, float launchAngleRad)
    {
        // 1. 计算所需的 XZ 平面方向 (Launch Direction)
        Vector3 startFlat = new Vector3(startPoint.x, 0, startPoint.z);
        Vector3 targetFlat = new Vector3(targetPoint.x, 0, targetPoint.z);
        Vector3 directionToTargetFlat = (targetFlat - startFlat).normalized;

        // 2. 根据 LaunchAngleRad 分解初速度 Vx 和 Vy
        float Vx = launchVelocity * Mathf.Cos(launchAngleRad);
        float Vy = launchVelocity * Mathf.Sin(launchAngleRad);

        // 3. 构建 XZ 平面上的速度向量 (水平初速度)
        Vector3 velocityXZ = directionToTargetFlat * Vx;

        // 4. 构建最终的世界坐标系初速度
        Vector3 initialVelocity = velocityXZ + new Vector3(0, Vy, 0);

        // 5. 赋予刚体初速度
        rb.velocity = initialVelocity;

        // 6. 确保重力生效（如果你的 Rigidbody 默认没有开启）
        rb.useGravity = true;

        Debug.Log($"手雷发射！初速度: {initialVelocity.magnitude:F2}, 水平分量: {Vx:F2}, 垂直分量: {Vy:F2}");
    }
}
