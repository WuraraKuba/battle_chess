using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("技能相关")]
    public Vector3 skillTarget;   // 技能最终位置
    public Vector3 skillLoc;      // 技能初始位置
    public bool isSkill;  // 进入技能触发状态
    public GameObject skillObject;

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
        isSkill = false;
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

        // 从MouseControl哪里获取坐标
        if (MouseControl.Instance != null) 
        {
            skillTarget = MouseControl.Instance.ReturnRaycastHitLoc();
        }

        // 进入技能释放状态
        if (isSkill) 
        {
            FireInTheHole();
        }

    }

    private IEnumerator TransitionTimeScale(float targetScale)
    {

        if (UIController.Instance != null) // 使用单例来获取 UIController
        {

            // 如果目标是慢速 (targetScale < originalTimeScale)，则显示面板
            bool shouldShowUI = (targetScale < originalTimeScale);

            // 调用 UIController 的新方法
            UIController.Instance.SetTimeWarpUIVisibility(shouldShowUI);
        }

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

    // 扔手雷
    public void GetSkillLoc(Vector3 startLoc, GameObject skillObj)
    {
        isSkill = true;
        skillLoc = startLoc;
        skillObject = skillObj;
        Debug.Log("技能时放点" + skillLoc);
    }

    // 手雷技能释放
    public void FireInTheHole()
    {
        // 绘制两点间的抛物线
        if (HXController.Instance != null) 
        {
            skillTarget = HXController.Instance.GetRestrictedEndPoint(skillLoc, skillTarget, 100f);
            HXController.Instance.DrawSkillPath(skillLoc, skillTarget);
        }
        // 按下了F键用于释放手雷
        if (Input.GetKeyDown(KeyCode.F))
        {
            HXController.Instance.HidePath();
            isSkill = false;
            // 获取对象的手雷, 已获取
            // 发射手雷
            skillLoc += new Vector3(0, 20f, 0);
            Vector3 startFlat = new Vector3(skillLoc.x, 0, skillLoc.z);
            Vector3 endFlat = new Vector3(skillTarget.x, 0, skillTarget.z);
            float actualHorizontalDistance = Vector3.Distance(startFlat, endFlat); // 现在这个距离一定 <= maxThrowRange
            float actualDeltaY = skillTarget.y - skillLoc.y;

            float g = Physics.gravity.magnitude;

            // B. 确定发射角度和速度 (使用之前调试出的最优解算逻辑)
            float baseLaunchAngle = 20f;
            float initialSpeed = 30f;
            float baseAngleRad = baseLaunchAngle * Mathf.Deg2Rad; // 假设 baseLaunchAngle 是 CommandController 的变量

            // 1. 固定 Vx (用于确定 timeToHit)
            float Vx = initialSpeed * Mathf.Cos(baseAngleRad);
            if (Vx < 0.001f) Vx = 0.001f;

            // 2. 确定 timeToHit
            float timeToHit = actualHorizontalDistance / Vx;
            timeToHit = Mathf.Max(0.1f, timeToHit);

            // 3. 反求 Vy (确保命中修正后的目标高度)
            float Vy = (actualDeltaY / timeToHit) + (0.5f * g * timeToHit);

            // 4. 计算实际发射的总速度和角度
            float actualLaunchVelocity = Mathf.Sqrt(Vx * Vx + Vy * Vy);
            float actualLaunchAngleRad = Mathf.Atan2(Vy, Vx);

            if (skillObject != null)
            {
                // 实例化手雷
                GameObject grenade = Instantiate(skillObject, skillLoc, Quaternion.identity);
                Debug.Log("手雷生成没有" + grenade);
                if (grenade.TryGetComponent<GrenadesController>(out var launcher))
                {
                    // Launch 函数接收 StartPoint, EndPoint, 总速度, 角度
                    launcher.Launch(
                        skillLoc,
                        skillTarget,
                        actualLaunchVelocity,
                        actualLaunchAngleRad
                    );
                }
            }

        }
    }
}
