using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static Unit;


// �����ͼѰ·�㷨
public class UnitController : MonoBehaviour
{

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    public GameObject SelectionIndicator;
    private bool isSelected = false;

    // �洢����
    private List<GameObject> targetsInRange = new List<GameObject>();

    // �洢����״̬
    private Unit currentUnit;
    [Header("ѡ������")]
    public SpriteRenderer selectImage;

    // ���ڻ�ȡ���̵�����
    private void Awake()  // �ű���ʵ�������һ�������õĺ�������start��Ҫǰ��start��updateǰ�����������Ϊʵ���������ڵĵ�һ������
    {
        selectImage.enabled = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetFloat("MotionSpeed", 2f);
        // ��ɫ��س�ʼ��
        currentUnit = GetComponent<Unit>();
        currentUnit.currentHealth = currentUnit.maxHealth;
        // ��Ѫ����λ������һ��
        
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        if (isSelected)
        {
            TurnOnSelector();
        }
        else
        {
            TurnOffSelector();
        }
        float currentSpeed = navMeshAgent.velocity.magnitude;
        if (currentUnit.ifInMove())
        {
            animator.SetFloat("Speed", currentSpeed);
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && navMeshAgent.velocity.sqrMagnitude == 0f)
        {
            // �����ƶ�ֹͣ����߼�
            movementFinshed();
        }
    }
    // �ܻ��߼�
    public void TakeDamage(int damage)
    {
        float actualDamage = Mathf.Max(1, damage - currentUnit.defense);
        currentUnit.currentHealth = Mathf.Max(currentUnit.currentHealth - actualDamage, 0);
        if (UIController.Instance != null)
        {
            // ֪ͨ UI ������չʾ�˺�
            UIController.Instance.DisplayDamage(
                actualDamage,
                transform.position
            );
        }
        if (currentUnit.currentHealth <= 0 && currentUnit.state != UnitState.Dead)
        {
            Die();
        }
    }

    // �����߼�
    public void Die()
    {
        currentUnit.changeToDead();
        if (GetComponent<UnityEngine.AI.NavMeshAgent>() is UnityEngine.AI.NavMeshAgent navMeshAgent)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
        }
        // ֹͣ�ý�ɫ��UnitController�ű�
        this.enabled = false;
/*        if (GetComponent<SphereCollider>() is SphereCollider sphereCollider)
        {
            // ���÷�Χ������
            sphereCollider.enabled = false;
        }*/
        // ��������ײ�壨���ʿ��������������������ײ�� Collider��

        // 4. ���� UI ��ģ�� (�޶����汾)

/*        // ����Ѫ�� UI
        if (transform.Find("HealthBarCanvas") is Transform healthBar)
        {
            Destroy(healthBar.gameObject);
        }*/

/*        // ����ģ�ͣ���Ϊ�������Ӿ�������
        if (transform.Find("ToonSoldier_model") is Transform model)
        {
            model.gameObject.SetActive(false);
        }*/

