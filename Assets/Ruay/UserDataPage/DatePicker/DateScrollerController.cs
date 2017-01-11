using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
public class DateScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private SmallList<DateScrollerData> _data;
    public EnhancedScroller scroller;
    public DatePickerCellView CellViewPrefab;
    void Awake()
    {
        // create a new data list for the slots
        _data = new SmallList<DateScrollerData>();
        scroller.Delegate = this;
    }

    void Start()
    {
        //_data = new SmallList<ScrollerData>();
        
    }

    public void Reload(string[] names)
    {
        // reset the data list
        _data.Clear();

        // at the sprites from the demo script to this scroller's data cells
        foreach (var n in names)
        {
            _data.Add(new DateScrollerData() { Name = n });
        }

        // reload the scroller
        scroller.ReloadData();
    }

    public void SetDayInMonth(int day,int selectedDay)
    {
        //scroller.ClearAll();
        //scroller.ScrollPosition = 0;
        if (_data.Count > day)
        {
            int count = _data.Count - day;
            for (int i = 0; i < count; i++)
            {
                _data.RemoveEnd();
            }
            scroller.ReloadData();
            scroller.JumpToDataIndex(selectedDay, 0.5f, 0.5f);
        }
        else if (_data.Count < day)
        {
            for (int i = _data.Count; i < day; i++)
            {
                _data.Add(new DateScrollerData() { Name = (i + 1).ToString() });
            }
            scroller.ReloadData();
            scroller.JumpToDataIndex(selectedDay, 0.5f, 0.5f);
        }
    }

    public int GetNumberOfCells(EnhancedScroller scroller) { return _data.Count; }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) { return 150f; }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        DatePickerCellView cellView = scroller.GetCellView(CellViewPrefab) as DatePickerCellView;
        cellView.SetData(_data[dataIndex]);
        return cellView;
    }
}