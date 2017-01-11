using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class ScrollerSnap : MonoBehaviour, IEndDragHandler
{
    public delegate void onDragEvent(PointerEventData eventData);
    public onDragEvent onEndDrag;
    public void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null)
        {
            onEndDrag(eventData);
        }
    }
}
