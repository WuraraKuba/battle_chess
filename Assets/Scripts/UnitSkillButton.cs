using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  // ButtonҪ�õ�

public class UnitSkillButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    // �洢������ĵ�λ����
    public Unit associatedUnit;
    // �洢���������еĲ���
    public BattleController associatedBattleController;


    // ��ʼ����ť���� SkillCommandPanelManager ���ã�
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
            Debug.Log($"ָ�� {associatedBattleController.gameObject.name} ����ܣ�");
            // ���� UnitController �ķ���
            associatedBattleController.ActivateSkill();

            // �ر��ӵ�ʱ�� (���� TimeWarpCommandController �ǵ���)
            // if (TimeWarpCommandController.Instance != null)
            // {
            //     TimeWarpCommandController.Instance.SetTimeWarpActive(false); 
            // }
        }
    }

}
