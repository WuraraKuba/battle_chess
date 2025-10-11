using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCommandPanelManager : MonoBehaviour
{
    [Header("��̬ UI ����")]
    public GameObject unitButtonPrefab; // ���� UnitSkillButton.prefab
    public LayoutGroup layoutGroup;     // ���� Horizontal Layout Group ���

    private List<GameObject> activeButtons = new List<GameObject>();

    // ��TODO������Ҫһ����������ȡ�����ѷ���λ
    // �������� BattleController ����һ����̬�б� UnitController.PlayerUnits

    public void ShowUnitButtons(List<Unit> playerUnits)
    {
        // 1. ����ɰ�ť
        ClearButtons();

        if (playerUnits == null || playerUnits.Count == 0) return;

        // 2. Ϊÿ����λ���ɰ�ť
        foreach (Unit unit in playerUnits)
        {
            GameObject newButtonObj = Instantiate(unitButtonPrefab, transform);
            activeButtons.Add(newButtonObj);

            // 3. ��ʼ����ť���� (�����ı�)
            UnitSkillButton unitButton = newButtonObj.GetComponent<UnitSkillButton>();
            if (unitButton != null)
            {
                unitButton.Initialize(unit);
            }
        }
        // ������� (�����屾�� UIController �����ˣ��ⲽ����ʡ��)
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
