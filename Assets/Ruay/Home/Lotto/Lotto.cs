using UnityEngine;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using System.Text;
using UnityEngine.UI;

public class Lotto : RoundPopup {
    [SerializeField]
    LottoNumberController[] numberScroller;
    private string number = "999999";
    [SerializeField]
    private int minRandom = -500000;
    [SerializeField]
    private int maxRandom = 500000;
    [SerializeField]
    private Text desc;
    public override void Start()
    {
        base.Start();
        StartCoroutine(SetScroller());
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
    public override void SetItem(Item r, bool isItem, Texture tex)
    {
        base.SetItem(r, isItem, tex);
        if (!string.IsNullOrEmpty(r.desc))
        {
            desc.text = r.desc;
        }
    }
    public void Buy()
    {
        int n;
        if (int.TryParse(number,out n))
        {
            Manager.Instance.BuyRound(round.id, n, amount,() => {
                //Back();
            });
        }
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
}
