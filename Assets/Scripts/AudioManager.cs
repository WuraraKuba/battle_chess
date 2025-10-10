using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // --- 1. 单例模式 ---
    // 确保只有一个对象
    public static AudioManager Instance;

    // 音效相关
    private bool useSingleSource;
    [Header("Weapon Audio")]
    public AudioClip gunShotClip;        // 枪声文件 (AudioClip)
    [Tooltip("用于连发模式，需要拖入一个 AudioSource Prefab")]
    public AudioSource audioSourcePrefab; // 用于连发时实例化的 AudioSource Prefab
    [Tooltip("用于单发模式，需要拖入角色身上的 AudioSource 组件")]
    public AudioSource singleAudioSource; // 角色身上的 AudioSource 引用

    public Vector2 audioPitch = new Vector2(0.9f, 1.1f); // 随机音高范围

    [Header("Parent Object")]
    public Transform sfxGunParent;

    private void Awake()
    {
        // 确保只有一个 AudioManager 实例存在
        if (Instance == null)
        {
            Instance = this;
            // 如果你想让它在场景切换时不被销毁，可以添加这行：
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

        // 用 isAutoFire 替换 useSingleSource
        if (!isAutoFire)
        {
            // === 模式一：单源播放 (适合单发/慢速武器) ===
            // 依赖于外部传入的 AudioSource 组件
            if (singleSource != null)
            {
                // 随机化音高（可选）
                singleSource.pitch = Random.Range(audioPitch.x, audioPitch.y);
                singleSource.PlayOneShot(gunShotClip);
            }
            else
            {
                Debug.LogError("isAutoFire is OFF, but singleSource reference is missing! Please configure the Unit/BattleController.");
            }
        }
        else // isAutoFire 为 True
        {
            // === 模式二：实例化播放 (适合连发/快速武器) ===
            if (audioSourcePrefab == null)
            {
                Debug.LogError("isAutoFire is ON, but audioSourcePrefab reference is missing on AudioManager!");
                return;
            }

            // 1. 实例化 AudioSource
            AudioSource newAS = Instantiate(audioSourcePrefab, position, Quaternion.identity, sfxGunParent);

            // 2. 随机化音高
            float randomPitch = Random.Range(audioPitch.x, audioPitch.y);
            newAS.pitch = randomPitch;

            // 3. 播放音效并销毁
            newAS.PlayOneShot(gunShotClip);
            Destroy(newAS.gameObject, gunShotClip.length + 0.5f);
        }
    }
}
