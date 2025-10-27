using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEvents
{
    // �����¼�����
    public static event System.Action OnAnyUnitDeployed;
/*    // ��ʼ��Ϸ�¼���������ʼ��ť����
    public static event System.Action StartButtonClick;*/
    // �غ��л��¼�����
    public static event System.Action TurnRoundButtonClick;

    // �������������ײ㰴ť����,�����㱨���¼��ѷ�����
    public static void ReportUnitDeployedFinshed()
    {
        OnAnyUnitDeployed?.Invoke();
    }

    // �㱨�غ��л�
    public static void ReportTurnRoundButtonClick() 
    {
        TurnRoundButtonClick?.Invoke();
    }
}
