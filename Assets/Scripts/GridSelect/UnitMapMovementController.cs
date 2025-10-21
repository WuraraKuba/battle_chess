using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMapMovementController : MonoBehaviour
{
    public static UnitMapMovementController Instance {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ȷ�������ᱻ����
            DontDestroyOnLoad(gameObject);
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
    // �������������������ⲿ����
    public void StartMovement(GameObject selectedObject, List<Vector3> path)
    {
        // �ؼ�����һ�� MonoBehaviour ʵ���ϵ��� StartCoroutine
        StartCoroutine(MoveAlongPath(selectedObject, path));
    }

    public IEnumerator MoveAlongPath(GameObject selectedObject, List<Vector3> path)
    {

        // ȷ��·����Ϊ��
        if (path == null || path.Count == 0)
        {
            yield break; // �˳�Э��
        }
/*        // ȷ��ѡ�������λ����·�������ͬ
        if (selectedObject.transform.position.x != path[0].x || selectedObject.transform.position.z != path[0].z)
        {
            Debug.Log("��ʼ�㲻�ԣ�");
            yield break;
        }*/

        float moveSpeed = 1f; // �ƶ��ٶ�
        /*        // ��ȡAnimator���
                Animator animator = selectedObject.GetComponent<Animator>();*/
        /*        // �ƶ���ʼ
                if (animator != null)
                {
                    animator.SetFloat("Speed", moveSpeed);
                    animator.SetFloat("MotionSpeed", moveSpeed);
                }*/
        // ����·���е�ÿ���ڵ�
        foreach (Vector3 targetNode in path)
        {
            
            // �ý�ɫ����Ŀ���
            selectedObject.transform.LookAt(targetNode);
            // �����ƶ���ֱ������Ŀ��ڵ�
            while (Vector3.Distance(selectedObject.transform.position, targetNode) > 0.01f)
            {
                //Debug.Log("�ƶ������������");
                Vector3 oldPos = selectedObject.transform.position;
                //Debug.Log("�ɵص�" + oldPos);
                //Debug.Log("�µص�" +  targetNode);
                // ����Ŀ����ƶ�
                selectedObject.transform.position = Vector3.MoveTowards(
                    oldPos,
                    targetNode,
                    moveSpeed * Time.deltaTime
                );
                // ����unity�� ��һ��ѭ���ȵ���һ֡������
                yield return null; // �ȴ���һ֡
            }
        }
/*        // === �ƶ���ɣ�������ȡ��ѡ��״̬ ===
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("MotionSpeed", 0f); // �ָ��� Idle ״̬
        }
        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
        selectedObject = null;*/
    }
}
