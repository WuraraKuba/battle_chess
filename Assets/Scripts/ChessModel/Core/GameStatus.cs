using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于表征游戏状态的
/// </summary>
public enum GameStatus
{
    BeforeGame,  // 游戏前
    InGame,      // 游戏中  -点击start按钮进入游戏中状态
    Victory,     // 棋盘上无敌人
    Failure      // 棋盘上无我方单位
}
