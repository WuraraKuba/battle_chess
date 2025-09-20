using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// ���������صĲ���������ѡ��֮���
public class MouseControl : MonoBehaviour
{
    public AStarPathfinder aStarPathfinder;
    public Material highlightMaterial;  // �����㴴���ĸ�������
    private GameObject selectedObject;  // �洢��ǰѡ�е�����
    private Material originalMatrial;   // �洢ԭʼ����

    private GameObject currentHoverObject; // ��ǰ�����ͣ������
    private Vector3 originalPosition;      // �洢��ͣ�����ԭʼλ��

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // �����������񣬶�����һ������Ӱ��λ�������λ�÷��������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // ���ڴ洢���߼��Ľ��
        RaycastHit hit;
        // ��������Ƿ�Ӵ�������
        if (Physics.Raycast(ray, out hit))
        {
            // ����������Ƿ񱻰���
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.gameObject.CompareTag("Player"))   // Tag���ڴ���������
                {
                    if (selectedObject != null)
                    {  // ���֮ǰѡ������ô��ԭ��ѡ���Ĳ����滻Ϊԭʼ���ʺ��ٽ�����һ������
                        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
                    }
                    selectedObject = hit.transform.gameObject;
                    //originalMatrial = selectedObject.GetComponent<Material>();
                    originalMatrial = selectedObject.GetComponent<MeshRenderer>().material;
                    selectedObject.GetComponent<MeshRenderer>().material = highlightMaterial;
                }
                else  // ѡ�еĲ����µ�player
                {
                    if (selectedObject != null)   // selectedObject ֻ������ѡ�еĽ�ɫ
                    {
                        if (hit.transform.gameObject.CompareTag("GridCell"))  // �ٴε������һ�����̸�
                        {
                            // ��ȡ�ƶ�������ʼ����
                            Vector3 startPosition = selectedObject.transform.position;
                            // ��ȡ������̸������
                            Vector3 targetPosition = hit.transform.position;
                            // �ƶ�ѡ�����������
                            //selectedObject.transform.position = targetPosition;
                            // ����Ѱ·�㷨
                            List<Vector3> path = aStarPathfinder.FindPath(startPosition, targetPosition);
                            if (path != null)
                            {
                                // StartCoroutine : ����Э��
                                StartCoroutine(MoveAlongPath(path));
                                //foreach (Vector3 node in path)
                                //{
                                    //Debug.Log(node);
                                    //selectedObject.transform.position = node;
                                //}
                                //selectedObject.transform.position = path[path.Count - 1];
                            }
                            else
                            {
                                selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
                                selectedObject = null;
                            }
                           
                        }
                    }
                    
                }
            }
            // ������û�б����µ����
            else
            {
                if (hit.transform.gameObject.CompareTag("GridCell"))
                {
                    if (currentHoverObject != hit.transform.gameObject)
                    {
                        if (currentHoverObject != null)
                        {
                            currentHoverObject.transform.position = originalPosition;
                        }
                            currentHoverObject = hit.transform.gameObject;
                            originalPosition = hit.transform.position;
                            currentHoverObject.transform.position += new Vector3(0, 0.5f, 0);
                    }
                }
            }
        }
        else  // û�м�⵽����
        {
            if (currentHoverObject != null)
            {
                currentHoverObject.transform.position = originalPosition;
            }
        }
        
    }

    // �ƶ�Э��, ��νЭ��
    IEnumerator MoveAlongPath(List<Vector3> path)
    {
        // ȷ��·����Ϊ��
        if (path == null || path.Count == 0)
        {
            yield break; // �˳�Э��
        }

        float moveSpeed = 5f; // �ƶ��ٶ�

        // ����·���е�ÿ���ڵ�
        foreach (Vector3 targetNode in path)
        {
            // �����ƶ���ֱ������Ŀ��ڵ�
            while (selectedObject.transform.position != targetNode)
            {
                
                // ����Ŀ����ƶ�
                selectedObject.transform.position = Vector3.MoveTowards(
                    selectedObject.transform.position,
                    targetNode,
                    moveSpeed * Time.deltaTime
                );

                // ����unity�� ��һ��ѭ���ȵ���һ֡������
                yield return null; // �ȴ���һ֡
            }
        }
        // === �ƶ���ɣ�������ȡ��ѡ��״̬ ===
        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
        selectedObject = null;
    }
}
