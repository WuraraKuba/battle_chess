using Cinemachine;
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
    // ����ֱ�Ӱ��µ����
    // �����Ҫ�����׵��������Ҫ�Ȱ���ť����ʱʱ�����ɼ�������UI��ʧ��ʹ���ܹ�ѡ��Ͷ��λ��
    private void OnSkillButtonClicked()
    {
        if (associatedUnit == null) return;
        if (UIController.Instance != null)
        {
            // 1. �ر�ʱ����� UI 
            UIController.Instance.SetTimeWarpUIVisibility(false);
        }
        // ��õ�ǰѡ���ɫ��λ��
        Vector3 startLoc = associatedUnit.transform.position;
        GameObject SkillObject = associatedUnit.grenade.gameObject;
        // ��ȡ������ߵ�ǰλ��
        Debug.Log("���뼼���߼�" + startLoc);
        if (CommandController.Instance != null) 
        {
            CommandController.Instance.GetSkillLoc(startLoc, SkillObject);
        }

    }

    // ��ʼ����ť���� SkillCommandPanelManager ���ã�
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
