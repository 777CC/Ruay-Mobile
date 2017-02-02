using UnityEngine;
using UnityEngine.UI;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Button))]
public class HomeScrollCallView : EnhancedScrollerCellView{
    public Text NameText;
    public GameObject Mask;
    public RawImage Photo;
    public Button button;
    public Image BG;
    public Image Line;
    private string currentPhotoName;
    public void SetData(Card data)
    {
        NameText.text = data.Name;
        currentPhotoName = data.Photo;
        switch (data.ViewType)
        {
            case CardType.PhotoWithName:
                SetView(true, true, false, TextAnchor.MiddleLeft);
                break;
            case CardType.PhotoOnly:
                SetView(true, true, false, TextAnchor.MiddleLeft);
                break;
            case CardType.IconWithName:
                SetView(true, false, false, TextAnchor.MiddleLeft);
                break;
            case CardType.IconOnly:
                SetView(true, false, false, TextAnchor.MiddleLeft);
                break;
            case CardType.NameOnly:
                SetView(false, false, false, TextAnchor.MiddleLeft);
                break;
            case CardType.NameWithBG:
                SetView(true, false, false, TextAnchor.MiddleLeft);
                break;
            case CardType.NameWithLine:
                SetView(false, false, true, TextAnchor.MiddleLeft);
                break;
            default:
                break;
        }
    }
    void LoadPhoto(string name)
    {
        Manager.Instance.GetPhotoByName(name, SetPhoto);
    }
    void SetPhoto(string name,Texture tex)
    {
        if(name == currentPhotoName)
        {
            Photo.texture = tex;
        }
    }
    void SetView(bool isBG,bool isPhoto,bool isLine, TextAnchor nameAli)
    {
        BG.enabled = isBG;
        Mask.SetActive(isPhoto);
        if (!string.IsNullOrEmpty(currentPhotoName))
        {
            LoadPhoto(currentPhotoName);
        }
        else
        {
            Photo.texture = null;
        }
        Line.enabled = isLine;
        NameText.alignment = nameAli;
    }
}
