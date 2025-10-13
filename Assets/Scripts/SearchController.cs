using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchController : MonoBehaviour
{
    private EnemyController enemyController;
    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        // 1. 确保是玩家单位
        if (other.CompareTag("PlayerUnit"))
        {
            // 2. 检查 EnemyController 是否有效
            if (enemyController != null)
            {
                // 3. 将目标信息传递给 EnemyController
                // 假设 EnemyController 有一个公共方法来设置目标并启动追逐
                enemyController.SetTargetAndStartChase(other.transform);
            }
        }
    }

    // 丢失逻辑：当有对象离开大触发器范围时
    private void OnTriggerExit(Collider other)
    {
        // 1. 确保是玩家单位
        if (other.CompareTag("PlayerUnit"))
        {
            if (enemyController != null)
            {
                // 2. 通知 EnemyController 目标可能丢失
                // 假设 EnemyController 有一个公共方法来处理目标丢失
                enemyController.HandleTargetLoss(other.transform);
            }
        }
    }
}
