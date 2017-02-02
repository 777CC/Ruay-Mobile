using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems; // Required when using event data
[RequireComponent(typeof(ScrollRect))]
class PopupEndDrag : MonoBehaviour, IEndDragHandler // required interface when using the OnEndDrag method.
{
    public Action OnBack;
    private ScrollRect scrollRect;
    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }
    //Do this when the user stops dragging this UI Element.
    public void OnEndDrag(PointerEventData data)
    {
        if (OnBack != null)
        {
            var pos = scrollRect.normalizedPosition.y;
            var worldCorners = new Vector3[4];
            scrollRect.content.GetWorldCorners(worldCorners);
            var result = new Rect(
                      worldCorners[0].x,
                      worldCorners[0].y,
                      worldCorners[2].x - worldCorners[0].x,
                      worldCorners[2].y - worldCorners[0].y);
            Debug.Log("OnEndDrag : " + result.yMin + " : " + result.yMax + " : " + result.y + " : " + Screen.height);
            //Debug.Log()
            float scale = 0.8f;
            float d = Screen.height * scale;
            if (worldCorners[0].y > Screen.height - d || result.yMax  < d)
            {
                OnBack();
            }
            if (pos < -0.3f || pos > 1.3f)
            {
                //OnBack();
            }
        }
    }
}