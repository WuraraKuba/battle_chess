using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 针对每个棋子的信息
/// </summary>
[CreateAssetMenu(fileName = "NewUnitData", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    [SerializeField]
    public GameObject gameObject;  // 棋子的Prefab

/*    [SerializeField]
    private Sprite Icon;   // 在UI上显示的图片*/

    public string UnitName = "UnitTest";

    public Vector3 UnitLocation = Vector3.zero;

    public bool isEnemy;

    public int AP;  // 行动力

}
