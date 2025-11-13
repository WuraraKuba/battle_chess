using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputHandler : IInputHandler
{
    private readonly MouseEntity _context;
    public InputHandler(MouseEntity context)
    {
        _context = context; 
    }
    // 鼠标悬停
    public void HandleHover(Vector3 hexPosition, GameObject hitObject)
    {
        // 高亮, 将当前hitObject加入临时高亮对象中
        MainRenderController.Instance.MapHexCellHighLight(hexPosition, hitObject);
    }
    // 鼠标左点击
    public void HandleLeftClick(GameObject hitObject)
    {
        if (_context.startPosition != null)
        {
            Debug.Log("选择当前单位，开始移动");
            _context.endPosition = _context.mousePosition;
            if (!GridMapController.Instance.TargetLocReachedAble(_context.endPosition.Value, _context.startPosition.Value, 3.0f))
            {
                _context.endPosition = null;
                return;
            }
            // 清除高亮
            MainRenderController.Instance.ClearMoveRangeHighlights();
            // 此时这个位置将是终点


            // 根据起点与终点，算出路径吧
            List<Vector3> path = GridMapController.Instance.FindPath(_context.startPosition.Value, _context.endPosition.Value);
            _context.startPosition = null;
            _context.endPosition = null;
            // 但在此之前，先把起始点的渲染做好
            UnitMapMovementController.Instance.StartMovement(_context.selectedUnit, path);
        }
        else
        {
            if (hitObject.layer == 8) // 如果选择的是单位才处理
            {
                Debug.Log("选择当前单位ing");
                _context.selectedUnit = hitObject;
                UnitComponent unitComponent = hitObject.GetComponent<UnitComponent>();
                _context.startPosition = unitComponent.GetLocation();
                Vector3Int startIndex = GridMapController.Instance.Position2HexIndex(_context.startPosition.Value);
                // 测试洪泛算法
                GridMapController.Instance.UnitMovementRange(startIndex, unitComponent.AP);
            }
        }
        
        // 选中当前的目标
    }
    // 鼠标右点击
    public void HandleRightClick()
    {
        // 清除当前高亮列表
    }
}
