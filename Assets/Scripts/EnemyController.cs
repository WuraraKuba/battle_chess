using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // ����Ѱ·

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    // �洢����
    private List<GameObject> targetsInRange = new List<GameObject>();

    // �洢����״̬
    private Unit currentUnit;

    // ������˵�״̬��StateMachine�ĺ��ģ�


    private Transform target; // ��ҵ� Transform
    private Vector3 destination;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;


    // AI ���
    public enum AiFocus { Sensing, Chasing, Lost }
    private AiFocus currentFocus = AiFocus.Sensing;

    void Awake()
    {
        // ��ȡѰ·���
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetFloat("MotionSpeed", 2f);
        // ��ɫ��س�ʼ��
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

            // 2. ���������Ŀ���������������
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
                Debug.Log("����"+length);
                MoveTo(targetPos);
            }
           /* // ʹ�� NavMeshAgent ��ֹͣ������Ϊ�����뾶
            float attackRange = 8.0f;

            // directionToTarget.normalized �õ�һ������Ϊ 1 �ķ�������
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
                        // ���˶�ʧĿ�꣬�������һ�������߼�
                        currentFocus = AiFocus.Sensing; // ��Ϊֱ�ӻص�����
                        break;
                }*/
    }

    public void SetTargetAndStartChase(Transform newTarget)
    {
        // ֻ���ڵ��˴�������״̬ʱ�������µ�Ŀ��
        if (currentFocus == AiFocus.Sensing || currentFocus == AiFocus.Lost)
        {
            target = newTarget;

            // �������״̬�� AI ����
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = chaseSpeed;
            currentFocus = AiFocus.Chasing;

            Debug.Log(gameObject.name + " ����Ŀ�꣺" + newTarget.name + "����ʼ׷��");
        }
    }

    // ���������� 2���� SearchController ���á�
    public void HandleTargetLoss(Transform lostTarget)
    {
        // ֻ�е���ʧ�Ķ����ǵ�ǰĿ��ʱ����ִ��Ŀ�궪ʧ�߼�
        if (target != null && target == lostTarget)
        {
            // Ŀ���ܳ���֪��Χ

            target = null;
            currentUnit.changeToIdle();
            navMeshAgent.isStopped = true;
            currentFocus = AiFocus.Lost;

            Debug.Log(gameObject.name + " ��ʧĿ�ꡣ");
        }
    }

    public void MoveTo(Vector3 destination)
    {
        // ����ɫ��״̬
        if (currentUnit.ifInIdle() || currentUnit.ifInMove())
        {
            currentUnit.changeToMove();
            animator.SetFloat("Speed", 2f);   // ��ʼ�ٶ�
            if (navMeshAgent != null)
            {
                // ����ǰѰ··������ѡ�����Ƽ���
                navMeshAgent.ResetPath();

                // �����µ�Ŀ�ĵأ�NavMeshAgent ���Զ�����·�����ƶ�
                navMeshAgent.SetDestination(destination);
            }
        }
        else if (currentUnit.ifInCombat())
        {
            // ����ս������
            animator.SetBool("InCombat", false);
            currentUnit.changeToMove();
            if (navMeshAgent != null)
            {
                // ����ǰѰ··������ѡ�����Ƽ���
                navMeshAgent.ResetPath();

                // �����µ�Ŀ�ĵأ�NavMeshAgent ���Զ�����·�����ƶ�
                navMeshAgent.SetDestination(destination);
            }
        }
    }

}
