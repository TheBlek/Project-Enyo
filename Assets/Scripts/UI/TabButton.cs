using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    public UnityAction OnClick;
    public UnityAction OnEnter;
    public UnityAction OnExit;
    public UnityAction OnDown; 

    public void ChangeSprite(Sprite sprite)
    {
        GetComponent<Image>().sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDown();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit();
    }
}
