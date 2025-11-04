using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 使用UnitData那个类进行部署的话只用上了一个prefab，需要通过这个类把其他信息给注入到prefab里
/// </summary>
public class UnitComponent : MonoBehaviour
{
    // 暂时只弄个位置
    public Vector3 UnitLocation;
    public int AP; // 行动力
    // 加个是否是敌人的判断
    public bool isEnemy;
    public void Initialize(UnitData data)
    {
        // 确保 UnitData 不是空，以防万一
        if (data == null) return;

        // 1. 从数据资产中获取属性
        UnitLocation = data.UnitLocation;
        AP = data.AP;
        // 3. (可选) 可以在这里设置单位的视觉或行为逻辑
        Debug.Log($"单位初始化成功");
    }

    public Vector3 GetLocation()
    {
        return UnitLocation;
    }
    public void ChangeLocation(Vector3 location)
    {
        UnitLocation = location;
    }
}

