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
        // 部署确认按钮
        deployButton.onClick.AddListener(OnDeployClicked);

        deployButton.interactable = true;
    }
    private void OnDeployClicked()
    {
        // 1. 通知主控脚本：我们准备部署了
        OnDeployStartClicked?.Invoke();

        Debug.Log($"MapUIController：已通知开始部署流程。");

        // 2. 隐藏 UI，准备接收地图点击
        // 这一步最好由主控脚本来做，但 UI 确实需要先隐藏
        gameObject.SetActive(false);
    }
}
