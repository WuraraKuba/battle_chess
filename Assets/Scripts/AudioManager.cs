using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // --- 1. ����ģʽ ---
    // ȷ��ֻ��һ������
    public static AudioManager Instance;

    // ��Ч���
    private bool useSingleSource;
    [Header("Weapon Audio")]
    public AudioClip gunShotClip;        // ǹ���ļ� (AudioClip)
    [Tooltip("��������ģʽ����Ҫ����һ�� AudioSource Prefab")]
    public AudioSource audioSourcePrefab; // ��������ʱʵ������ AudioSource Prefab
    [Tooltip("���ڵ���ģʽ����Ҫ�����ɫ���ϵ� AudioSource ���")]
    public AudioSource singleAudioSource; // ��ɫ���ϵ� AudioSource ����

    public Vector2 audioPitch = new Vector2(0.9f, 1.1f); // ������߷�Χ

    [Header("Parent Object")]
    public Transform sfxGunParent;

    private void Awake()
    {
        // ȷ��ֻ��һ�� AudioManager ʵ������
        if (Instance == null)
        {
            Instance = this;
            // ������������ڳ����л�ʱ�������٣�����������У�
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayGunShotSound(Vector3 position, bool isAutoFire, AudioSource singleSource)
    {
        if (gunShotClip == null)
        {
            Debug.LogWarning("GunShotClip is missing on AudioManager.");
            return;
        }

        // �� isAutoFire �滻 useSingleSource
        if (!isAutoFire)
        {
            // === ģʽһ����Դ���� (�ʺϵ���/��������) ===
            // �������ⲿ����� AudioSource ���
            if (singleSource != null)
            {
                // ��������ߣ���ѡ��
                singleSource.pitch = Random.Range(audioPitch.x, audioPitch.y);
                singleSource.PlayOneShot(gunShotClip);
            }
            else
            {
                Debug.LogError("isAutoFire is OFF, but singleSource reference is missing! Please configure the Unit/BattleController.");
            }
        }
        else // isAutoFire Ϊ True
        {
            // === ģʽ����ʵ�������� (�ʺ�����/��������) ===
            if (audioSourcePrefab == null)
            {
                Debug.LogError("isAutoFire is ON, but audioSourcePrefab reference is missing on AudioManager!");
                return;
            }

            // 1. ʵ���� AudioSource
            AudioSource newAS = Instantiate(audioSourcePrefab, position, Quaternion.identity, sfxGunParent);

            // 2. ���������
            float randomPitch = Random.Range(audioPitch.x, audioPitch.y);
            newAS.pitch = randomPitch;

            // 3. ������Ч������
            newAS.PlayOneShot(gunShotClip);
            Destroy(newAS.gameObject, gunShotClip.length + 0.5f);
        }
    }
}
