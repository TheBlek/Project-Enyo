using System;
using UnityEngine;
using UnityEngine.UI;

public class Tab : GridManager<TabElement>
{
    public Action<TabElement> OnElementClicked;

    [SerializeField] private Sprite idle_sprite;
    [SerializeField] private Sprite hover_sprite;
    [SerializeField] private Sprite press_sprite;

    private void Awake()
    {
        SetUpMetrics();
        InitGrid();
        ScrapGrid();
    }

    private void Start()
    {
        foreach (TabElement element in grid)
        {
            element.OnButtonDown += Pressed;
            element.OnButtonEnter += Entered;
            element.OnButtonExit += Exited;
            element.OnButtonClick += Clicked;
        }
    }

    private void SetUpMetrics()
    {
        Rect rect = GetComponent<RectTransform>().rect;
        cell_size = Math.Min(rect.width / grid_size.x, rect.height / grid_size.y);
        grid_origin = new Vector2(rect.xMin, rect.yMin);
    }

    private void ScrapGrid()
    {

        TabElement[] elements = GetComponentsInChildren<TabElement>();
        foreach (TabElement element in elements)
        {
            SetCellByGlobalPosition(element.GetComponent<RectTransform>().anchoredPosition, element);
            Debug.Log(GetGridPositionFromGlobal(element.GetComponent<RectTransform>().anchoredPosition));
            element.GetComponent<RectTransform>().anchoredPosition = GetGlobalPosition(element);
            element.CurrentSprite = idle_sprite;
        }
    }

    private void Clicked(TabElement element)
    {
        element.CurrentSprite = idle_sprite;
        OnElementClicked(element);
    }

    private void Pressed(TabElement element)
    {
        element.CurrentSprite = press_sprite;
    }

    private void Exited(TabElement element)
    {
        element.CurrentSprite = idle_sprite;
    }
    
    private void Entered(TabElement element)
    {
        element.CurrentSprite = hover_sprite;
    }

}
