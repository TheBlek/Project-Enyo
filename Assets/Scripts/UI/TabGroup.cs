using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class TabGroup : MonoBehaviour
{
    [SerializeField] private Sprite idle_sprite;
    [SerializeField] private Sprite hovered_sprite;
    [SerializeField] private Sprite selected_sprite;
    [SerializeField] List<TabButton> buttons;

    [SerializeField] private float offset_selected;
    [SerializeField] private float offset_hovered;
    
    private TabButton selected_button;
    private float base_positionY;


    private void Start()
    {
        base_positionY = buttons[0].GetComponent<RectTransform>().anchoredPosition.y;
        foreach (TabButton button in buttons)
        {
            button.OnClick += OnTabSelected;
            button.OnEnter += OnTabEnter;
            button.OnExit += OnTabExit;
            ResetTabButton(button);
        }
    }

    private void OnTabEnter(TabButton button)
    {
        if (button != selected_button || selected_button == null)
        {
            button.ChangeSprite(hovered_sprite);
            button.GetComponent<RectTransform>().anchoredPosition += Vector2.up * offset_hovered;
        }
    }

    private void OnTabExit(TabButton button)
    {
        if (button != selected_button)
        {
            ResetTabButton(button);
        }
    }

    private void OnTabSelected(TabButton button)
    {
        ResetTabButtons();
        button.ChangeSprite(selected_sprite);
        button.GetComponent<RectTransform>().anchoredPosition += Vector2.up * offset_selected;
        selected_button = button;
    }

    private void ResetTabButton(TabButton button)
    {
        button.ChangeSprite(idle_sprite);
        RectTransform trans = button.GetComponent<RectTransform>();
        Vector2 pos = trans.anchoredPosition;
        pos.y = base_positionY;
        trans.anchoredPosition = pos;
    }

    private void ResetTabButtons()
    {
        foreach (TabButton button in buttons)
        {
            ResetTabButton(button);
        }
    }

}
