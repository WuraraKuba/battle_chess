using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [Header("UI Prefabs")]
    public GameObject damageTextPrefab; // 必须拖入带有 DamageIndicator 脚本的 Prefab

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 确保它不会被销毁
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 供 UnitController 调用的核心公共方法
    public void DisplayDamage(float damage, Vector3 worldPosition)
    {
        if (damageTextPrefab == null) return;
        // 1. 决定 UI 样式（逻辑属于 UIController）
        Color color = (damage > 50) ? Color.yellow : Color.white;
        // 2. 实例化并定位（负责 View 的创建）
        GameObject textInstance = Instantiate(
            damageTextPrefab,
            worldPosition + Vector3.up * 24f, // 微调到单位头上
            Quaternion.identity
        );

        // 3. 将数据传递给 View 自身（调用 View 上的初始化函数）
        FloatingTextIndicator indicator = textInstance.GetComponent<FloatingTextIndicator>();
        if (indicator != null)
        {
            string damageString = damage.ToString();
            indicator.Initialize(damageString, color);
        }
    }
}
