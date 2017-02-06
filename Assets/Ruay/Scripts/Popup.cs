using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Popup : MonoBehaviour {
    [SerializeField]
    protected CanvasGroup canvasGroup;
    protected Item round;
    protected int amount = 1;
    [SerializeField]
    protected Text amountText;
    public virtual void Start()
    {
        Debug.Log("PopupEndDrag");
        PopupEndDrag drag = GetComponentInChildren<PopupEndDrag>();
        Debug.Log(drag.name);
        if(drag != null)
        {
            drag.OnBack = Back;
        }
    }
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
                Destroy(gameObject);
            });
        }
    }
}
