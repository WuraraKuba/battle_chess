using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEntity
{
    // 依旧还是射线检测
    public int RaycastIgnoreLayerMask;  // 射线检测需要忽略的
    public Vector3 mousePosition;

    public Vector3 tempPosition;  // 用于存临时位置的坐标
    public Vector3 lastPosition;  // 上一个临时位置的坐标
    public Vector3 lastCellPosition = Vector3.zero;  // 用于存储上次选择的单位坐标

    public GameObject selectedUnit = null;
    // 加个？就能让这个数据结构能被赋值为null了
    public Vector3? startPosition = null;
    public Vector3? endPosition = null;

    private float yOffset;

    public MouseEntity(int RaycastIgnoreLayerMask)
    {
        this.RaycastIgnoreLayerMask = RaycastIgnoreLayerMask;
    }


}
