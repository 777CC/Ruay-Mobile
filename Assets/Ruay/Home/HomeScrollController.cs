using UnityEngine;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
public class HomeScrollController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private SmallList<HomeScrollData> _data;
    public EnhancedScroller scroller;
    public HomeScrollCallView CellViewPrefab;
    void Awake()
    {
        // create a new data list for the slots
        _data = new SmallList<HomeScrollData>();
        scroller.Delegate = this;
    }

    void Start()
    {
        //_data = new SmallList<ScrollerData>();
        //Reload(interestNames);

    }

    public void Reload(string[] names)
    {
        // reset the data list
        _data.Clear();

        // at the sprites from the demo script to this scroller's data cells
        foreach (var n in names)
        {
            _data.Add(new HomeScrollData() { Name = n });
        }
        // reload the scroller
        scroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller) { return _data.Count; }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) { return 300f; }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        HomeScrollCallView cellView = scroller.GetCellView(CellViewPrefab) as HomeScrollCallView;
        cellView.SetData(_data[dataIndex]);
        return cellView;
    }
}
