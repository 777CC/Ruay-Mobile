using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
public class InterestController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private SmallList<Interest> _data;
    public EnhancedScroller scroller;
    public InterestCellView CellViewPrefab;
    private string[] interestNames = new string[] { "fdsafdsa", "treghfsd", "qrweqfdsaf", "vbcxzvbsdfgv", "hgtghfbngfn", "gfdbvcxbs", "greregdfs", "vbcxbnsrhrgh", "grwegfds", "gewgfdsgfd", "ertwetfd", "bvcxbvsfg", "qerewqrte", "vbfscbvcs", "fdsafdas", "vcxzvddfg", "fgsdafgrefgdsafgsdgfsdfg" };
    public List<string> userInterests = new List<string>();
    void Awake()
    {
        // create a new data list for the slots
        _data = new SmallList<Interest>();
        scroller.Delegate = this;//names.AddRange(JsonHelper.getJsonArray<KeyValuePair<string, string>>(test));
    }
    void Start()
    {
        //_data = new SmallList<ScrollerData>();
        Manager.Instance.UpdateAppInfo(()=> { Reload(Manager.Instance.AppInterests); });
        //Reload(Manager.Instance.AppInterests);
    }

    public void Reload(Interest[] interests)
    {
        // reset the data list
        _data.Clear();

        // at the sprites from the demo script to this scroller's data cells
        if (interests != null)
        {
            foreach (var n in interests)
            {
                _data.Add(n);
            }
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
        cellView.SetData(_data[dataIndex],isToggleOn(_data[dataIndex].value));
        cellView.ToggleChange = EditInterests;
        return cellView;
    }
    private bool isToggleOn(string val)
    {
        return userInterests.Contains(val);
    }
    private void EditInterests(bool isOn,string val)
    {
        if (isOn)
        {
            Debug.Log("Toggle : " + isOn + " " + val);
            if (!userInterests.Contains(val))
            {
                userInterests.Add(val);
            }
        }
        else
        {
            userInterests.Remove(val);
        }
    }
}