using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于保存鼠标操作相关的变量
/// </summary>
public class SelectionContext : MonoBehaviour
{
    public static SelectionContext Instance;
    // 目前鼠标所选择的单位
    public GameObject selectedUnit = null;
    // 加个？就能让这个数据结构能被赋值为null了
    // 目前的起点和终点
    private Vector3? startPosition = null;
    private Vector3? endPosition = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 确保它不会被销毁
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
