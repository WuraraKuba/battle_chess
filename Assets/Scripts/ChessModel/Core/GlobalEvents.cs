using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEvents
{
    public static event System.Action OnAnyUnitDeployed;

    // �������������ײ㰴ť����,�����㱨���¼��ѷ�����
    public static void ReportUnitDeployedFinshed()
    {
        OnAnyUnitDeployed?.Invoke();
    }
}
