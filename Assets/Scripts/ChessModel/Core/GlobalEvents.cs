using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEvents
{
    public static event System.Action OnAnyUnitDeployed;

    // 公共方法：供底层按钮调用,用来汇报此事件已发生的
    public static void ReportUnitDeployedFinshed()
    {
        OnAnyUnitDeployed?.Invoke();
    }
}
