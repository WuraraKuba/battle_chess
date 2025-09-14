using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// ���������صĲ���������ѡ��֮���
public class MouseControl : MonoBehaviour
{
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
                    Debug.Log("��ѡ��");
                }
                else
                {
                    if (selectedObject != null)
                    {
                        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
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
}
