using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
public class InterestingsController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private SmallList<InterestingsData> _data;
    public EnhancedScroller scroller;
    public InterestingsCellView CellViewPrefab;
    private string[] interestingNames = new string[] { "fdsafdsa", "treghfsd", "qrweqfdsaf", "vbcxzvbsdfgv", "hgtghfbngfn", "gfdbvcxbs", "greregdfs", "vbcxbnsrhrgh", "grwegfds", "gewgfdsgfd", "ertwetfd", "bvcxbvsfg", "qerewqrte", "vbfscbvcs", "fdsafdas", "vcxzvddfg", "fgsdafgrefgdsafgsdgfsdfg" };
    public List<string> userInterestings = new List<string>();
    void Awake()
    {
        // create a new data list for the slots
        _data = new SmallList<InterestingsData>();
        scroller.Delegate = this;
    }

    void Start()
    {
        //_data = new SmallList<ScrollerData>();
        Reload(interestingNames);
    }

    public void Reload(string[] names)
    {
        // reset the data list
        _data.Clear();

        // at the sprites from the demo script to this scroller's data cells
        foreach (var n in names)
        {
            _data.Add(new InterestingsData() { Name = n });
        }

        // reload the scroller
        scroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller) { return _data.Count; }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) { return 300f; }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        InterestingsCellView cellView = scroller.GetCellView(CellViewPrefab) as InterestingsCellView;
        cellView.ToggleChange = null;
        cellView.SetData(_data[dataIndex],isToggleOn(_data[dataIndex].Name));
        cellView.ToggleChange = EditInteresting;
        return cellView;
    }
    private bool isToggleOn(string name)
    {
        Debug.Log("Toggle count : " + name + userInterestings.Contains(name));
        Debug.Log(userInterestings.ToArray());
        return userInterestings.Contains(name);
    }
    private void EditInteresting(bool isOn,string name)
    {
        if (isOn)
        {
            Debug.Log("Toggle : " + isOn + " " + name);
            if (!userInterestings.Contains(name))
            {
                userInterestings.Add(name);
            }
        }
        else
        {
            userInterestings.Remove(name);
        }
    }
    private void AddInteresting(string name)
    {
        Debug.Log("Add : " + name);
    }
    private void removeInteresting(string name)
    {
        Debug.Log("remove : " + name);
    }
}