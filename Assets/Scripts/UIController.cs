using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [Header("伤害显示")]
    public GameObject damageTextPrefab; 

    [Header("全局时间扭曲 UI")]
    public GameObject timeWarpOverlayPanel;

    [Header("用来调取按钮的")]
    public SkillCommandPanelManager skillPanelManager;

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
        if (timeWarpOverlayPanel != null)
        {
            timeWarpOverlayPanel.SetActive(false);
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

    public void SetTimeWarpUIVisibility(bool isVisible)
    {
        // 1. 控制全局半透明面板
        if (timeWarpOverlayPanel != null)
        {
            timeWarpOverlayPanel.SetActive(isVisible);
        }

        if (skillPanelManager != null)
        {
            if (isVisible)
            {
                // TODO: 假设有一个地方可以获取所有友方单位列表
                List<Unit> playerUnits = GetPlayerUnitsList();
                skillPanelManager.ShowUnitButtons(playerUnits);
            }
            else
            {
                skillPanelManager.ClearButtons();
            }
        }
    }

    private List<Unit> GetPlayerUnitsList()
    {
        // 这里的实现取决于你的游戏结构。
        // 示例：假设所有友方单位都有 "PlayerUnit" 标签
        List<Unit> units = new List<Unit>();
        GameObject[] unitObjects = GameObject.FindGameObjectsWithTag("PlayerUnit");

        foreach (GameObject obj in unitObjects)
        {
            Unit unit = obj.GetComponent<Unit>();
            if (unit != null)
            {
                units.Add(unit);
            }
        }
        return units;
    }
}
