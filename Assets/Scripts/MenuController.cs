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
        // 可以在这里加载游戏场景或执行其他启动逻辑
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void ShowMenu()
    {
        mainMenuPanel.SetActive(true);
        Time.timeScale = 0.1f; // 【关键】暂停游戏时间，让 AI 和物理停止
    }
    private void HideMenu()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f; // 恢复游戏时间
    }
}
