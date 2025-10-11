using System.Collections;
using System.Collections.Generic;
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

        // ȷ������ֵ��׼ȷ��
        Time.timeScale = targetScale;
    }
}
