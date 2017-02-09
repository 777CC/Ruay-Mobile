using System;
using UnityEngine;
using UnityEngine.UI;
public class ItemDescPopup : Popup {
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
        if (tex)
        {
            photo.gameObject.SetActive(true);
            photo.texture = tex;
        }
        else
        {
            photo.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(item.name))
        {
            Name.text  = item.name;
        }
        if (int.TryParse(ticket.amount.ToString(),out amount))
        {
            amountText.text = amount.ToString();
        }
        Choice choice = Array.Find(item.choices, c => c.value == ticket.reserveNumber);
        if (!string.IsNullOrEmpty(choice.name))
        {
            ChoiceText.text = choice.name;
        }
        else
        {
            ChoiceText.text = ticket.reserveNumber.ToString();
        }
        if (!string.IsNullOrEmpty(item.desc))
        {
            Description.text = item.desc;
        }
        Show();
    }
    public void SetReward(Item item, Reward reward,Texture tex)
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
        if (!string.IsNullOrEmpty(item.name))
        {
            Name.text = item.name;
        }
        if (int.TryParse(reward.amount.ToString(), out amount))
        {
            amountText.text = amount.ToString();
        }
        Choice choice = Array.Find(item.choices, c => c.value == reward.choice);
        if (!string.IsNullOrEmpty(choice.name))
        {
            ChoiceText.text = choice.name;
        }
        else
        {
            ChoiceText.text = reward.choice.ToString();
        }
        if (!string.IsNullOrEmpty(item.desc))
        {
            Description.text = item.desc;
        }
        Show();
    }
}
