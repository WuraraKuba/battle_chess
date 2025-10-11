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


    // 初始化按钮（由 SkillCommandPanelManager 调用）
    public void Initialize(Unit unit)
    {
        associatedUnit = unit;
        buttonText.text = unit.chessName;

        // 暂时关闭按钮的交互性，直到选择目标
        GetComponent<Button>().interactable = false;

        // 【TODO】添加按钮点击事件监听
    }

}
