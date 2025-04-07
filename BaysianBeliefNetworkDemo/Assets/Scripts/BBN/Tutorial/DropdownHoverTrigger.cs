using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

public class DropdownHoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private DropdownList dropdown;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        dropdown.MoveDown();    
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        dropdown.MoveUp();
    }
}
