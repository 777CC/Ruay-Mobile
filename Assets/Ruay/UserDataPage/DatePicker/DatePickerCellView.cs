using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;
public class DatePickerCellView : EnhancedScrollerCellView
{
    //public delegate void CellButtonIntegerClickedDelegate(int value);

    public Text NameText;
    public void SetData(DateScrollerData data) { NameText.text = data.Name; }
}
