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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // 2026.9.2
        // 检查鼠标左键是否被点击
        if (Input.GetMouseButtonDown(0)) {
            // 难道所有鼠标选择都是这样的吗，UI他有自己专门的
            // 创建一条从摄像机位置向鼠标位置发射的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 用于存储射线检测的结果
            RaycastHit hit;
            // Physics.Raycast:进行射线投射的方法
            // ray:投射起点与方向
            // out: 表示投射后遇到的第一个的结果会输出给hit
            if (Physics.Raycast(ray, out hit)) {

                // 左边返回的是层级索引，所以右边需要根据layer名找到其索引
                //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("chess"))
                if (hit.transform.gameObject.CompareTag("Player"))   // Tag用于处理分类情况
                {
                    if (selectedObject != null){  // 如果之前选过，那么将原来选过的材质替换为原始材质后再进行下一步操作
                        selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
                    }
                    selectedObject = hit.transform.gameObject;
                    //originalMatrial = selectedObject.GetComponent<Material>();
                    originalMatrial = selectedObject.GetComponent<MeshRenderer>().material;
                    selectedObject.GetComponent<MeshRenderer>().material = highlightMaterial;
                    Debug.Log("已选中");
                    // 修改状态，在视觉上表示其被选中
                }
                else {
                    selectedObject.GetComponent<MeshRenderer>().material = originalMatrial;
                }

            }
        }
    }
}
