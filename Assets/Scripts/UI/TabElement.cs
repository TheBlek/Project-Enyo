﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TabElement : MonoBehaviour, IGridItem // tabElement assumes that it on the same object as tabButton
{
    private Vector2Int grid_position;

    public Action<TabElement> OnButtonClick;
    public Action<TabElement> OnButtonEnter;
    public Action<TabElement> OnButtonExit;
    public Action<TabElement> OnButtonDown;

    [SerializeField] private TabElementFunctions function;
    [SerializeField] private Buildings new_building;
    [SerializeField] private Tab new_tab;

    private Image image;
    private TabButton button;

    public Sprite CurrentSprite
    {
        get { return image.sprite; }
        set { image.sprite = value; }
    }

    public TabElementFunctions Function => function;
    public Buildings NewBuilding => new_building;
    public Tab NewTab => new_tab;

    private void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<TabButton>();
        button.OnClick += Clicked;
        button.OnEnter += Entered;
        button.OnExit += Exited;
        button.OnDown += Pressed;
    }

    public Vector2Int GridPosition
    {
        get
        {
            return grid_position;
        }
        set
        {
            grid_position = value;
        }
    }

    #region Delegates in here
    private void Clicked()
    {
        OnButtonClick(this);
    }

    private void Entered()
    {
        OnButtonEnter(this);
    }

    private void Exited()
    {
        OnButtonExit(this);
    }

    private void Pressed()
    {
        OnButtonDown(this);
    }
    #endregion
}
