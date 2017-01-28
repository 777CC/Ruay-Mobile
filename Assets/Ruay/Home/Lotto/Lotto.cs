using UnityEngine;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using System.Text;
using UnityEngine.UI;

public class Lotto : RoundPage {
    [SerializeField]
    LottoNumberController[] numberScroller;
    private string number = "999999";
    [SerializeField]
    private int minRandom = -500000;
    [SerializeField]
    private int maxRandom = 500000;
    private int amount = 1;
    [SerializeField]
    private Text amountText;
    [SerializeField]
    private Text confirmationText;
    [SerializeField]
    private GameObject errorText;
    [SerializeField]
    private CanvasGroup confirmationCanvasGroup;
    [SerializeField]
    private Text priceText;
    void Start()
    {
        StartCoroutine(SetScroller());
    }
    public override void SetRound(Round r)
    {
        base.SetRound(r);
        SetPrice();
    }
    void SetPrice()
    {   
        priceText.text = round.Price * amount + "/" + Manager.Instance.satang + " สตางค์";
    }
    IEnumerator SetScroller()
    {
        yield return new WaitForSeconds(0.2f);
        System.Array.ForEach(numberScroller, n => {
            n.scroller.ImmediatelySnap(9);
            n.scroller.scrollerSnapped = numberSnapped;
        });
    }

    private void numberSnapped(EnhancedScroller scroller, int cellIndex, int dataIndex)
    {
        for (int i = 0; i < numberScroller.Length; i++)
        {
            if (scroller == numberScroller[i].scroller)
            {
                StringBuilder numberBuf = new StringBuilder(number);
                numberBuf[i] = char.Parse(dataIndex.ToString());
                number = numberBuf.ToString();
                break;
            }
        }
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
    public void Buy()
    {
        Manager.Instance.BuyRound(round.id, number);
    }
    public void RandomFirst()
    {
        for(int i = 0; i < 3;i++)
        {
            numberScroller[i].scroller.LinearVelocity = Random.Range(minRandom, maxRandom);
        }
    }
    public void RandomLast()
    {
        for (int i = 3; i < 6; i++)
        {
            numberScroller[i].scroller.LinearVelocity = Random.Range(minRandom, maxRandom);
        }
    }
    public void AddMount(int value)
    {
        amount += value;
        if(amount < 1)
        {
            amount = 1;
        }
        amountText.text = amount.ToString();
        SetPrice();
    }
}
