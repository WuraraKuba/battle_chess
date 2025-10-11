using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [Header("�˺���ʾ")]
    public GameObject damageTextPrefab; 

    [Header("ȫ��ʱ��Ť�� UI")]
    public GameObject timeWarpOverlayPanel;

    [Header("������ȡ��ť��")]
    public SkillCommandPanelManager skillPanelManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ȷ�������ᱻ����
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

    // �� UnitController ���õĺ��Ĺ�������
    public void DisplayDamage(float damage, Vector3 worldPosition)
    {
        if (damageTextPrefab == null) return;
        // 1. ���� UI ��ʽ���߼����� UIController��
        Color color = (damage > 50) ? Color.yellow : Color.white;
        // 2. ʵ��������λ������ View �Ĵ�����
        GameObject textInstance = Instantiate(
            damageTextPrefab,
            worldPosition + Vector3.up * 24f, // ΢������λͷ��
            Quaternion.identity
        );

        // 3. �����ݴ��ݸ� View �������� View �ϵĳ�ʼ��������
        FloatingTextIndicator indicator = textInstance.GetComponent<FloatingTextIndicator>();
        if (indicator != null)
        {
            string damageString = damage.ToString();
            indicator.Initialize(damageString, color);
        }
    }

    public void SetTimeWarpUIVisibility(bool isVisible)
    {
        // 1. ����ȫ�ְ�͸�����
        if (timeWarpOverlayPanel != null)
        {
            timeWarpOverlayPanel.SetActive(isVisible);
        }

        if (skillPanelManager != null)
        {
            if (isVisible)
            {
                // TODO: ������һ���ط����Ի�ȡ�����ѷ���λ�б�
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
        // �����ʵ��ȡ���������Ϸ�ṹ��
        // ʾ�������������ѷ���λ���� "PlayerUnit" ��ǩ
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
