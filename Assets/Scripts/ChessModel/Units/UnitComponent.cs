using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ʹ��UnitData�Ǹ�����в���Ļ�ֻ������һ��prefab����Ҫͨ��������������Ϣ��ע�뵽prefab��
/// </summary>
public class UnitComponent : MonoBehaviour
{
    // ��ʱֻŪ��λ��
    public Vector3 UnitLocation;
    // �Ӹ��Ƿ��ǵ��˵��ж�
    public bool isEnemy;
    public void Initialize(UnitData data)
    {
        // ȷ�� UnitData ���ǿգ��Է���һ
        if (data == null) return;

        // 1. �������ʲ��л�ȡ����
        UnitLocation = data.UnitLocation;
        // 3. (��ѡ) �������������õ�λ���Ӿ�����Ϊ�߼�
        Debug.Log($"��λ��ʼ���ɹ�");
    }

    public Vector3 GetLocation()
    {
        return UnitLocation;
    }
    public void ChangeLocation(Vector3 location)
    {
        UnitLocation = location;
    }
}

