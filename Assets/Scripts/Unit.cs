using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �洢���ֵ�λ������
public class Unit : MonoBehaviour
{
    [Header("��������")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 1.8f, 0); // ���� Y ֵ����Ѫ��λ��ʿ��ͷ��
    public float maxHealth = 100;
    public float currentHealth;
    public string chessName;

    [Header("ս������")]
    public bool autoFire = false; 
    public int attackDamage = 50;
    public int defense = 10;
    public float fireRate = 0.1f;
    public float nextFireTime;
    public float spreadAngle = 1f; // �ӵ����ɢ���Ƕ�
    public GameObject bulletPrefab;  // �ӵ�����
    public Transform muzzlePoint;  // �ӵ�������

    [Header("�ƶ�����")]
    public int movementRange = 1;


    // ��λ״̬
    public enum UnitState
    {
        Idle,  // ����
        Moving, // �ƶ�״̬  �����ȼ�
        Attacking,  // ս��״̬  �����ȼ�
        Dead        // ����״̬
    }
    [Header("��ǰ״̬")]
    public UnitState state = UnitState.Idle;  // ��ʼ״̬��ԶΪ����״̬


    public float CurrentHealthFloat { get { return currentHealth; } } // ת��Ϊ float
    public float MaxHealthFloat { get { return maxHealth; } }       // ת��Ϊ float
    // ��ʼ��ѡ��״̬
    public UnitState getState()
    {
        return state;
    }

    public bool ifInMove()
    {
        return state==UnitState.Moving;
    }

    public bool ifInIdle()
    {
        return state == UnitState.Idle;
    }

    public bool ifInCombat()
    {
        return state == UnitState.Attacking;
    }

    public void changeToCombat()
    {
        state = UnitState.Attacking;
    }


    public void changeToMove()
    {
        state = UnitState.Moving;
    }
    public void changeToIdle()
    {
        state = UnitState.Idle;
    }

    public void changeToDead()
    {
        state = UnitState.Dead;
    }



}
