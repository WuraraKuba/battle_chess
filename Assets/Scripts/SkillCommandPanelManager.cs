using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCommandPanelManager : MonoBehaviour
{
    [Header("动态 UI 引用")]
    public GameObject unitButtonPrefab; // 拖入 UnitSkillButton.prefab
    public LayoutGroup layoutGroup;     // 拖入 Horizontal Layout Group 组件

    private List<GameObject> activeButtons = new List<GameObject>();

    // 【TODO】你需要一个方法来获取所有友方单位
    // 假设你在 BattleController 中有一个静态列表 UnitController.PlayerUnits

    public void ShowUnitButtons(List<Unit> playerUnits)
    {
        // 1. 清理旧按钮
        ClearButtons();

        if (playerUnits == null || playerUnits.Count == 0) return;

        // 2. 为每个单位生成按钮
        foreach (Unit unit in playerUnits)
        {
            GameObject newButtonObj = Instantiate(unitButtonPrefab, transform);
            activeButtons.Add(newButtonObj);

            // 3. 初始化按钮数据 (设置文本)
            UnitSkillButton unitButton = newButtonObj.GetComponent<UnitSkillButton>();
            if (unitButton != null)
            {
                unitButton.Initialize(unit);
            }
        }
        // 激活面板 (如果面板本身被 UIController 隐藏了，这步可以省略)
        gameObject.SetActive(true);
    }

    public void ClearButtons()
    {
        foreach (GameObject button in activeButtons)
        {
            Destroy(button);
        }
        activeButtons.Clear();
        gameObject.SetActive(false);
    }
}
