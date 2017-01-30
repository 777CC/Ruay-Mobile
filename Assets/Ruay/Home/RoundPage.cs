using UnityEngine;
using UnityEngine.UI;
public class RoundPage : Popup {
    [SerializeField]
    private Text priceText;
    [SerializeField]
    private Text confirmationText;
    [SerializeField]
    private GameObject errorText;
    [SerializeField]
    private CanvasGroup confirmationCanvasGroup;
    public virtual void SetRound(Round r)
    {
        Debug.Log("SetRound");
        round = r;
        Show();
        SetPrice();
    }
    
    public void AddMount(int value)
    {
        amount += value;
        if (amount < 1)
        {
            amount = 1;
        }
        amountText.text = amount.ToString();
        SetPrice();
    }
    protected void SetPrice()
    {
        priceText.text = round.Price * amount + "/" + Manager.Instance.satang + " สตางค์";
    }
    public void Confirmation()
    {
        int satang = Manager.Instance.satang;
        int pay = amount * round.Price;
        int balance = satang - pay;
        if (balance >= 0)
        {
            confirmationText.text = amount.ToString() + "\n" +
                                pay.ToString() + "\n" +
                                balance.ToString();
            ShowConfirmation();
        }
        else
        {
            errorText.SetActive(true);
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
        errorText.SetActive(false);
    }
}
