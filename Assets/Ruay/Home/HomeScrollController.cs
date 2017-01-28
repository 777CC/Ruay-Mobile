using UnityEngine;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System.Collections;

public struct view
{
    public Page page;
    public float viewPos;
}

[RequireComponent(typeof(EnhancedScroller))]
public class HomeScrollController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField]
    private CanvasGroup homeCanvasGroup;
    private EnhancedScroller scroller;
    public HomeScrollCallView CellViewPrefab;
    private Page currentPage;
    private SmallList<view> viewStack = new SmallList<view>();
    //private SmallList<Card> card;
    void Awake()
    {
        scroller = GetComponent<EnhancedScroller>();
        // create a new data list for the slots
        //card = new SmallList<Card>();
        Manager.Instance.DownloadHomeJson(()=> { });
        NextPage("Home");
    }
    void LoadPage(Page nextPage)
    {
        currentPage = nextPage;
        scroller.Delegate = this;
        scroller.ReloadData();
    }
    void NextPage(Page nextPage)
    {
        if (viewStack.Count > 0)
        {
            viewStack.data[viewStack.Count - 1].viewPos = scroller.ScrollPosition;
        }
        LoadPage(nextPage);
        view v = new view();
        v.page = nextPage;
        v.viewPos = 0;
        viewStack.Add(v);
    }
    void NextPage(string pageName)
    {
        string round = string.Empty;
        if (pageName.Length >= 6)
        {
            round = pageName.Substring(0, 6);
        }
        Debug.Log(pageName + " : " + pageName.Length);
        if (pageName == "Back")
        {
            BackPage();
        }
        else if (round == "RoundA")
        {
            string id = pageName.Substring(5, pageName.Length - 5);
            Manager.Instance.GetRoundById(id, (r) => { PopRound("LottoPopup", r); });
        }
        else if (round == "RoundB")
        {
            string id = pageName.Substring(5, pageName.Length - 5);
            Manager.Instance.GetRoundById(id, (r) => { PopRound("Round2ChoicePopup", r); });
        }
        else if (round == "RoundC")
        {
            string id = pageName.Substring(5, pageName.Length - 5);
            Manager.Instance.GetRoundById(id, (r) => { PopRound("Round2ChoicePopup", r); });
        }
        else if (!string.IsNullOrEmpty(pageName))
        {
            Manager.Instance.GetPageByName(pageName, NextPage);
            ChangePageEffect();
        }
    }
    void PopRound(string prefabName,Round r)
    {
        RoundPage round = Instantiate(Resources.Load<GameObject>(prefabName)).GetComponent<RoundPage>();
        round.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        round.transform.SetSiblingIndex(homeCanvasGroup.transform.GetSiblingIndex() + 1);
        round.SetRound(r);
    }
    void BackPage()
    {
        if (viewStack.Count > 0)
        {
            viewStack.RemoveEnd();
            LoadPage(viewStack.Last().page);
            scroller.ScrollPosition = viewStack.Last().viewPos;
            ChangePageEffect();
        }
    }
    void ChangePageEffect()
    {
        homeCanvasGroup.interactable = false;
        LeanTween.alphaCanvas(homeCanvasGroup, 0.5f, 0.15f).setLoopPingPong(1).setOnComplete(() =>
        {
            homeCanvasGroup.interactable = true;
        });
    }
    public int GetNumberOfCells(EnhancedScroller scroller) { return currentPage.Cards.Count; }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
        float height;
        switch (currentPage.Cards[dataIndex].ViewType)
        {
            case CardType.PhotoWithName:
                height = 706;
                break;
            case CardType.PhotoOnly:
                height = 706;
                break;
            case CardType.IconWithName:
                height = 200;
                break;
            case CardType.IconOnly:
                height = 706;
                break;
            case CardType.NameOnly:
                height = 706;
                break;
            default:
                height = 706;
                break;
        }
        return height;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        HomeScrollCallView cellView = scroller.GetCellView(CellViewPrefab) as HomeScrollCallView;
        cellView.SetData(currentPage.Cards[dataIndex]);
        cellView.button.onClick.RemoveAllListeners();
        cellView.button.onClick.AddListener(() => {
            NextPage(currentPage.Cards[dataIndex].NextPage); });
        return cellView;
    }
}
