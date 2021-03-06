﻿using UnityEngine;
using UnityEngine.UI;
public class RoundPopup : Popup {
    protected Item round;
    protected int amount = 1;
    [SerializeField]
    protected Text amountText;
    [SerializeField]
    private Text priceText;
    [SerializeField]
    private Text confirmationText;
    [SerializeField]
    private CanvasGroup confirmationCanvasGroup;
    protected bool isItem = true;
    public virtual void SetItem(Item r,bool isItem,Texture tex)
    {
        Debug.Log("SetRound");
        round = r;
        this.isItem = isItem;
        Show();
        SetPrice();
    }
    
    public void AddMount(int value)
    {
        if (round.price * value <= Manager.Instance.satang)
        {
            amount += value;
            if (amount < 1)
            {
                amount = 1;
            }
            amountText.text = amount.ToString();
            SetPrice();
        }
    }
    protected void SetPrice()
    {
        priceText.text = round.price * amount + "/" + Manager.Instance.satang + " สตางค์";
    }
    public void Confirmation()
    {
        int pay = amount * round.price;
        int balance = Manager.Instance.satang - pay;
        int have = Manager.Instance.GetRoundAmountById(round.id);
        if (balance < 0)
        {
            Manager.Instance.DialogPopup("สตางค์ไม่พอน่ะ", "", null, null);
        }
        else if (have + amount > round.amountLimit)
        {
            Manager.Instance.DialogPopup("ไม่สามารถแลกได้", "คุณมี " + have + "ใบ จำกัดแค่ " + round.amountLimit + "ใบ", null, null);
        }
        else
        {
            confirmationText.text = amount.ToString() + "\n" +
                                pay.ToString() + "\n" +
                                balance.ToString();
            ShowConfirmation();
        }
    }
    void ShowConfirmation()
    {
        LeanTween.alphaCanvas(confirmationCanvasGroup, 1, 0.15f);
        confirmationCanvasGroup.interactable = true;
        confirmationCanvasGroup.blocksRaycasts = true;
    }
    public void HideConfirmation()
    {
        LeanTween.alphaCanvas(confirmationCanvasGroup, 0, 0.15f);
        confirmationCanvasGroup.interactable = false;
        confirmationCanvasGroup.blocksRaycasts = false;
    }
}
