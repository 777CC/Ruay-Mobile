using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;
public class HomeScrollCallView : EnhancedScrollerCellView{
    public Text NameText;
    public Sprite Photo;
    public void SetData(HomeScrollData data)
    {
        NameText.text = data.Name;
    }
}
