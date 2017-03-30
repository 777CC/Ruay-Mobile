using System;
using UnityEngine;
using UnityEngine.UI;
public class ItemDescPopup : RoundPopup {
    [SerializeField]
    private RawImage photo;
    [SerializeField]
    private Text Name;
    [SerializeField]
    private Text ChoiceText;
    [SerializeField]
    private Text Description;
    public void SetTicket(Item item,Ticket ticket, Texture tex)
    {
        if (item.choices != null)
        {
            Choice choice = Array.Find(item.choices, c => c.choiceValue == ticket.reserveNumber);
            string choiceName = choice.choiceName;
            if (string.IsNullOrEmpty(choiceName))
            {
                if (!string.IsNullOrEmpty(round.id))
                {
                    if (round.id[0] == 'A')
                    {
                        choiceName = ticket.reserveNumber.ToString("000000");
                    }
                }
            }
            SetView(tex, item.title, ticket.amount, choiceName, item.desc);
        }
        else
        {
            SetView(tex, item.title, ticket.amount, ticket.reserveNumber.ToString(), item.desc);
        }
    }
    public void SetReward(Item item, Reward reward,Texture tex)
    {
        Choice choice = Array.Find(item.choices, c => c.choiceValue == reward.choice);
        SetView(tex, item.title, reward.amount, choice.choiceName, item.desc);
    }
    void SetView(Texture tex,string name,int amount,string choice, string desc)
    {
        if (tex && photo != null)
        {
            photo.gameObject.SetActive(true);
            photo.texture = tex;
        }
        else
        {
            photo.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(name) && Name != null)
        {
            Name.text = name;
        }
        if (int.TryParse(amount.ToString(), out this.amount) && amountText != null)
        {
            amountText.text = this.amount.ToString();
        }
        if (!string.IsNullOrEmpty(choice) && ChoiceText != null)
        {
            ChoiceText.text = choice;
        }
        else
        {
            ChoiceText.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(desc) && Description != null)
        {
            Description.text = desc;
        }
        Show();
    }
    public override void Back()
    {
        base.Back();
        Debug.Log("BackDestroy");
        if (photo != null)
        {
            Destroy(photo);
        }
    }
}
