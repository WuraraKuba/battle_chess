using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingTextIndicator : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    // ���ã������ٶȺʹ��ʱ��
    public float lifetime = 1f;
    public float moveSpeed = 0.8f;

    // ���󻯵ĳ�ʼ�������������κ��ַ�������ɫ
    public void Initialize(string content, Color textColor)
    {
        // 1. �������ݺ���ʽ
        textComponent.text = content;
        textComponent.color = textColor;

        // 2. �����Ի�ʱ��
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 3. �������������ϸ���
        Debug.Log("��������");
        transform.position += Vector3.up * Time.deltaTime * moveSpeed;

        // ǿ����������
        transform.rotation = Camera.main.transform.rotation;
    }
}
