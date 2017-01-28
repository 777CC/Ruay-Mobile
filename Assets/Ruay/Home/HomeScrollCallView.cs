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
    public void SetData(Card data)
    {
        switch (data.ViewType)
        {
            case CardType.PhotoWithName:
                Mask.SetActive(true);
                break;
            case CardType.PhotoOnly:
                Mask.SetActive(true);
                break;
            case CardType.IconWithName:
                Mask.SetActive(false);
                break;
            case CardType.IconOnly:
                Mask.SetActive(false);
                break;
            case CardType.NameOnly:
                Mask.SetActive(false);
                break;
            default:
                break;
        }
        NameText.text = data.Name;
    }
}
