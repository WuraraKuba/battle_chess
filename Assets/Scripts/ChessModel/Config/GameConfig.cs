using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Global Config")]
public class GameConfig : ScriptableObject
{
    [Header("Ä¬ÈÏÉäÏßºöÂÔ²ã")]
    public string[] RaycastIgnoredLayers = new string[] { "trigger", "Ignore Raycast" };
}
