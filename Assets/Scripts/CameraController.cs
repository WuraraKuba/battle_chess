using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;   // �����Χ����ת��Ŀ��
    public float distance = 100f;  // �������Ŀ��ľ���
    public float rotationSpeed = 5f;  // ��ת�ٶ�
    public float panSpeed = 0.5f;   // ƽ���ٶ�
    public float zoomSpeed = 5f;  // �����ٶ�

    private float currentX = 0f;  // ������ڴ�ֱ�����ϵ�����ת��
    private float currentY = 0f;  // �������ˮƽ�����ϵ�����ת��

    private Vector3 panOrigin;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ����Ҽ���ת
        if (Input.GetMouseButton(1)) 
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentY = Mathf.Clamp(currentY, -60f, 60f);
        }

        if (Input.GetMouseButton(2))  // ��ס�м�ʱ����ƽ��
        {
            float panX = Input.GetAxis("Mouse X") * panSpeed;
            float panY = Input.GetAxis("Mouse Y") * panSpeed;

            // ��������ı�������ϵ��ƽ��
            Vector3 panDelta = transform.right * -panX + transform.up * -panY;
            // ���� Y ���ƶ�
            panDelta.y = 0; 
            transform.position += panDelta;
            target.position += panDelta;
            // transform.Translate(-panX, -panY, 0, Space.Self);
        }

        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            distance -= scrollDelta * zoomSpeed;
            distance = Mathf.Clamp(distance, 5f, 500f);  // �������ž���
        }
    }

    private void LateUpdate()  // ��ÿһ֡��Update���������ú󱻵���
    {
        // ������ת
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        // ������λ��
        Vector3 position = rotation * new Vector3(0, 0, -distance) + target.position;
        // Ӧ�õ���Ӱ��
        transform.rotation = rotation;
        transform.position = position;

    }
}
