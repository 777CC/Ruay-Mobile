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
        Choice choice = Array.Find(item.choices, c => c.value == ticket.reserveNumber);
        string choiceName = choice.name;
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
        SetView(tex,item.name,ticket.amount, choiceName,item.desc);
    }
    public void SetReward(Item item, Reward reward,Texture tex)
    {
        Choice choice = Array.Find(item.choices, c => c.value == reward.choice);
        SetView(tex, item.name, reward.amount, choice.name, item.desc);
    }
    void SetView(Texture tex,string name,int amount,string choice, string desc)
    {
        if (tex)
        {
            photo.gameObject.SetActive(true);
            photo.texture = tex;
        }
        else
        {
            photo.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(name))
        {
            Name.text = name;
        }
        if (int.TryParse(amount.ToString(), out this.amount))
        {
            amountText.text = this.amount.ToString();
        }
        if (!string.IsNullOrEmpty(choice))
        {
            ChoiceText.text = choice;
        }
        else
        {
            ChoiceText.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(desc))
        {
            Description.text = desc;
        }
        Show();
    }
}
