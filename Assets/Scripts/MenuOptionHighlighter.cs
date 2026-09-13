using UnityEngine;
using UnityEngine.EventSystems;

public class MenuOptionHighlighter : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public MainMenuController menu;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (menu != null) menu.SetHighlighted(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (menu != null) menu.SetHighlighted(this);
    }
}
