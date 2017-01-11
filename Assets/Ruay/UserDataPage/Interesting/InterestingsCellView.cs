using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;
public class InterestingsCellView : EnhancedScrollerCellView
{
    public delegate void ToggleDelegate(bool isOn,string name);
    public ToggleDelegate ToggleChange;
    public Text NameText;
    public Toggle Checker;
    public void SetData(InterestingsData data,bool isOn) { NameText.text = data.Name; Checker.isOn = isOn; }
    public void SetInteresting(bool isOn)
    {
        if (ToggleChange != null)
        {
            ToggleChange(isOn, NameText.text);
        }
    }
}
