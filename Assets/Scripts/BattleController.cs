using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unit;

public class BattleController : MonoBehaviour
{
    // 存储敌人
    private List<GameObject> targetsInRange = new List<GameObject>();
    private Animator animator;
    private Unit currentUnit;


    [Header("Audio Dependencies")]
    public AudioSource weaponSingleSource; // 单发模式用的常驻 AudioSource 组件

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        currentUnit = GetComponentInParent<Unit>();
    }
    private void CleanTargetList()
    {
        targetsInRange.RemoveAll(target => target == null);
    }

    private void OnTriggerStay(Collider other)
    {
        CleanTargetList();
        // 1. 确保是敌人，且当前状态为 Combat
        if (currentUnit.ifInIdle())
        {
            // 每次移动后，重新索敌
            targetsInRange.Clear();
            string currentTag = transform.parent.gameObject.tag;
            if (currentTag == "enemy")
            {
                if (other.CompareTag("PlayerUnit"))
                {
                    targetsInRange.Add(other.gameObject);
                }
            }
            if (currentTag == "PlayerUnit")
            {
                if (other.CompareTag("enemy"))
                {
                    targetsInRange.Add(other.gameObject);
                    Debug.Log("敌人" + other.gameObject);
                }
            }
            // 检测是否有敌人，将敌人存入targetsInRange
            
            // 如果敌人的数目大于0，将状态转换为战斗状态
            if (targetsInRange.Count > 0)
            {
                currentUnit.changeToCombat();
            }
        }
        else if (currentUnit.ifInCombat())
        {
            // 如果有敌人，播放战斗动画
            if (targetsInRange.Count > 0)
            {
                // 面向第一个目标
                Vector3 lookAtPos = targetsInRange[0].transform.position;
                // 消除 Y 轴影响，只在水平面旋转

                lookAtPos.y = transform.parent.position.y;

                // 平滑转向目标 (Slerp 或 Lerp)
                Quaternion targetRotation = Quaternion.LookRotation(lookAtPos - transform.parent.position);
                transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, targetRotation, Time.deltaTime * 10f); // 10f 是旋转速度
                // 攻击
                HandleShooting();
                animator.SetBool("InCombat", true);
            }
            // 如果没有敌人，进入待机状态
            else
            {
                animator.SetBool("InCombat", false);
                currentUnit.changeToIdle();
            }
        }
    }

    private void HandleShooting()
    {
        if (currentUnit.ifInCombat())
        {
            if (Time.time >= currentUnit.nextFireTime)
            {
                if (AudioManager.Instance != null)
                {
                    // 传入枪口位置、autoFire状态和单发AudioSource组件
                    AudioManager.Instance.PlayGunShotSound(
                        currentUnit.muzzlePoint.position,
                        currentUnit.autoFire,
                        weaponSingleSource
                    );
                }
                // 1. 生成子弹
                GameObject bulletInstance = Instantiate(currentUnit.bulletPrefab, currentUnit.muzzlePoint.position, currentUnit.muzzlePoint.rotation);
                BulletController bulletScript = bulletInstance.GetComponent<BulletController>();
                if (bulletScript != null)
                {
                    bulletScript.owner = this.gameObject; // 将士兵对象设置为子弹的 owner
                }
                // 2. 计算散布
                Quaternion spreadRotation = Quaternion.Euler(
                    Random.Range(-currentUnit.spreadAngle, currentUnit.spreadAngle),
                    Random.Range(-currentUnit.spreadAngle, currentUnit.spreadAngle),
                    0
                );

                // 3. 应用散布并给予初始速度
                Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // 结合枪口方向和散布
                    Vector3 shootDirection = currentUnit.muzzlePoint.forward;
                    Vector3 finalDirection = spreadRotation * shootDirection;

                    // 假设子弹速度是 30f
                    rb.velocity = finalDirection * 300f;
                }

                // 4. 设置下次射击时间
                currentUnit.nextFireTime = Time.time + currentUnit.fireRate;
            }
        }
    }

    public void ActivateSkill()
    {
        Debug.Log("Fire");
    }



}
