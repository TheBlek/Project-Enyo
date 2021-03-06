﻿using System;
using UnityEngine;

public class FlatMenu : MonoBehaviour
{
    [SerializeField] PlayerControls player_controls;

    private Tab[] tabs;
    private Tab current_tab;

    private void Start()
    {
        SetUpTabs();
    }

    private void SetUpTabs()
    {
        tabs = GetComponentsInChildren<Tab>();

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
        if (!Array.Exists<Tab>(tabs, element => element == new_tab))
        {
            Debug.Log("Menu can not switch to this tab. It is not included in this menu");
            return;
        }

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
