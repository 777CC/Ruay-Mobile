using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

public class LottoNumberCallView: EnhancedScrollerCellView
{
    public Text number;
    public void SetData(int i) { number.text = i.ToString(); }
}
