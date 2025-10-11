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


    // ��ʼ����ť���� SkillCommandPanelManager ���ã�
    public void Initialize(Unit unit)
    {
        associatedUnit = unit;
        buttonText.text = unit.chessName;

        // ��ʱ�رհ�ť�Ľ����ԣ�ֱ��ѡ��Ŀ��
        GetComponent<Button>().interactable = false;

        // ��TODO����Ӱ�ť����¼�����
    }

}
