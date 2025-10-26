using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDeployUI : MonoBehaviour
{

    public GameObject unitDeployButtonPrefab;

    // 动态创建按钮
    public void DeployButtons(List<UnitData> unitDatas, Vector3 DeployLoc, ref List<GameObject> activeButtons)
    {
        ClearDeployButtons(ref activeButtons);
        Debug.Log("？？？？" + unitDatas.Count);
        if (unitDatas == null || unitDatas.Count == 0) return;
        else
        {
            for (int i=0; i<unitDatas.Count; i++)
            {
                unitDatas[i].UnitLocation = DeployLoc;
                GameObject newButtonObj = Instantiate(unitDeployButtonPrefab, transform);
                activeButtons.Add(newButtonObj);
                // 3. 初始化按钮数据 (设置文本)
                UnitDeployButton unitButton = newButtonObj.GetComponent<UnitDeployButton>();
                if (unitButton != null)
                {
                    unitButton.Initialize(unitDatas[i]);
                }
            }
            gameObject.SetActive(true);

        }
    }

    public void ClearDeployButtons(ref List<GameObject> activeButtons)
    {
        foreach (GameObject button in activeButtons)
        {
            Destroy(button);
        }
        activeButtons.Clear();
        gameObject.SetActive(false);
    }

}
