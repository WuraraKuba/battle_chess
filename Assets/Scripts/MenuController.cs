using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    // Start is called before the first frame update
    void Start()
    {
        ShowMenu();
    }
    public void StartGame()
    {
        HideMenu();
        // ���������������Ϸ������ִ�����������߼�
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void ShowMenu()
    {
        mainMenuPanel.SetActive(true);
        Time.timeScale = 0.1f; // ���ؼ�����ͣ��Ϸʱ�䣬�� AI ������ֹͣ
    }
    private void HideMenu()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f; // �ָ���Ϸʱ��
    }
}
