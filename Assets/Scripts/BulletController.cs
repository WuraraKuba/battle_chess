using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int damageAmount = 50;
    public float lifetime = 5f;
    public GameObject owner;  // �ӵ�������

    void Start()
    {
        // ȷ���ӵ�������Զ����ȥ
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. ��ȡĿ���ϵ� Unit �ű�
        UnitController targetUnitController = other.GetComponentInParent<UnitController>();
        Debug.Log(owner);
        // �����ײ���������Ƿ����ӵ���ӵ����
        if (other.gameObject == owner || other.transform.root.gameObject == owner)
        {
            return;
        }

        if (targetUnitController != null)
        {
            // 2. Ŀ���� Unit �ű��������˺�����
            Debug.Log("damageAmount" + damageAmount);
            targetUnitController.TakeDamage(damageAmount);

        }
        // 3. �����ӵ�
        Destroy(gameObject);

    }
}
