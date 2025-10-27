using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ���ÿ�����ӵ���Ϣ
/// </summary>
[CreateAssetMenu(fileName = "NewUnitData", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    [SerializeField]
    public GameObject gameObject;  // ���ӵ�Prefab

/*    [SerializeField]
    private Sprite Icon;   // ��UI����ʾ��ͼƬ*/

    public string UnitName = "UnitTest";

    public Vector3 UnitLocation = Vector3.zero;

    public bool isEnemy;

}
