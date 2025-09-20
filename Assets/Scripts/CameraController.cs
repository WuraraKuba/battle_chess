using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;   // 摄像机围绕旋转的目标
    public float distance = 100f;  // 摄像机与目标的距离
    public float rotationSpeed = 5f;  // 旋转速度
    public float panSpeed = 0.5f;   // 平移速度
    public float zoomSpeed = 5f;  // 缩放速度

    private float currentX = 0f;  // 摄像机在垂直方向上的总旋转量
    private float currentY = 0f;  // 摄像机在水平方向上的总旋转量

    private Vector3 panOrigin;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 鼠标右键旋转
        if (Input.GetMouseButton(1)) 
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentY = Mathf.Clamp(currentY, -60f, 60f);
        }

        if (Input.GetMouseButton(2))  // 按住中键时进行平移
        {
            float panX = Input.GetAxis("Mouse X") * panSpeed;
            float panY = Input.GetAxis("Mouse Y") * panSpeed;

            // 在摄像机的本地坐标系中平移
            Vector3 panDelta = transform.right * -panX + transform.up * -panY;
            // 锁定 Y 轴移动
            panDelta.y = 0; 
            transform.position += panDelta;
            target.position += panDelta;
            // transform.Translate(-panX, -panY, 0, Space.Self);
        }

        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            distance -= scrollDelta * zoomSpeed;
            distance = Mathf.Clamp(distance, 5f, 500f);  // 限制缩放距离
        }
    }

    private void LateUpdate()  // 在每一帧的Update函数被调用后被调用
    {
        // 计算旋转
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        // 计算新位置
        Vector3 position = rotation * new Vector3(0, 0, -distance) + target.position;
        // 应用到摄影机
        transform.rotation = rotation;
        transform.position = position;

    }
}
