using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
public class InterestController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private SmallList<InterestData> _data;
    public EnhancedScroller scroller;
    public InterestCellView CellViewPrefab;
    private string[] interestNames = new string[] { "fdsafdsa", "treghfsd", "qrweqfdsaf", "vbcxzvbsdfgv", "hgtghfbngfn", "gfdbvcxbs", "greregdfs", "vbcxbnsrhrgh", "grwegfds", "gewgfdsgfd", "ertwetfd", "bvcxbvsfg", "qerewqrte", "vbfscbvcs", "fdsafdas", "vcxzvddfg", "fgsdafgrefgdsafgsdgfsdfg" };
    public List<string> userInterests = new List<string>();
    void Awake()
    {
        // create a new data list for the slots
        _data = new SmallList<InterestData>();
        scroller.Delegate = this;
    }

    void Start()
    {
        //_data = new SmallList<ScrollerData>();
        Reload(interestNames);
    }

    public void Reload(string[] names)
    {
        // reset the data list
        _data.Clear();

        // at the sprites from the demo script to this scroller's data cells
        foreach (var n in names)
        {
            _data.Add(new InterestData() { Name = n });
        }
        // reload the scroller
        scroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller) { return _data.Count; }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) { return 300f; }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        InterestCellView cellView = scroller.GetCellView(CellViewPrefab) as InterestCellView;
        cellView.ToggleChange = null;
        cellView.SetData(_data[dataIndex],isToggleOn(_data[dataIndex].Name));
        cellView.ToggleChange = EditInterests;
        return cellView;
    }
    private bool isToggleOn(string name)
    {
        Debug.Log("Toggle count : " + name + userInterests.Contains(name));
        Debug.Log(userInterests.ToArray());
        return userInterests.Contains(name);
    }
    private void EditInterests(bool isOn,string name)
    {
        if (isOn)
        {
            Debug.Log("Toggle : " + isOn + " " + name);
            if (!userInterests.Contains(name))
            {
                userInterests.Add(name);
            }
        }
        else
        {
            userInterests.Remove(name);
        }
    }
}