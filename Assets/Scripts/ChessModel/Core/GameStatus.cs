using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ڱ�����Ϸ״̬��
/// </summary>
public enum GameStatus
{
    BeforeGame,  // ��Ϸǰ
    InGameMe,      // �ҷ��غ�  -���start��ť������Ϸ��״̬
    InGameEnemy,   // �з��غ�
    Victory,     // �������޵���
    Failure      // ���������ҷ���λ
}
