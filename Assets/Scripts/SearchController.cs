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
        // 1. ȷ������ҵ�λ
        if (other.CompareTag("PlayerUnit"))
        {
            // 2. ��� EnemyController �Ƿ���Ч
            if (enemyController != null)
            {
                // 3. ��Ŀ����Ϣ���ݸ� EnemyController
                // ���� EnemyController ��һ����������������Ŀ�겢����׷��
                enemyController.SetTargetAndStartChase(other.transform);
            }
        }
    }

    // ��ʧ�߼������ж����뿪�󴥷�����Χʱ
    private void OnTriggerExit(Collider other)
    {
        // 1. ȷ������ҵ�λ
        if (other.CompareTag("PlayerUnit"))
        {
            if (enemyController != null)
            {
                // 2. ֪ͨ EnemyController Ŀ����ܶ�ʧ
                // ���� EnemyController ��һ����������������Ŀ�궪ʧ
                enemyController.HandleTargetLoss(other.transform);
            }
        }
    }
}
