using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int damageAmount = 50;
    public float lifetime = 5f;
    public GameObject owner;  // 子弹发射者

    void Start()
    {
        // 确保子弹不会永远飞下去
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. 获取目标上的 Unit 脚本
        UnitController targetUnitController = other.GetComponentInParent<UnitController>();
        Debug.Log(owner);
        // 检查碰撞到的物体是否是子弹的拥有者
        if (other.gameObject == owner || other.transform.root.gameObject == owner)
        {
            return;
        }

        if (targetUnitController != null)
        {
            // 2. 目标有 Unit 脚本，调用伤害函数
            Debug.Log("damageAmount" + damageAmount);
            targetUnitController.TakeDamage(damageAmount);

        }
        // 3. 销毁子弹
        Destroy(gameObject);

    }
}
