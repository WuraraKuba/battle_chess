using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// 用于鼠标相关的操作，包括选择，之类的
public class MouseControl : MonoBehaviour
{
    public Material highlightMaterial;  // 拖入你创建的高亮材质
    private GameObject selectedObject;  // 存储当前选中的物体
    private Material originalMatrial;   // 存储原始材质

    private GameObject currentHoverObject; // 当前鼠标悬停的物体
    private Vector3 originalPosition;      // 存储悬停物体的原始位置

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 无论鼠标点击与否，都创建一个从摄影机位置向鼠标位置发射的射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 用于存储射线检测的结果
        RaycastHit hit;
        // 检测射线是否接触到物体
        if (Physics.Raycast(ray, out hit))
        {
            // 检测鼠标左键是否被按下
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.gameObject.CompareTag("Player"))   // Tag用于处理分类情况
                {
                    if (selectedObject != null)
                    {  // 如果之前选过，那么将原来选过的材质替换为原始材质后再进行下一步操作
                        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
                    }
                    selectedObject = hit.transform.gameObject;
                    //originalMatrial = selectedObject.GetComponent<Material>();
                    originalMatrial = selectedObject.GetComponent<MeshRenderer>().material;
                    selectedObject.GetComponent<MeshRenderer>().material = highlightMaterial;
                    Debug.Log("已选中");
                }
                else
                {
                    if (selectedObject != null)
                    {
                        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
                    }
                    
                }
            }
            // 鼠标左键没有被按下的情况
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
        else  // 没有检测到物体
        {
            if (currentHoverObject != null)
            {
                currentHoverObject.transform.position = originalPosition;
            }
        }
        
    }
}
