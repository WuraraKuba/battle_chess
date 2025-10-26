using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets; // 引入 Addressables 命名空间
using UnityEngine.ResourceManagement.AsyncOperations; // 用于异步操作句柄

/// <summary>
/// 主要负责对UnitManager进行处理
/// 加载待选单位，部署啊，消失啊，统计啊什么的
/// </summary>
public class UnitCoreController : MonoBehaviour
{
    public static UnitCoreController Instance { get; private set; }

    // 对象池，用于存储敌人与我方游戏对象的
    [Header("Unit Pools")]
    [SerializeField] public GameObject EnemyUnitsPool;
    [SerializeField] public GameObject OurUnitsPool;
    private int enemyNums;  // 敌人数目
    private int OurNums;    // 我方数目

    [Header("Deploy Settings")]
    [SerializeField] private GameObject unitPrefab; // 要部署的单位 Prefab,目前的情况是只能部署一个，以及一种
    public GameObject deployedUnit = null;
    // 存储加载完成的所有 UnitData
    private List<UnitData> allUnitConfigurations = new List<UnitData>();
    [SerializeField]
    private string unitConfigLabel = "Chess1";


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 加载单位数据
            LoadAllUnitData();
        }
        else
        {
            Destroy(gameObject);
        }

        

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 加载单位资产
    // 初始化
    public void UnitCoreControllerInitialized()
    {
        // 获取Hierarchy中的单位总对象
        GameObject unitManagerObject = GameObject.FindGameObjectWithTag("Unit");
        if (unitManagerObject != null)
        {
            Debug.Log("游戏初始化");
            Transform enemyUnitsPoolTransform = unitManagerObject.transform.Find("EnemyUnitsPool");
            Transform ourUnitPoolTransform = unitManagerObject.transform.Find("OurUnitsPool");
            if (enemyUnitsPoolTransform != null)
            {
                Debug.Log("敌人池初始化");
                EnemyUnitsPool = enemyUnitsPoolTransform.gameObject;
            }
            if (ourUnitPoolTransform != null)
            {
                Debug.Log("友方池初始化");
                OurUnitsPool = ourUnitPoolTransform.gameObject;
            }
        }
    }
    // 加载单位资产
    public void LoadAllUnitData()
    {
        // Addressables.LoadAssetsAsync 方法返回一个异步操作句柄 (AsyncOperationHandle)
        // 它会加载所有匹配标签的 T 类型资产
        AsyncOperationHandle<IList<UnitData>> loadHandle = Addressables.LoadAssetsAsync<UnitData>(
            unitConfigLabel,
            (unitData) =>
            {
                // 可选：加载过程中每加载完一个资产的回调 (例如更新加载条)
                // Debug.Log($"Loaded asset: {unitData.UnitName}");
            }
        );

        // 关键：等待异步操作完成
        loadHandle.Completed += OnUnitDataLoadingComplete;
    }
    // 异步加载完成
    private void OnUnitDataLoadingComplete(AsyncOperationHandle<IList<UnitData>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // 将加载到的资产列表存储起来
            allUnitConfigurations = handle.Result.ToList();

            Debug.Log($"Addressables: 成功加载 {allUnitConfigurations.Count} 个 UnitData 资产。");

            List<string> teamNames = new List<string>();
            foreach (UnitData unitData in allUnitConfigurations)
            {
                teamNames.Add(unitData.name);
            }
            // 把NameList给UI，让其自己去动态生成按钮吧
            // ❗ 可以在这里触发一个事件，通知 UI 系统 "单位数据已准备就绪"
            // 例如: CustomActions.OnUnitDataLoaded?.Invoke(allUnitConfigurations);

            // 示例：将加载到的数据交给 UIManager 动态生成按钮
            // UIManager.Instance.PopulateUnitSelection(allUnitConfigurations);
        }
        else
        {
            Debug.LogError($"Addressables: 加载 UnitData 失败！ 错误: {handle.OperationException}");
        }

        // 释放句柄：尽管 Addressables 会管理内存，但加载 Assets 应该在用完后手动释放。
        // Addressables.Release(handle); 
    }
    /// <summary>
    /// 向UI发送
    /// </summary>
    /// <returns></returns>
    public List<UnitData> getOurTeam()
    {
        List<UnitData> teams = new List<UnitData>();
        foreach (UnitData unitData in allUnitConfigurations)
        {
            teams.Add(unitData);
        }
        return teams;
    }
    // 友方部署
    public void DeployUnit(GameObject unitPrefab, Vector3 deployPosition)
    {
        GameObject enemyPoolParent = OurUnitsPool;
        Transform transform = enemyPoolParent.transform;
        CapsuleCollider collider = unitPrefab.GetComponent<CapsuleCollider>();
        float height = collider.height / 2;
        deployPosition.y += height;
        // 2. 实例化单位
        GameObject newUnit = Instantiate(
                                        unitPrefab
                                        , deployPosition
                                        , Quaternion.identity
                                        , transform);

        // 3. 存储引用
        // 目前以及被部署的单位，虽然通过设置子对象可以方便查询友方数
        // 但是现在的新问题是如果要部署多个单位该如何是好。
        deployedUnit = newUnit;

        Debug.Log($"单位已部署在 {deployPosition}。请点击 '游戏开始' 按钮切换模式。");
    }

    // 友方重新部署
    public void RedeployUnit(Vector3 deployNewPosition)
    {
        // 销毁旧单位
        Destroy(deployedUnit);
        // 部署新单位
        DeployUnit(unitPrefab, deployNewPosition);
    }
    // 敌人生成

    // 获取敌我数目
    private void GetEnemiesNumsFromPool()
    {
        if (EnemyUnitsPool != null)
        {
            enemyNums = EnemyUnitsPool.transform.childCount;
        }
    }

    private void GetOurNumsFromPool()
    {
        if (OurUnitsPool != null)
        {
            OurNums = OurUnitsPool.transform.childCount;
        }

    }

    // 向外界返回敌我数目
    public int GetOurNums()
    {
        GetOurNumsFromPool();
        return OurNums;
    }
    public int GetEnemiesNums()
    {
        GetEnemiesNumsFromPool();
        return enemyNums;
    }
}
