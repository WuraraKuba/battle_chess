using Cinemachine;
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
    // 处理直接按下的情况
    // 针对需要扔手雷的情况，需要先按按钮，此时时间依旧减缓，但UI消失，使得能够选择投放位置
    private void OnSkillButtonClicked()
    {
        if (associatedUnit == null) return;
        if (UIController.Instance != null)
        {
            // 1. 关闭时间减缓 UI 
            UIController.Instance.SetTimeWarpUIVisibility(false);
        }
        // 获得当前选择角色的位置
        Vector3 startLoc = associatedUnit.transform.position;
        GameObject SkillObject = associatedUnit.grenade.gameObject;
        // 获取鼠标射线当前位置
        Debug.Log("进入技能逻辑" + startLoc);
        if (CommandController.Instance != null) 
        {
            CommandController.Instance.GetSkillLoc(startLoc, SkillObject);
        }

    }

    // 初始化按钮（由 SkillCommandPanelManager 调用）
    public void Initialize(Unit unitData)
    {
        BattleController battleController = unitData.GetFlatmateBattleController();
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

}
