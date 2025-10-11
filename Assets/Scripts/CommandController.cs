using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 指挥控制器，用来存储各种独立于棋子的指挥指令，如时间流逝减缓
public class CommandController : MonoBehaviour
{
    // 单例模式(确保全局可访问)
    public static CommandController Instance;

    // 配置参数
    [Header("子弹时间配置")]
    public float slowMotionFactor = 0.1f; // 减缓到正常速度的 10%
    public float transitionDuration = 0.5f; // 进入/退出子弹时间的过渡时间

    private float originalTimeScale = 1.0f;
    private bool isTimeWarping = false;

    [Header("输入配置")]
    public KeyCode slowMotionKey = KeyCode.LeftShift; // 用于触发子弹时间的按键

    private Coroutine timeWarpCoroutine = null;


    // 跟踪输入状态，避免在每一帧重复启动协程
    private bool isKeyHeld = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            originalTimeScale = Time.timeScale;
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
        bool keyIsCurrentlyHeld = Input.GetKey(slowMotionKey);
        // 1. 按键按下事件 (从未按住到按住的第一帧)
        if (keyIsCurrentlyHeld && !isKeyHeld)
        {
            // 启动子弹时间
            if (timeWarpCoroutine != null)
            {
                StopCoroutine(timeWarpCoroutine);
            }
            StartCoroutine(TransitionTimeScale(slowMotionFactor));
            isKeyHeld = true; // 更新状态为按住
        }

        // 2. 按键松开事件 (从按住到松开的第一帧)
        else if (!keyIsCurrentlyHeld && isKeyHeld)
        {
            // 恢复正常速度
            if (timeWarpCoroutine != null)
            {
                StopCoroutine(timeWarpCoroutine);
            }
            StartCoroutine(TransitionTimeScale(originalTimeScale));
            isKeyHeld = false; // 更新状态为松开
        }
    }

    private IEnumerator TransitionTimeScale(float targetScale)
    {
        isTimeWarping = (targetScale < originalTimeScale);
        float time = 0;
        float startScale = Time.timeScale;

        while (time < transitionDuration)
        {
            Time.timeScale = Mathf.Lerp(startScale, targetScale, time / transitionDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        // 确保最终值是准确的
        Time.timeScale = targetScale;
    }
}