        // 5. ���ٶ���
        Destroy(gameObject, 0.5f);
    }
    
    // �н��߼�
    public void MoveTo(Vector3 destination)
    {

            Debug.Log("start_path");
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

    private void movementFinshed()
    {
        // һ��ʼ�������״̬
        if (currentUnit.ifInMove()) 
        {
            currentUnit.changeToIdle();
        }
        // ȡ��ѡ��
        
        /*else if (currentUnit.ifInIdle() && targetsInRange.Count > 0)
        {
            // ��Χ���е��ˣ�
            currentUnit.changeToCombat();
            animator.SetBool("InCombat", true);

            // ǿ��ֹͣ NavMeshAgent������ Agent ����΢��λ��
            navMeshAgent.ResetPath();
        }
        else if (currentUnit.ifInCombat())
        {
            animator.SetBool("InCombat", true);
        }
        else
        {
            // ǿ��ֹͣ NavMeshAgent
            navMeshAgent.ResetPath();
        }*/
    }

    public void Select()
    {
        isSelected = true;
        if (SelectionIndicator != null)
        {
            SelectionIndicator.SetActive(true);
        }
    }

    public void Deselect()
    {
        isSelected = false;
        if (SelectionIndicator != null)
        {
            SelectionIndicator.SetActive(false);
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    /*public List<Vector3> FindReachableNodes(Vector3 start, int range)
    {
        Queue<GridNode> queue = new Queue<GridNode>();
        HashSet<Vector3> visited = new HashSet<Vector3>();
        Dictionary<Vector3, int> distance = new Dictionary<Vector3, int>();

        // ����ת����������
        int start_x = Mathf.RoundToInt((start.x - gridOrigin.x) / cellSize);
        int start_z = Mathf.RoundToInt((start.z - gridOrigin.z) / cellSize);
        start = new Vector3(start_x, gridOrigin.y, start_z);

        // ����������У����ʼ�¼���������Ϊ0
        GridNode startNode = new GridNode(start, null);
        queue.Enqueue(startNode);
        visited.Add(start);
        distance.Add(start, 0);

        List<Vector3> reachableNodes = new List<Vector3>();

        while (queue.Count > 0)
        {
            GridNode currentNode = queue.Dequeue();
            int currentDistance = distance[currentNode.position];
            if (currentDistance > range)
            {
                continue;
            }
            else
            {
                float xPos = gridOrigin.x + currentNode.position.x * cellSize;
                float zPos = gridOrigin.z + currentNode.position.z * cellSize;
                reachableNodes.Add(new Vector3(xPos, gridOrigin.y, zPos));
                // ��ȡ�������
                List<Vector3> NeiborList = GetNeighbors(currentNode.position);
                foreach (Vector3 p in NeiborList)
                {
                    if (!visited.Contains(p))
                    {
                        visited.Add(p);
                        GridNode NeiborNode = new GridNode(p, currentNode);
                        queue.Enqueue(NeiborNode);
                        distance.Add(p, currentDistance + 1);
                    }

                }
            }
        }

        return reachableNodes;
    }
    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        Queue<GridNode> queue = new Queue<GridNode> ();
        HashSet<Vector3> visited = new HashSet<Vector3>();
        // �������յ�ֱ�ת��Ϊ��������

        int start_x = Mathf.RoundToInt((start.x - gridOrigin.x) / cellSize);
        int start_z = Mathf.RoundToInt((start.z - gridOrigin.z) / cellSize);
        start = new Vector3(start_x, gridOrigin.y + cellSize, start_z);
        int end_x = Mathf.RoundToInt((end.x - gridOrigin.x) / cellSize);
        int end_z = Mathf.RoundToInt((end.z - gridOrigin.z) / cellSize);
        end = new Vector3(end_x, gridOrigin.y + cellSize, end_z);
        // ����������кͷ��ʼ�¼
        GridNode startNode = new GridNode(start, null);
        queue.Enqueue(startNode);
        visited.Add(start);

        while (queue.Count > 0) 
        {
            // ������
            GridNode currentNode = queue.Dequeue();

            // ����ҵ��յ�
            if (currentNode.position == end) 
            {
                return ReconstructPath(currentNode);
            }
            List < Vector3 > NeiborList = GetNeighbors(currentNode.position);
            foreach (Vector3 neighborPos in GetNeighbors(currentNode.position)) 
            { 
                if (!visited.Contains(neighborPos))
                {
                    visited.Add(neighborPos);
                    GridNode neighborNode = new GridNode(neighborPos, currentNode);
                    queue.Enqueue(neighborNode);
                }
            }
        }
        return new List<Vector3>(); 
    }

    private List<Vector3> GetNeighbors(Vector3 pos) 
    {
        List<Vector3> neighbors = new List<Vector3>();
        Vector3[] directions = new Vector3[]
        {
            new Vector3(0, 0, cellSize),
            new Vector3(0, 0, -cellSize),
            new Vector3(cellSize, 0, 0),
            new Vector3(-cellSize, 0, 0),
        };
        foreach (Vector3 direction in directions) 
        {
            Vector3 neighborPos = pos + direction;
            // ��ȡ��������������
            int x = Mathf.RoundToInt(neighborPos.x  / cellSize);
            int z = Mathf.RoundToInt(neighborPos.z / cellSize);
            if (x >= 0 && z >= 0 && x < width && z < height)
            {
                neighbors.Add(neighborPos);
            }
        }
        return neighbors;

    }

    private List<Vector3> ReconstructPath(GridNode endNode) 
    {
        Debug.Log("�ؽ�·��");
        List<Vector3> path = new List<Vector3>();
        GridNode currentNode = endNode;
        while (currentNode != null) 
        {
            float xPos = gridOrigin.x + currentNode.position.x * cellSize;
            float zPos = gridOrigin.z + currentNode.position.z * cellSize;
            path.Add(new Vector3(xPos, gridOrigin.y + cellSize, zPos));
            //path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }*/

    // ѡ��������
    public void TurnOffSelector()
    {
        selectImage.enabled = false;
    }

    //Turns on the sprite renderer
    public void TurnOnSelector()
    {
        selectImage.enabled = true;
    }
}
