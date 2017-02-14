using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(ScrollRect))]
public class PageHeader : MonoBehaviour
{
    public RectTransform contentRect;
    public RectTransform me;
    public Vector2 startPos;
    private float startY;
    public float oldmeY;
    public float oldcontentY;
    private void Start()
    {
        startPos = me.anchoredPosition;
        startY = me.position.y;
        oldmeY = me.position.y;
        oldcontentY = oldmeY;
        contentRect = GetComponent<ScrollRect>().content;
        Debug.Log(me.rect);
        GetComponent<ScrollRect>().onValueChanged.AddListener((pos)=> {
            Debug.Log(me.anchoredPosition);
            if (contentRect.position.y >= startY)
            {
                me.SetPositionAndRotation(new Vector3(me.position.x, oldmeY - (oldcontentY - contentRect.position.y)), Quaternion.identity);
            }
            else
            {
                me.anchoredPosition = startPos;
            }
            if (me.anchoredPosition.y < startPos.y)
            {
                me.anchoredPosition = startPos;
            }
            else if (me.anchoredPosition.y > me.rect.height)
            {
                me.anchoredPosition = new Vector2(startPos.x, me.rect.height);
            }
            oldmeY = me.position.y;
            oldcontentY = contentRect.position.y;
        });
    }
}
