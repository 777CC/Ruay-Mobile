using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogPopup : Popup{
    [SerializeField]
    private Text header;
    [SerializeField]
    private Text desc;
    [SerializeField]
    private Button accept;
    [SerializeField]
    private Button cancel;
   public  void SetDialog(string head,string description, Action onAccept,Action onCancel)
    {
        header.text = head;
        desc.text = description;
        accept.onClick.AddListener(()=> {
            Back();
            if (onAccept != null)
            {
                onAccept();
            }
        });
        if (onCancel == null)
        {
            cancel.gameObject.SetActive(false);
        }
        else
        {
            cancel.onClick.AddListener(() => {
                Back();
                if (onCancel != null)
                {
                    onCancel();
                }
            });
        }
        Show();
    }
}
