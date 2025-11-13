using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 定义鼠标各种输入模式下的响应操作
public interface IInputHandler
{
    // 鼠标悬停时的逻辑 (例如：高亮)
    void HandleHover(Vector3 hexPosition, GameObject hitObject);

    // 处理鼠标左键点击时的逻辑 (例如：选择单位、确认移动)
    void HandleLeftClick(GameObject hitObject);

    // 处理鼠标右键点击时的逻辑 (例如：取消选择、清除高亮)
    void HandleRightClick();


}
