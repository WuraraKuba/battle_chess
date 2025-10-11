using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static Unit;


// 网格地图寻路算法
public class UnitController : MonoBehaviour
{
    // 临时方案，未来应该尝试做一个参数传入
    /*public GridGenerator gridGenerator;
    public GridManager gridManager;
    private int width;
    private int height;
    private float cellSize = 1f;
    private Vector3 gridOrigin;*/
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    public GameObject SelectionIndicator;
    private bool isSelected = false;

    // 存储敌人
    private List<GameObject> targetsInRange = new List<GameObject>();

    // 存储自身状态
    private Unit currentUnit;
    [Header("选择属性")]
    public SpriteRenderer selectImage;

    // 用于获取棋盘的属性
    private void Awake()  // 脚本被实例化后第一个被调用的函数（比start还要前，start在update前），可以理解为实例生命周期的第一个函数
    {
        selectImage.enabled = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetFloat("MotionSpeed", 2f);
        // 角色相关初始化
        currentUnit = GetComponent<Unit>();
        currentUnit.currentHealth = currentUnit.maxHealth;
        // 将血条的位置设置一下
        
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
            // 进入移动停止后的逻辑
            movementFinshed();
        }
/*        if (currentUnit.ifInCombat()) 
        {
            HandleShooting();
        }*/
        /*if (currentUnit.ifInIdle())
        {
            Debug.Log(currentSpeed);
            animator.SetFloat("Speed", currentSpeed);
            if (currentSpeed < 1f)
            {
                currentUnit.changeToIdle();
            } 
        }*/
        //Debug.Log(currentUnit.getState());
    }
    /*// 清除空敌人
    private void CleanTargetList()
    {
        targetsInRange.RemoveAll(target => target == null);
    }
    // 进入战斗逻辑
    private void OnTriggerStay(Collider other)
    {
        CleanTargetList();
        // 1. 确保是敌人，且当前状态为 Combat
        if (currentUnit.ifInIdle())
        {
            
            // 检测是否有敌人，将敌人存入targetsInRange
            if (other.CompareTag("enemy"))
            {
                targetsInRange.Clear();
                targetsInRange.Add(other.gameObject);
            }
            // 如果敌人的数目大于0，将状态转换为战斗状态
            if (targetsInRange.Count > 0)
            {
                currentUnit.changeToCombat();
            }
        }
        else if (currentUnit.ifInCombat()) 
        {
            // 如果有敌人，播放战斗动画
            if (targetsInRange.Count > 0)
            {
                // 面向第一个目标
                Vector3 lookAtPos = targetsInRange[0].transform.position;
                // 消除 Y 轴影响，只在水平面旋转
                lookAtPos.y = transform.position.y;

                // 平滑转向目标 (Slerp 或 Lerp)
                Quaternion targetRotation = Quaternion.LookRotation(lookAtPos - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // 10f 是旋转速度
                // 攻击
                HandleShooting();
                animator.SetBool("InCombat", true);
            }
            // 如果没有敌人，进入待机状态
            else
            {
                animator.SetBool("InCombat", false);
                currentUnit.changeToIdle();
            }
        }
    }
    // 攻击逻辑
    private void HandleShooting()
    {
        if (currentUnit.ifInCombat())
        {
            if (Time.time >= currentUnit.nextFireTime)
            {
                // 1. 生成子弹
                GameObject bulletInstance = Instantiate(currentUnit.bulletPrefab, currentUnit.muzzlePoint.position, currentUnit.muzzlePoint.rotation);
                BulletController bulletScript = bulletInstance.GetComponent<BulletController>();
                if (bulletScript != null)
                {
                    bulletScript.owner = this.gameObject; // 将士兵对象设置为子弹的 owner
                }
                Debug.Log(bulletScript.owner);
                // 2. 计算散布
                Quaternion spreadRotation = Quaternion.Euler(
                    Random.Range(-currentUnit.spreadAngle, currentUnit.spreadAngle),
                    Random.Range(-currentUnit.spreadAngle, currentUnit.spreadAngle),
                    0
                );

                // 3. 应用散布并给予初始速度
                Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Debug.Log("发射子弹");
                    // 结合枪口方向和散布
                    Vector3 shootDirection = currentUnit.muzzlePoint.forward;
                    Vector3 finalDirection = spreadRotation * shootDirection;

                    // 假设子弹速度是 30f
                    rb.velocity = finalDirection * 300f;
                }

                // 4. 设置下次射击时间
                currentUnit.nextFireTime = Time.time + currentUnit.fireRate;
            }
        }
    }*/
    // 受击逻辑
    public void TakeDamage(int damage)
    {
        float actualDamage = Mathf.Max(1, damage - currentUnit.defense);
        currentUnit.currentHealth = Mathf.Max(currentUnit.currentHealth - actualDamage, 0);
        if (UIController.Instance != null)
        {
            // 通知 UI 管理器展示伤害
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

    // 死亡逻辑
    public void Die()
    {
        currentUnit.changeToDead();
        if (GetComponent<UnityEngine.AI.NavMeshAgent>() is UnityEngine.AI.NavMeshAgent navMeshAgent)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
        }
        // 停止该角色的UnitController脚本
        this.enabled = false;
/*        if (GetComponent<SphereCollider>() is SphereCollider sphereCollider)
        {
            // 禁用范围触发器
            sphereCollider.enabled = false;
        }*/
        // 禁用主碰撞体（如果士兵上有其他用于物理碰撞的 Collider）

        // 4. 清理 UI 和模型 (无动画版本)

/*        // 清理血条 UI
        if (transform.Find("HealthBarCanvas") is Transform healthBar)
        {
            Destroy(healthBar.gameObject);
        }*/

/*        // 隐藏模型（作为死亡的视觉反馈）
        if (transform.Find("ToonSoldier_model") is Transform model)
        {
            model.gameObject.SetActive(false);
        }*/

        // 5. 销毁对象
        Destroy(gameObject, 0.5f);
    }
    // 进入战斗逻辑
    /*private void OnTriggerEnter(Collider other)
    {
        // 检查进入的物体是否是敌人（通过 Tag）
        if (other.CompareTag("enemy"))
        {

            // 避免重复添加，虽然 List 通常会处理
            if (!targetsInRange.Contains(other.gameObject))
            {
                targetsInRange.Add(other.gameObject);
                Debug.Log("发现目标：" + other.gameObject.name + "，当前范围内有目标数量: " + targetsInRange.Count);
*//*                Debug.Log(currentUnit.getState());
                // 【核心逻辑：如果范围内部存在敌人，则播放战斗动画】
                if (currentUnit.ifInIdle())
                {
                    Debug.Log("Hey");
                    if (targetsInRange.Count > 0)
                    {
                        currentUnit.changeToCombat();
                        animator.SetBool("InCombat", true);
                        //PlayAttackAnimation();
                    }
                }*//*

            }
        }
    }*/
/*    private void OnTriggerExit(Collider other)
    {
        // 检查离开的物体是否是敌人
        if (other.CompareTag("enemy"))
        {
            if (targetsInRange.Contains(other.gameObject))
            {
                targetsInRange.Remove(other.gameObject);
                Debug.Log("目标离开范围，当前范围内目标数量: " + targetsInRange.Count);

                // 【核心逻辑：如果范围内没有敌人了，则停止战斗动画】
                if (targetsInRange.Count == 0)
                {
                    currentUnit.changeToIdle();
                    animator.SetBool("InCombat", false);
                    //StopAttackAnimation();
                }
            }
        }
    }*/
    // 行进逻辑
    public void MoveTo(Vector3 destination)
    {
        Debug.Log("start_path");
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

    private void movementFinshed()
    {
        // 一开始进入待机状态
        if (currentUnit.ifInMove()) 
        {
            isSelected = false;
            currentUnit.changeToIdle();
        }
        // 取消选择
        
        /*else if (currentUnit.ifInIdle() && targetsInRange.Count > 0)
        {
            // 范围里有敌人！
            currentUnit.changeToCombat();
            animator.SetBool("InCombat", true);

            // 强制停止 NavMeshAgent，避免 Agent 尝试微调位置
            navMeshAgent.ResetPath();
        }
        else if (currentUnit.ifInCombat())
        {
            animator.SetBool("InCombat", true);
        }
        else
        {
            // 强制停止 NavMeshAgent
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

        // 坐标转成棋盘坐标
        int start_x = Mathf.RoundToInt((start.x - gridOrigin.x) / cellSize);
        int start_z = Mathf.RoundToInt((start.z - gridOrigin.z) / cellSize);
        start = new Vector3(start_x, gridOrigin.y, start_z);

        // 将起点加入队列，访问记录，令其距离为0
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
                // 获取其邻域点
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
        // 将起点和终点分别转换为棋盘坐标

        int start_x = Mathf.RoundToInt((start.x - gridOrigin.x) / cellSize);
        int start_z = Mathf.RoundToInt((start.z - gridOrigin.z) / cellSize);
        start = new Vector3(start_x, gridOrigin.y + cellSize, start_z);
        int end_x = Mathf.RoundToInt((end.x - gridOrigin.x) / cellSize);
        int end_z = Mathf.RoundToInt((end.z - gridOrigin.z) / cellSize);
        end = new Vector3(end_x, gridOrigin.y + cellSize, end_z);
        // 将起点加入队列和访问记录
        GridNode startNode = new GridNode(start, null);
        queue.Enqueue(startNode);
        visited.Add(start);

        while (queue.Count > 0) 
        {
            // 遍历吧
            GridNode currentNode = queue.Dequeue();

            // 如果找到终点
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
            // 获取邻域点的网格坐标
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
        Debug.Log("重建路径");
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

    // 选择高亮相关
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
