using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingTextIndicator : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    // 配置：浮动速度和存活时间
    public float lifetime = 1f;
    public float moveSpeed = 0.8f;

    // 抽象化的初始化函数：接收任何字符串和颜色
    public void Initialize(string content, Color textColor)
    {
        // 1. 设置内容和样式
        textComponent.text = content;
        textComponent.color = textColor;

        // 2. 设置自毁时间
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 3. 驱动动画：向上浮动
        Debug.Log("来动画吧");
        transform.position += Vector3.up * Time.deltaTime * moveSpeed;

        // 强制面对摄像机
        transform.rotation = Camera.main.transform.rotation;
    }
}
