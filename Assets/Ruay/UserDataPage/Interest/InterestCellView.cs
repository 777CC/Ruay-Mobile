﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;
public class InterestCellView : EnhancedScrollerCellView
{
    public delegate void ToggleDelegate(bool isOn,string name);
    public ToggleDelegate ToggleChange;
    public Text NameText;
    public Toggle Checker;
    static int imageWidth, imageHeight;
    public void SetData(InterestData data,bool isOn) {
        NameText.text = data.Name; Checker.isOn = isOn;
        //if(imageWidth == 0)
        {
            Vector3[] corners = new Vector3[4];
            NameText.rectTransform.GetWorldCorners(corners);
            Rect newRect = new Rect(corners[0], corners[2] - corners[0]);
            imageWidth = (int)newRect.width;
            imageHeight = (int)newRect.height;
            Debug.Log("SetData : " + imageWidth+ " : " + imageHeight);
        }
    }
    public void SetInterest(bool isOn)
    {
        if (ToggleChange != null)
        {
            ToggleChange(isOn, NameText.text);
        }
    }
}