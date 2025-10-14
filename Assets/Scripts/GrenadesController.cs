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
    /// ʹ�ü�������ٶȺ�ʱ�䷢������
    /// </summary>
    /// <param name="startPoint">������ʼ��</param>
    /// <param name="targetPoint">����Ŀ���</param>
    /// <param name="timeToHit">����Ŀ�������ʱ��</param>
    /// <param name="launchVelocity">��������ܳ��ٶ�</param>
    /// <param name="launchAngleRad">������ķ���Ƕȣ����ȣ�</param>
    public void Launch(Vector3 startPoint, Vector3 targetPoint, float launchVelocity, float launchAngleRad)
    {
        // 1. ��������� XZ ƽ�淽�� (Launch Direction)
        Vector3 startFlat = new Vector3(startPoint.x, 0, startPoint.z);
        Vector3 targetFlat = new Vector3(targetPoint.x, 0, targetPoint.z);
        Vector3 directionToTargetFlat = (targetFlat - startFlat).normalized;

        // 2. ���� LaunchAngleRad �ֽ���ٶ� Vx �� Vy
        float Vx = launchVelocity * Mathf.Cos(launchAngleRad);
        float Vy = launchVelocity * Mathf.Sin(launchAngleRad);

        // 3. ���� XZ ƽ���ϵ��ٶ����� (ˮƽ���ٶ�)
        Vector3 velocityXZ = directionToTargetFlat * Vx;

        // 4. �������յ���������ϵ���ٶ�
        Vector3 initialVelocity = velocityXZ + new Vector3(0, Vy, 0);

        // 5. ���������ٶ�
        rb.velocity = initialVelocity;

        // 6. ȷ��������Ч�������� Rigidbody Ĭ��û�п�����
        rb.useGravity = true;

        Debug.Log($"���׷��䣡���ٶ�: {initialVelocity.magnitude:F2}, ˮƽ����: {Vx:F2}, ��ֱ����: {Vy:F2}");
    }
}
