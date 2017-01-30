using UnityEngine;
using UnityEngine.UI;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Button))]
public class HomeScrollCallView : EnhancedScrollerCellView{
    public Text NameText;
    public GameObject Mask;
    public Image Photo;
    public Button button;
    public Image BG;
    public Image Line;
    public void SetData(Card data)
    {
        switch (data.ViewType)
        {
            case CardType.PhotoWithName:
                Mask.SetActive(true);
                Line.gameObject.SetActive(false);
                BG.enabled = true;
                NameText.alignment = TextAnchor.MiddleCenter;
                break;
            case CardType.PhotoOnly:
                Mask.SetActive(true);
                Line.gameObject.SetActive(false);
                BG.enabled = true;
                NameText.alignment = TextAnchor.MiddleCenter;
                break;
            case CardType.IconWithName:
                Mask.SetActive(false);
                Line.gameObject.SetActive(false);
                BG.enabled = true;
                NameText.alignment = TextAnchor.MiddleCenter;
                break;
            case CardType.IconOnly:
                Mask.SetActive(false);
                Line.gameObject.SetActive(false);
                BG.enabled = true;
                NameText.alignment = TextAnchor.MiddleCenter;
                break;
            case CardType.NameOnly:
                Mask.SetActive(false);
                Line.gameObject.SetActive(false);
                BG.enabled = true;
                NameText.alignment = TextAnchor.MiddleCenter;
                break;
            case CardType.NameWithoutBG:
                Mask.SetActive(false);
                Line.gameObject.SetActive(false);
                BG.enabled = false;
                NameText.alignment = TextAnchor.MiddleLeft;
                break;
            case CardType.NameWithLine:
                Mask.SetActive(false);
                Line.gameObject.SetActive(true);
                BG.enabled = false;
                NameText.alignment = TextAnchor.MiddleLeft;
                break;
            default:
                break;
        }
        NameText.text = data.Name;
    }
}
