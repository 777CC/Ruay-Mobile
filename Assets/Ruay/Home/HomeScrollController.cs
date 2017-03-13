using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System.Collections;
using Facebook.Unity;

public struct view
{
    public Page page;
    public float viewPos;
}

[RequireComponent(typeof(EnhancedScroller))]
public class HomeScrollController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private Canvas canvas;
    [SerializeField]
    private CanvasGroup homeCanvasGroup;
    private EnhancedScroller scroller;
    public HomeScrollCallView CellViewPrefab;
    [SerializeField]
    private PageHeader header;
    public Page currentPage;
    private SmallList<view> viewStack = new SmallList<view>();
    UnityEngine.Events.UnityAction<Vector2> scrollRectMove;
    HomeScrollCallView adBanner;
    //private SmallList<Card> card;
    void Awake()
    {
        Screen.fullScreen = false;
        canvas = FindObjectOfType<Canvas>();
        scroller = GetComponent<EnhancedScroller>();
        // create a new data list for the slots
        //card = new SmallList<Card>();
#if UNITY_EDITOR
        Manager.Instance.DownloadHomeJson(()=> { NextPage("Home", null); });
#else
        NextPage("Home", null);
#endif
    }
#if UNITY_ANDROID
    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (FindObjectOfType<Popup>() == null)
            {
                BackPage();
            }
        }
    }
