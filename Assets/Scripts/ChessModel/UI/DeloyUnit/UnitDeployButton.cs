using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitDeployButton : MonoBehaviour
{
    
    public UnitData associateUnitData;


    public void Initialize(UnitData unitData)
    {
        TextMeshProUGUI buttonText = GetComponentInChildren<TextMeshProUGUI>();
        associateUnitData = unitData;
        Debug.Log(unitData.name);
        buttonText.text = unitData.UnitName;

        Button buttonComponent = GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.interactable = true;
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(OnDeployButtonClicked);
        }
    }
    public void OnDeployButtonClicked()
    {
        if (associateUnitData == null) return;
        if (UnitCoreController.Instance != null)
        {
            UnitCoreController.Instance.DeployUnit(associateUnitData.gameObject, associateUnitData.UnitLocation);
        }
        // 通报该事件完成
        GlobalEvents.ReportUnitDeployedFinshed();
    }

}
