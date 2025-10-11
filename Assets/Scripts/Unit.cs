using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 存储各种单位的属性
public class Unit : MonoBehaviour
{
    [Header("基础属性")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 1.8f, 0); // 调整 Y 值，让血条位于士兵头顶
    public float maxHealth = 100;
    public float currentHealth;
    public string chessName;

    [Header("战斗属性")]
    public bool autoFire = false; 
    public int attackDamage = 50;
    public int defense = 10;
    public float fireRate = 0.1f;
    public float nextFireTime;
    public float spreadAngle = 1f; // 子弹最大散布角度
    public GameObject bulletPrefab;  // 子弹对象
    public Transform muzzlePoint;  // 子弹出发点

    [Header("移动属性")]
    public int movementRange = 1;


    // 单位状态
    public enum UnitState
    {
        Idle,  // 空闲
        Moving, // 移动状态  高优先级
        Attacking,  // 战斗状态  次优先级
        Dead        // 死亡状态
    }
    [Header("当前状态")]
    public UnitState state = UnitState.Idle;  // 初始状态永远为待机状态


    public float CurrentHealthFloat { get { return currentHealth; } } // 转换为 float
    public float MaxHealthFloat { get { return maxHealth; } }       // 转换为 float
    // 初始化选择状态
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
