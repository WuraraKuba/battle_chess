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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // 2026.9.2
        // ����������Ƿ񱻵��
        if (Input.GetMouseButtonDown(0)) {
            // �ѵ��������ѡ������������UI�����Լ�ר�ŵ�
            // ����һ���������λ�������λ�÷��������
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // ���ڴ洢���߼��Ľ��
            RaycastHit hit;
            // Physics.Raycast:��������Ͷ��ķ���
            // ray:Ͷ������뷽��
            // out: ��ʾͶ��������ĵ�һ���Ľ���������hit
            if (Physics.Raycast(ray, out hit)) {

                // ��߷��ص��ǲ㼶�����������ұ���Ҫ����layer���ҵ�������
                //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("chess"))
                if (hit.transform.gameObject.CompareTag("Player"))   // Tag���ڴ���������
                {
                    if (selectedObject != null){  // ���֮ǰѡ������ô��ԭ��ѡ���Ĳ����滻Ϊԭʼ���ʺ��ٽ�����һ������
                        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
                    }
                    selectedObject = hit.transform.gameObject;
                    //originalMatrial = selectedObject.GetComponent<Material>();
                    originalMatrial = selectedObject.GetComponent<MeshRenderer>().material;
                    selectedObject.GetComponent<MeshRenderer>().material = highlightMaterial;
                    Debug.Log("��ѡ��");
                    // �޸�״̬�����Ӿ��ϱ�ʾ�䱻ѡ��
                }
                else {
                    selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
                }

            }
        }
    }
}
