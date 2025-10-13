using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // 用于寻路

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    // 存储敌人
    private List<GameObject> targetsInRange = new List<GameObject>();

    // 存储自身状态
    private Unit currentUnit;

    // 定义敌人的状态（StateMachine的核心）


    private Transform target; // 玩家的 Transform
    private Vector3 destination;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;


    // AI 相关
    public enum AiFocus { Sensing, Chasing, Lost }
    private AiFocus currentFocus = AiFocus.Sensing;

    void Awake()
    {
        // 获取寻路组件
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetFloat("MotionSpeed", 2f);
        // 角色相关初始化
        currentUnit = GetComponent<Unit>();
        currentUnit.currentHealth = currentUnit.maxHealth;

    }

    void Update()
    {
        if (currentUnit.ifInIdle())
        {
            animator.SetFloat("Speed", 0f);
        }
        if (target != null)
        {
            Vector3 targetPos = target.position;
            Vector3 selfPos = transform.position;

            // 2. 计算从自身到目标的完整方向向量
            Vector3 directionToTarget = targetPos - selfPos;
            float length = directionToTarget.magnitude;
            if (length < 56.0) 
            {
                navMeshAgent.ResetPath();
                //navMeshAgent.isStopped = true;
                currentUnit.changeToIdle();
            }
            else
            {
                Debug.Log("距离"+length);
                MoveTo(targetPos);
            }
           /* // 使用 NavMeshAgent 的停止距离作为攻击半径
            float attackRange = 8.0f;

            // directionToTarget.normalized 得到一个长度为 1 的方向向量
            Vector3 destination = targetPos - directionToTarget.normalized * attackRange;*/
            
        }
        /*        switch (currentFocus)
                {
                    case AiFocus.Sensing:
                        SenseAndPatrol();
                        break;
                    case AiFocus.Chasing:
                        ChaseTarget();
                        break;
                    case AiFocus.Lost:
                        // 敌人丢失目标，可以添加一段搜索逻辑
                        currentFocus = AiFocus.Sensing; // 简化为直接回到索敌
                        break;
                }*/
    }

    public void SetTargetAndStartChase(Transform newTarget)
    {
        // 只有在敌人处于索敌状态时才锁定新的目标
        if (currentFocus == AiFocus.Sensing || currentFocus == AiFocus.Lost)
        {
            target = newTarget;

            // 启动宏观状态和 AI 焦点
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = chaseSpeed;
            currentFocus = AiFocus.Chasing;

            Debug.Log(gameObject.name + " 发现目标：" + newTarget.name + "，开始追逐。");
        }
    }

    // 【新增方法 2：供 SearchController 调用】
    public void HandleTargetLoss(Transform lostTarget)
    {
        // 只有当丢失的对象是当前目标时，才执行目标丢失逻辑
        if (target != null && target == lostTarget)
        {
            // 目标跑出感知范围

            target = null;
            currentUnit.changeToIdle();
            navMeshAgent.isStopped = true;
            currentFocus = AiFocus.Lost;

            Debug.Log(gameObject.name + " 丢失目标。");
        }
    }

    public void MoveTo(Vector3 destination)
    {
        // 检查角色的状态
        if (currentUnit.ifInIdle() || currentUnit.ifInMove())
        {
            currentUnit.changeToMove();
            animator.SetFloat("Speed", 2f);   // 初始速度
            if (navMeshAgent != null)
            {
                // 清理当前寻路路径（可选，但推荐）
                navMeshAgent.ResetPath();

                // 设置新的目的地，NavMeshAgent 会自动计算路径并移动
                navMeshAgent.SetDestination(destination);
            }
        }
        else if (currentUnit.ifInCombat())
        {
            // 结束战斗动画
            animator.SetBool("InCombat", false);
            currentUnit.changeToMove();
            if (navMeshAgent != null)
            {
                // 清理当前寻路路径（可选，但推荐）
                navMeshAgent.ResetPath();

                // 设置新的目的地，NavMeshAgent 会自动计算路径并移动
                navMeshAgent.SetDestination(destination);
            }
        }
    }

}
