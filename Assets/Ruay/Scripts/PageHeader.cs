using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(RectTransform))]
public class PageHeader : MonoBehaviour
{
    private ScrollRect contentRect;
    private RectTransform me;
    [SerializeField]
    private Text headerText;
    [SerializeField]
    private Text satangText;
    [SerializeField]
    private Button back;
    public Vector2 startPos;
    private float startY;
    private float oldmeY;
    private float oldcontentY;
    private void Start()
    {
        me = GetComponent<RectTransform>();
        startPos = me.anchoredPosition;
        startY = me.position.y;
        oldmeY = me.position.y;
        oldcontentY = oldmeY;
    }
    public void Show()
    {
        me.anchoredPosition = startPos;
        oldmeY = me.position.y;
        oldcontentY = contentRect.content.position.y;
    }
    public void SetData(ScrollRect scroll,string header,bool isShowSatang,bool isShowBack,UnityEngine.Events.UnityAction onBack)
    {
        if(isShowSatang)
        {
            satangText.text = Manager.Instance.satang + " ส.";
        }
        else
        {
            satangText.text = string.Empty;
        }
        headerText.text = header;
        if (isShowBack)
        {
            back.gameObject.SetActive(true);
            back.onClick.RemoveAllListeners();
            back.onClick.AddListener(onBack);
        }
        else
        {
            back.gameObject.SetActive(false);
        }
        contentRect = scroll;
        contentRect.onValueChanged.AddListener((pos)=> {
            if (contentRect.content.position.y >= startY)
            {
                me.SetPositionAndRotation(new Vector3(me.position.x, oldmeY - (oldcontentY - contentRect.content.position.y)), Quaternion.identity);
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
            oldcontentY = contentRect.content.position.y;
        });
    }
}
