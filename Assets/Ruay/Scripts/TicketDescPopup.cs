using System;
using UnityEngine;
using UnityEngine.UI;
public class TicketDescPopup : Popup {
    [SerializeField]
    private Image photo;
    [SerializeField]
    private Text Name;
    [SerializeField]
    private Text ChoiceText;
    [SerializeField]
    private Text Description;
    public void SetTicket(Round round,Ticket ticket)
    {
        if (!string.IsNullOrEmpty(round.Photo))
        {

        }
        else
        {
            photo.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(round.Name))
        {
            Name.text  = round.Name;
        }
        if (int.TryParse(ticket.amount.ToString(),out amount))
        {
            amountText.text = amount.ToString();
        }
        Choice choice = Array.Find(round.Choices, c => c.Value == ticket.reserveNumber);
        if (!string.IsNullOrEmpty(choice.Name))
        {
            ChoiceText.text = choice.Name;
        }
        if (!string.IsNullOrEmpty(round.Description))
        {
            Description.text = round.Description;
        }
        Show();
    }
}
