using UnityEngine;
using System.Collections;
using EnhancedUI.EnhancedScroller;

public class LottoNumberController : MonoBehaviour, IEnhancedScrollerDelegate
{
    public EnhancedScroller scroller;
    public LottoNumberCallView CellViewPrefab;
    void Awake()
    {
        scroller.Delegate = this;
    }
    public int GetNumberOfCells(EnhancedScroller scroller) { return 10; }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) { return 200f; }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        LottoNumberCallView cellView = scroller.GetCellView(CellViewPrefab) as LottoNumberCallView;
        cellView.SetData(dataIndex);
        return cellView;
    }
}
