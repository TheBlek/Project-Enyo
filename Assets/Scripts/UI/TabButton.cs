using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Action<TabButton> OnClick;
    public Action<TabButton> OnEnter;
    public Action<TabButton> OnExit;

    public void ChangeSprite(Sprite sprite)
    {
        GetComponent<Image>().sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit(this);
    }
}
