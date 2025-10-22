using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUIController : MonoBehaviour
{
    // 
    [Header("Deploy Button")]
    [SerializeField] private Button deployButton;
    public Action OnDeployStartClicked;

    private void Awake()
    {
        // ����ȷ�ϰ�ť
        deployButton.onClick.AddListener(OnDeployClicked);

        deployButton.interactable = true;
    }
    private void OnDeployClicked()
    {
        // 1. ֪ͨ���ؽű�������׼��������
        OnDeployStartClicked?.Invoke();

        Debug.Log($"MapUIController����֪ͨ��ʼ�������̡�");

        // 2. ���� UI��׼�����յ�ͼ���
        // ��һ����������ؽű��������� UI ȷʵ��Ҫ������
        gameObject.SetActive(false);
    }
}
