using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlatMenu : MonoBehaviour
{
    [SerializeField] PlayerControls player_controls;

    private Tab[] tabs;
    private Tab current_tab;

    private void Start()
    {
        if (tabs != null)
            current_tab = tabs[0];

        foreach (Tab tab in tabs)
        {
            tab.OnElementClicked += ProcessElementClicked;
            tab.gameObject.SetActive(false);
        }

        current_tab.gameObject.SetActive(true);
    }

    private void SwitchTab(Tab new_tab)
    {
        current_tab.gameObject.SetActive(false);
        new_tab.gameObject.SetActive(true);
        current_tab = new_tab;
    }

    private void ProcessElementClicked(TabElement element)
    {
        switch (element.Function)
        {
            case TabElementFunctions.ChangeTab:
                SwitchTab(element.NewTab);
                break;

            case TabElementFunctions.ChangeBuilding:
                player_controls.ChangeBuildingStateRequest(ChangeRequestType.OnlyIfOff);
                player_controls.BuildingType = element.NewBuilding;
                break;

            default:
                Debug.Log("Somehow current function is not processing");
                break;
        }
    }
}
