using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : IInputHandler
{
    // 鼠标悬停
    public void HandleHover(Vector3 hexPosition, GameObject hitObject)
    {
        Debug.Log("WTF");
        // 高亮, 将当前hitObject加入临时高亮对象中
        MainRenderController.Instance.MapHexCellHighLight(hexPosition, hitObject);
    }
    // 鼠标左点击
    public void HandleLeftClick(Vector3 hexPosition, GameObject hitObject)
    {
        // 选中当前的目标
    }
    // 鼠标右点击
    public void HandleRightClick()
    {
        // 清除当前高亮列表
    }
}
