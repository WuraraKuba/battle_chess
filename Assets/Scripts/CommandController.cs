using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// ָ�ӿ������������洢���ֶ��������ӵ�ָ��ָ���ʱ�����ż���
public class CommandController : MonoBehaviour
{
    // ����ģʽ(ȷ��ȫ�ֿɷ���)
    public static CommandController Instance;

    // ���ò���
    [Header("�ӵ�ʱ������")]
    public float slowMotionFactor = 0.1f; // �����������ٶȵ� 10%
    public float transitionDuration = 0.5f; // ����/�˳��ӵ�ʱ��Ĺ���ʱ��

    private float originalTimeScale = 1.0f;
    private bool isTimeWarping = false;

    [Header("��������")]
    public KeyCode slowMotionKey = KeyCode.LeftShift; // ���ڴ����ӵ�ʱ��İ���

    [Header("�������")]
    public Vector3 skillTarget;   // ��������λ��
    public Vector3 skillLoc;      // ���ܳ�ʼλ��
    public bool isSkill;  // ���뼼�ܴ���״̬
    public GameObject skillObject;

    private Coroutine timeWarpCoroutine = null;


    // ��������״̬��������ÿһ֡�ظ�����Э��
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
        // 1. ���������¼� (��δ��ס����ס�ĵ�һ֡)
        if (keyIsCurrentlyHeld && !isKeyHeld)
        {
            // �����ӵ�ʱ��
            if (timeWarpCoroutine != null)
            {
                StopCoroutine(timeWarpCoroutine);
            }
            StartCoroutine(TransitionTimeScale(slowMotionFactor));
            isKeyHeld = true; // ����״̬Ϊ��ס
        }

        // 2. �����ɿ��¼� (�Ӱ�ס���ɿ��ĵ�һ֡)
        else if (!keyIsCurrentlyHeld && isKeyHeld)
        {
            // �ָ������ٶ�
            if (timeWarpCoroutine != null)
            {
                StopCoroutine(timeWarpCoroutine);
            }
            StartCoroutine(TransitionTimeScale(originalTimeScale));
            isKeyHeld = false; // ����״̬Ϊ�ɿ�
        }

        // ��MouseControl�����ȡ����
        if (MouseControl.Instance != null) 
        {
            skillTarget = MouseControl.Instance.ReturnRaycastHitLoc();
        }

        // ���뼼���ͷ�״̬
        if (isSkill) 
        {
            FireInTheHole();
        }

    }

    private IEnumerator TransitionTimeScale(float targetScale)
    {

        if (UIController.Instance != null) // ʹ�õ�������ȡ UIController
        {

            // ���Ŀ�������� (targetScale < originalTimeScale)������ʾ���
            bool shouldShowUI = (targetScale < originalTimeScale);

            // ���� UIController ���·���
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

        // ȷ������ֵ��׼ȷ��
        Time.timeScale = targetScale;
    }

    // ������
    public void GetSkillLoc(Vector3 startLoc, GameObject skillObj)
    {
        isSkill = true;
        skillLoc = startLoc;
        skillObject = skillObj;
        Debug.Log("����ʱ�ŵ�" + skillLoc);
    }

    // ���׼����ͷ�
    public void FireInTheHole()
    {
        // ����������������
        if (HXController.Instance != null) 
        {
            skillTarget = HXController.Instance.GetRestrictedEndPoint(skillLoc, skillTarget, 100f);
            HXController.Instance.DrawSkillPath(skillLoc, skillTarget);
        }
        // ������F�������ͷ�����
        if (Input.GetKeyDown(KeyCode.F))
        {
            HXController.Instance.HidePath();
            isSkill = false;
            // ��ȡ���������, �ѻ�ȡ
            // ��������
            skillLoc += new Vector3(0, 20f, 0);
            Vector3 startFlat = new Vector3(skillLoc.x, 0, skillLoc.z);
            Vector3 endFlat = new Vector3(skillTarget.x, 0, skillTarget.z);
            float actualHorizontalDistance = Vector3.Distance(startFlat, endFlat); // �����������һ�� <= maxThrowRange
            float actualDeltaY = skillTarget.y - skillLoc.y;

            float g = Physics.gravity.magnitude;

            // B. ȷ������ǶȺ��ٶ� (ʹ��֮ǰ���Գ������Ž����߼�)
            float baseLaunchAngle = 20f;
            float initialSpeed = 30f;
            float baseAngleRad = baseLaunchAngle * Mathf.Deg2Rad; // ���� baseLaunchAngle �� CommandController �ı���

            // 1. �̶� Vx (����ȷ�� timeToHit)
            float Vx = initialSpeed * Mathf.Cos(baseAngleRad);
            if (Vx < 0.001f) Vx = 0.001f;

            // 2. ȷ�� timeToHit
            float timeToHit = actualHorizontalDistance / Vx;
            timeToHit = Mathf.Max(0.1f, timeToHit);

            // 3. ���� Vy (ȷ�������������Ŀ��߶�)
            float Vy = (actualDeltaY / timeToHit) + (0.5f * g * timeToHit);

            // 4. ����ʵ�ʷ�������ٶȺͽǶ�
            float actualLaunchVelocity = Mathf.Sqrt(Vx * Vx + Vy * Vy);
            float actualLaunchAngleRad = Mathf.Atan2(Vy, Vx);

            if (skillObject != null)
            {
                // ʵ��������
                GameObject grenade = Instantiate(skillObject, skillLoc, Quaternion.identity);
                Debug.Log("��������û��" + grenade);
                if (grenade.TryGetComponent<GrenadesController>(out var launcher))
                {
                    // Launch �������� StartPoint, EndPoint, ���ٶ�, �Ƕ�
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
