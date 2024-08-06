using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseVisibilityHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Fare imlecini görünür yap
        Cursor.visible = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Fare imlecini gizle
        Cursor.visible = false;
    }
}