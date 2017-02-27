using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Popup : MonoBehaviour {
    [SerializeField]
    protected CanvasGroup canvasGroup;
    [SerializeField]
    protected ScrollRect scrollRect;
    public Action OnBack;
    public ScrollRect ContentScroll
    {
        get
        {
            return scrollRect;
        }
    }
    public virtual void Start()
    {
        PopupEndDrag drag = GetComponentInChildren<PopupEndDrag>();
        if(drag != null)
        {
            drag.OnBack = Back;
        }
    }
    //For popup not close when press back button on android.
#if UNITY_ANDROID
    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }
#endif
    public virtual void Show()
    {
        LeanTween.alphaCanvas(canvasGroup, 1, 0.3f);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public virtual void Back()
    {
        if (!LeanTween.isTweening(canvasGroup.gameObject))
        {
            LeanTween.alphaCanvas(canvasGroup, 0, 0.3f).setOnComplete(() =>
            {
                if(OnBack != null)
                {
                    OnBack();
                }
                Destroy(gameObject);
            });
        }
    }
}
