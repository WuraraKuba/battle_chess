using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  // Button要用到

public class UnitSkillButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    // 存储它代表的单位对象
    public Unit associatedUnit;
    // 存储对象所进行的操作
    public BattleController associatedBattleController;


    // 初始化按钮（由 SkillCommandPanelManager 调用）
    public void Initialize(Unit unitData)
    {
        BattleController battleController = unitData.GetFlatmateBattleController();
        associatedBattleController = battleController;
        associatedUnit = unitData;

        buttonText.text = unitData.chessName;


        Button buttonComponent = GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.interactable = true;
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(OnSkillButtonClicked);
        }
    }

    private void OnSkillButtonClicked()
    {
        if (associatedBattleController != null)
        {
            Debug.Log($"指挥 {associatedBattleController.gameObject.name} 激活技能！");
            // 调用 UnitController 的方法
            associatedBattleController.ActivateSkill();

            // 关闭子弹时间 (假设 TimeWarpCommandController 是单例)
            // if (TimeWarpCommandController.Instance != null)
            // {
            //     TimeWarpCommandController.Instance.SetTimeWarpActive(false); 
            // }
        }
    }

}
