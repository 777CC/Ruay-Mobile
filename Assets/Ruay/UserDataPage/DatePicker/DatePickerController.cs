using UnityEngine;
using System.Collections;
using EnhancedUI.EnhancedScroller;
public class DatePickerController : MonoBehaviour {
    public DateScrollerController dayScroller;
    public DateScrollerController monthScroller;
    public DateScrollerController yearScroller;

    const int yearAdderForThai = 543;
    const int yearStart = 1900;
    const int yearStartAt = 19;
    private int yearEnd = 2017;

    int day = 1, month = 1, year = 2000;
    void Start()
    {
        string[] dayData = new string[31];
        string[] monthThaiName = { "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม" };
        yearEnd = System.DateTime.Now.Year > yearEnd ? System.DateTime.Now.Year : yearEnd;
        string[] yearData = new string[yearEnd - yearStart];
        for (int i = 1; i <= 31; i++)
        {
            dayData[i - 1] = i.ToString();
        }
        for (int i = 0; i < yearEnd - yearStart; i++)
        {
            yearData[i] = (i + yearStart + yearAdderForThai).ToString();
        }
        dayScroller.Reload(dayData);
        monthScroller.Reload(monthThaiName);
        yearScroller.Reload(yearData);

        dayScroller.scroller.scrollerSnapped = daySnapped;
        monthScroller.scroller.scrollerSnapped = monthSnapped;
        yearScroller.scroller.scrollerSnapped = yearSnapped;


        //dayScroller.scroller.JumpToDataIndex(5, 0.5f, 0.5f);
        //yearScroller.scroller.JumpToDataIndex(2000 - yearStart, 0.5f, 0.5f);
        if(Manager.Instance.birthday != 0)
        {
            SetDate(Manager.Instance.birthday);
        }
        else
        {
            SetDate(20000101);
        }
    }

    public void SetDate(int date)
    {
        day = date % 100;
        month = (date % 10000) / 100;
        year = date / 10000;
        //yearScroller.scroller.JumpToDataIndex(year - yearStart, 0.5f, 0.5f);
        //monthScroller.scroller.JumpToDataIndex(month - 1, 0.5f, 0.5f);
        //changeDayInMonth();
        //dayScroller.scroller.JumpToDataIndex(day - 1, 0.5f, 0.5f);

        yearScroller.scroller.ImmediatelySnap(year - yearStart);
        monthScroller.scroller.ImmediatelySnap(month - 1);
        changeDayInMonth();
        dayScroller.scroller.ImmediatelySnap(day - 1);

        //setDateData();
    }

    private void daySnapped(EnhancedScroller scroller, int cellIndex, int dataIndex)
    {
        //Debug.Log("day : " + cellIndex + " " + dataIndex);
        day = dataIndex + 1;
        setDateData();
    }
    private void monthSnapped(EnhancedScroller scroller, int cellIndex, int dataIndex)
    {
        //Debug.Log("month : " + cellIndex + " " + dataIndex);
        month = dataIndex + 1;
        setDateData();
        changeDayInMonth();
    }
    private void yearSnapped(EnhancedScroller scroller, int cellIndex, int dataIndex)
    {
        Debug.Log("year : " + cellIndex + " " + dataIndex);
        year = dataIndex + yearStart;
        setDateData();
        changeDayInMonth();
    }
    private void changeDayInMonth()
    {
        int dayInMonth = System.DateTime.DaysInMonth(year, month);
        dayScroller.SetDayInMonth(dayInMonth, day - 1);
    }
    void setDateData()
    {
        Manager.Instance.birthday = (year * 10000) + (month * 100) + day;
    }
}