#endif
    void LoadPage(Page nextPage)
    {
        if (nextPage != null)
        {
            Debug.Log("Load page : " + nextPage.name + " : " + viewStack.Count);
            currentPage = nextPage;
            scroller.ReloadData();
            header.SetData(scroller.ScrollRect, currentPage.name, viewStack.Count == 1, viewStack.Count > 1, () => { BackPage(); });
            if (scrollRectMove != null)
            {
                scroller.ScrollRect.onValueChanged.RemoveListener(scrollRectMove);
            }
            Card ad = currentPage.cards.Find((c) => c.viewType == CardType.Ad);
            if (ad.viewType == CardType.Ad)
            {
                scrollRectMove = (p) =>
                {
                    if (adBanner != null)
                    {
                        Manager.Instance.SetAdPosition(adBanner.pos);
                    }
                };
                scroller.ScrollRect.onValueChanged.AddListener(scrollRectMove);
            }
            Manager.Instance.HideAd();
            adBanner = null;
        }
    }
    void NextPage(Page nextPage)
    {
        if (viewStack.Count > 0)
        {
            viewStack.data[viewStack.Count - 1].viewPos = scroller.ScrollPosition;
        }
        view v = new view();
        v.page = nextPage;
        v.viewPos = 0;
        viewStack.Add(v);
        LoadPage(nextPage);
        scroller.Delegate = this;
        scroller.cellViewVisibilityChanged = CellViewVisibilityChanged;
    }
    void NextPage(string pageName, Texture tex)
    {
        string round = string.Empty;
        string ticket = string.Empty;
        string item = string.Empty;
        string reward = string.Empty;
        if (pageName.Length >= 6)
        {
            round = pageName.Substring(0, 6);
            ticket = pageName.Substring(0, 6);
            reward = pageName.Substring(0, 6);
        }
        if (pageName.Length >= 4)
        {
            item = pageName.Substring(0, 4);
        }
        if (pageName == "Back")
        {
            BackPage();
        }
        else if (round == "RoundA")
        {
            string id = pageName.Substring(5, pageName.Length - 5);
            Manager.Instance.GetRoundById(id, (r) => { ShowItem("LottoPopup", r, false,tex); });
        }
        else if (round == "RoundB")
        {
            string id = pageName.Substring(5, pageName.Length - 5);
            Manager.Instance.GetRoundById(id, (r) => { ShowItem("RoundChoicePopup", r, false, tex); });
        }
        else if (ticket == "Ticket")
        {
            int index;
            if (int.TryParse(pageName.Substring(6, pageName.Length - 6), out index))
            {
                Manager.Instance.GetTicketByIndex(index, (it, r) => { ShowTicketDesc("ItemDescPopup", it, r, tex); });
            }
        }
        else if (item == "Item")
        {
            string id = pageName.Substring(4, pageName.Length - 4);
            Manager.Instance.GetItemById(id, (i) => { ShowItem("RoundChoicePopup", i,true, tex); });
        }
        else if (reward == "Reward")
        {
            int index;
            if (int.TryParse(pageName.Substring(6, pageName.Length - 6), out index))
            {
                Manager.Instance.GetRewardByIndex(index, (it, r) => { ShowRewardDesc("ItemDescPopup", it, r,tex); });
            }
        }
        else if (pageName == "UserInfo")
        {
            Manager.Instance.HideAd();
            SceneManager.LoadScene("UserInfoEditor");
        }
        else if (pageName == "SetInviteName")
        {
            Popup invitePopup = Instantiate(Resources.Load<GameObject>("SetInviteNamePopup")).GetComponent<Popup>();
            Popup(invitePopup, "แก้ไขข้อมูล");
        }
        else if (pageName == "Quit")
        {
            FB.LogOut();
            PlayerPrefs.DeleteAll();
            Caching.CleanCache();
            Application.Quit();
        }
        else if (!string.IsNullOrEmpty(pageName))
        {
            Manager.Instance.GetPageById(pageName, NextPage);
            ChangePageEffect();
        }
    }
    void ShowItem(string prefabName,Item r,bool isItem,Texture tex)
    {
        RoundPopup round = Instantiate(Resources.Load<GameObject>(prefabName)).GetComponent<RoundPopup>();
        round.SetItem(r, isItem,tex);
        Popup(round, r.name);
    }
    void ShowTicketDesc(string prefabName, Ticket ticket,Item round, Texture tex)
    {
        ItemDescPopup popup = Instantiate(Resources.Load<GameObject>(prefabName)).GetComponent<ItemDescPopup>();
        popup.SetTicket(round ,ticket, tex);
        Popup(popup, round.name);
    }
    void ShowRewardDesc(string prefabName, Reward reward, Item item,Texture tex)
    {
        ItemDescPopup popup = Instantiate(Resources.Load<GameObject>(prefabName)).GetComponent<ItemDescPopup>();
        popup.SetReward(item, reward, tex);
        Popup(popup, item.name);
    }
    void Popup(Popup popup, string head)
    {
        popup.transform.SetParent(canvas.transform, false);
        popup.transform.SetSiblingIndex(homeCanvasGroup.transform.GetSiblingIndex() + 1);
        header.SetData(popup.ContentScroll, head, false, true, () =>
        {
            popup.Back();
        });
        popup.OnBack = () => {
            BackPage();
        };
        Page blank = new Page();
        blank.cards = new System.Collections.Generic.List<Card>();
        if (viewStack.Count > 0)
        {
            viewStack.data[viewStack.Count - 1].viewPos = scroller.ScrollPosition;
        }
        currentPage = blank;
        scroller.ReloadData();
        view v = new view();
        v.page = blank;
        v.viewPos = 0;
        viewStack.Add(v);
    }
    void BackPage()
    {
        if (viewStack.Count > 1 && !LeanTween.isTweening(gameObject))
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
        header.Show();
        LeanTween.alphaCanvas(homeCanvasGroup, 0.5f, 0.15f).setLoopPingPong(1).setOnComplete(() =>
        {
            homeCanvasGroup.interactable = true;
        });
    }
    public int GetNumberOfCells(EnhancedScroller scroller) { return currentPage.cards.Count; }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
        float height;
        switch (currentPage.cards[dataIndex].viewType)
        {
            case CardType.PhotoWithNameCenter:
                height = 706;
                break;
            case CardType.PhotoOnly:
                height = 533.2f;
                break;
            case CardType.NameOnlyLeft:
                height = 200;
                break;
            case CardType.Header:
                height = 170;
                break;
            case CardType.Ad:
                height = 900;
                break;
            default:
                goto case CardType.NameOnlyLeft;
        }
        return height;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        HomeScrollCallView cellView = scroller.GetCellView(CellViewPrefab) as HomeScrollCallView;
        if (cellView == adBanner && currentPage.cards[dataIndex].viewType != CardType.Ad)
        {
            Manager.Instance.HideAd();
            adBanner = null;
        }
        else
        {
            if (currentPage.cards[dataIndex].viewType == CardType.Ad)
            {
                StartCoroutine(SetAd(cellView));
            }
        }
        cellView.OnClick.RemoveAllListeners();
        if (!string.IsNullOrEmpty(currentPage.cards[dataIndex].nextPage))
        {
            cellView.OnClick.AddListener(() =>
            {
                NextPage(currentPage.cards[dataIndex].nextPage, cellView.Photo);
            });
        }
        return cellView;
    }
    IEnumerator SetAd(HomeScrollCallView view)
    {
        
        yield return new WaitForSeconds(0.2f);
        Manager.Instance.ShowAd(view.pos, (ad) =>
        {
            Debug.Log("ShowAd" + (view != null));
            if (view != null)
            {
                if (currentPage.cards.Count > view.dataIndex)
                {
                    Debug.Log("ShowAd" + view.dataIndex);
                    if (currentPage.cards[view.dataIndex].viewType == CardType.Ad)
                    {
                        Manager.Instance.SetAdPosition(view.pos);
                        adBanner = view;
                    }
                    else
                    {
                        Manager.Instance.HideAd();
                        adBanner = null;
                    }
                }
                else
                {
                    Manager.Instance.HideAd();
                    adBanner = null;
                }
            }
            else
            {
                Manager.Instance.HideAd();
                adBanner = null;
            }
        });
    }
    private void CellViewVisibilityChanged(EnhancedScrollerCellView cellView)
    {
        // cast the cell view to our custom view
        HomeScrollCallView view = cellView as HomeScrollCallView;

        // if the cell is active, we set its data, 
        // otherwise we will clear the image back to 
        // its default state
        
        if(currentPage.cards.Count == 0)
        {
            Manager.Instance.HideAd();
            adBanner = null;
        }
        else
        {
            if (currentPage.cards[cellView.dataIndex].viewType == CardType.Ad)
            {
                if (cellView.active)
                {
                    //Debug.Log("ShowAd" + view.pos);
                    //Manager.Instance.ShowAd(view.pos,(ad)=> {
                    //    Manager.Instance.SetAdPosition(view.pos);
                    //});
                    //adBanner = view;
                }
                else
                {
                    Debug.Log("HideAd");
                    Manager.Instance.HideAd();
                    adBanner = null;
                }
            }
        }
        if (cellView.active) { 
            view.SetData(currentPage.cards[cellView.dataIndex]);
        }
        else
        {
            view.ClearImage();
        }
    }
}