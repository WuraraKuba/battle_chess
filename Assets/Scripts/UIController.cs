using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [Header("UI Prefabs")]
    public GameObject damageTextPrefab; // ����������� DamageIndicator �ű��� Prefab

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
}
