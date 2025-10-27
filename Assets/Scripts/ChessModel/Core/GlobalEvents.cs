using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEvents
{
    // 部署事件监听
    public static event System.Action OnAnyUnitDeployed;
/*    // 开始游戏事件监听：开始按钮按下
    public static event System.Action StartButtonClick;*/
    // 回合切换事件监听
    public static event System.Action TurnRoundButtonClick;

    // 公共方法：供底层按钮调用,用来汇报此事件已发生的
    public static void ReportUnitDeployedFinshed()
    {
        OnAnyUnitDeployed?.Invoke();
    }

    // 汇报回合切换
    public static void ReportTurnRoundButtonClick() 
    {
        TurnRoundButtonClick?.Invoke();
    }
}
