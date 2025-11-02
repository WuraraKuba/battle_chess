using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets; // 引入 Addressables 命名空间
using UnityEngine.ResourceManagement.AsyncOperations; // 用于异步操作句柄

/// <summary>
/// 主要负责对UnitManager进行处理
/// 加载待选单位，部署啊，消失啊，统计啊什么的
/// 状态切换啥的也要
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
    // 存储加载完成的所有 UnitData
    private List<UnitData> allUnitConfigurations = new List<UnitData>();
    private List<UnitData> allEnemyConfigurations = new List<UnitData>();
    [SerializeField]
    private string meUnitConfigLabel = "Chess";
    private string enemyConfigLabel = "EnemyChess";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 加载单位数据
            LoadUnitData(meUnitConfigLabel, allUnitConfigurations);
            LoadUnitData(enemyConfigLabel, allEnemyConfigurations);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // 加载单位资产
    // 初始化
    public void UnitCoreControllerInitialized()
    {
        // 获取Hierarchy中的单位总对象
        GameObject unitManagerObject = GameObject.FindGameObjectWithTag("Unit");
        if (unitManagerObject != null)
        {
            Transform enemyUnitsPoolTransform = unitManagerObject.transform.Find("EnemyUnitsPool");
            Transform ourUnitPoolTransform = unitManagerObject.transform.Find("OurUnitsPool");
            if (enemyUnitsPoolTransform != null)
            {
                EnemyUnitsPool = enemyUnitsPoolTransform.gameObject;
            }
            if (ourUnitPoolTransform != null)
            {
                OurUnitsPool = ourUnitPoolTransform.gameObject;
            }
        }
    }
    // 加载单位资产
    public void LoadUnitData(string label, List<UnitData> targetList)
    {
        // Addressables.LoadAssetsAsync 方法返回一个异步操作句柄 (AsyncOperationHandle)
        // 它会加载所有匹配标签的 T 类型资产
        AsyncOperationHandle<IList<UnitData>> loadHandle = Addressables.LoadAssetsAsync<UnitData>(
            label,
            (unitData) =>
            {
                // 可选：加载过程中每加载完一个资产的回调 (例如更新加载条)
                // Debug.Log($"Loaded asset: {unitData.UnitName}");
            }
        );
        loadHandle.Completed += (handle) =>
        {
            // 调用通用的完成处理方法，并将捕获的变量传入
            OnLoadingComplete(handle, targetList);
        };
        // 关键：等待异步操作完成
        /*loadHandle.Completed += OnUnitDataLoadingComplete;*/
    }
    // 异步加载完成
    private void OnLoadingComplete(AsyncOperationHandle<IList<UnitData>> handle, List<UnitData> targetList)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // 将加载到的资产列表存储起来
            targetList.Clear(); // 确保清空旧数据
            targetList.AddRange(handle.Result);

            Debug.Log($"Addressables: 成功加载 {targetList.Count} 个 UnitData 资产。");

            List<string> teamNames = new List<string>();
            foreach (UnitData unitData in targetList)
            {
                teamNames.Add(unitData.name);
            }
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
    // 单位部署
    public void DeployUnit(UnitData unitData)
    {
        GameObject poolParent;
        if (!unitData.isEnemy) poolParent = OurUnitsPool;
        else poolParent = EnemyUnitsPool;
        Transform transform = poolParent.transform;
        GameObject unitPrefab = unitData.gameObject;
        CapsuleCollider collider = unitPrefab.GetComponent<CapsuleCollider>();
        float height = collider.height / 2;
        Vector3 deployPosition = unitData.UnitLocation;
        deployPosition.y += height;
        // 2. 实例化单位
        GameObject newUnit = Instantiate(
                                        unitPrefab
                                        , deployPosition
                                        , Quaternion.identity
                                        , transform);

        // 3. 存储引用
        UnitComponent unitComponent = newUnit.GetComponent<UnitComponent>();
        unitComponent.Initialize(unitData);
    }

/*    // 友方重新部署
    public void RedeployUnit(Vector3 deployNewPosition)
    {
        // 销毁旧单位
        Destroy(deployedUnit);
        // 部署新单位
        DeployUnit(unitPrefab, deployNewPosition);
    }*/
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
