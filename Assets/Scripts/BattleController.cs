using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unit;

public class BattleController : MonoBehaviour
{
    // �洢����
    private List<GameObject> targetsInRange = new List<GameObject>();
    private Animator animator;
    private Unit currentUnit;


    [Header("Audio Dependencies")]
    public AudioSource weaponSingleSource; // ����ģʽ�õĳ�פ AudioSource ���

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
        // 1. ȷ���ǵ��ˣ��ҵ�ǰ״̬Ϊ Combat
        if (currentUnit.ifInIdle())
        {
            // ÿ���ƶ�����������
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
                    Debug.Log("����" + other.gameObject);
                }
            }
            // ����Ƿ��е��ˣ������˴���targetsInRange
            
            // ������˵���Ŀ����0����״̬ת��Ϊս��״̬
            if (targetsInRange.Count > 0)
            {
                currentUnit.changeToCombat();
            }
        }
        else if (currentUnit.ifInCombat())
        {
            // ����е��ˣ�����ս������
            if (targetsInRange.Count > 0)
            {
                // �����һ��Ŀ��
                Vector3 lookAtPos = targetsInRange[0].transform.position;
                // ���� Y ��Ӱ�죬ֻ��ˮƽ����ת

                lookAtPos.y = transform.parent.position.y;

                // ƽ��ת��Ŀ�� (Slerp �� Lerp)
                Quaternion targetRotation = Quaternion.LookRotation(lookAtPos - transform.parent.position);
                transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, targetRotation, Time.deltaTime * 10f); // 10f ����ת�ٶ�
                // ����
                HandleShooting();
                animator.SetBool("InCombat", true);
            }
            // ���û�е��ˣ��������״̬
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
                    // ����ǹ��λ�á�autoFire״̬�͵���AudioSource���
                    AudioManager.Instance.PlayGunShotSound(
                        currentUnit.muzzlePoint.position,
                        currentUnit.autoFire,
                        weaponSingleSource
                    );
                }
                // 1. �����ӵ�
                GameObject bulletInstance = Instantiate(currentUnit.bulletPrefab, currentUnit.muzzlePoint.position, currentUnit.muzzlePoint.rotation);
                BulletController bulletScript = bulletInstance.GetComponent<BulletController>();
                if (bulletScript != null)
                {
                    bulletScript.owner = this.gameObject; // ��ʿ����������Ϊ�ӵ��� owner
                }
                // 2. ����ɢ��
                Quaternion spreadRotation = Quaternion.Euler(
                    Random.Range(-currentUnit.spreadAngle, currentUnit.spreadAngle),
                    Random.Range(-currentUnit.spreadAngle, currentUnit.spreadAngle),
                    0
                );

                // 3. Ӧ��ɢ���������ʼ�ٶ�
                Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // ���ǹ�ڷ����ɢ��
                    Vector3 shootDirection = currentUnit.muzzlePoint.forward;
                    Vector3 finalDirection = spreadRotation * shootDirection;

                    // �����ӵ��ٶ��� 30f
                    rb.velocity = finalDirection * 300f;
                }

                // 4. �����´����ʱ��
                currentUnit.nextFireTime = Time.time + currentUnit.fireRate;
            }
        }
    }

    public void ActivateSkill()
    {
        Debug.Log("Fire");
    }



}
