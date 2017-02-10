﻿using UnityEngine;
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
        Screen.fullScreen = false;

        scroller = GetComponent<EnhancedScroller>();
        // create a new data list for the slots
        //card = new SmallList<Card>();
        Manager.Instance.DownloadHomeJson(()=> { });
        NextPage("Home",null);
    }
#if UNITY_ANDROID
    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackPage();
        }
    }
#endif
    void LoadPage(Page nextPage)
    {
        if (nextPage != null)
        {
            Debug.Log("Load page : " + nextPage.name);
            currentPage = nextPage;
            scroller.ReloadData();
        }
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
        else if (!string.IsNullOrEmpty(pageName))
        {
            Manager.Instance.GetPageByName(pageName, NextPage);
            ChangePageEffect();
        }
    }
    void ShowItem(string prefabName,Item r,bool isItem,Texture tex)
    {
        RoundPopup round = Instantiate(Resources.Load<GameObject>(prefabName)).GetComponent<RoundPopup>();
        round.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        round.transform.SetSiblingIndex(homeCanvasGroup.transform.GetSiblingIndex() + 1);
        round.SetItem(r, isItem,tex);
    }
    void ShowTicketDesc(string prefabName, Ticket ticket,Item round, Texture tex)
    {
        ItemDescPopup popup = Instantiate(Resources.Load<GameObject>(prefabName)).GetComponent<ItemDescPopup>();
        popup.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        popup.transform.SetSiblingIndex(homeCanvasGroup.transform.GetSiblingIndex() + 1);
        popup.SetTicket(round ,ticket, tex);
    }
    void ShowRewardDesc(string prefabName, Reward reward, Item item,Texture tex)
    {
        ItemDescPopup popup = Instantiate(Resources.Load<GameObject>(prefabName)).GetComponent<ItemDescPopup>();
        popup.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        popup.transform.SetSiblingIndex(homeCanvasGroup.transform.GetSiblingIndex() + 1);
        popup.SetReward(item, reward, tex);
    }
    void BackPage()
    {
        if (viewStack.Count > 0 && !LeanTween.isTweening(gameObject))
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
    public int GetNumberOfCells(EnhancedScroller scroller) { return currentPage.cards.Count; }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
        float height;
        switch (currentPage.cards[dataIndex].viewType)
        {
            case CardType.PhotoWithName:
                height = 706;
                break;
            case CardType.PhotoOnly:
                height = 533.2f;
                break;
            case CardType.NameOnlyLeft:
                height = 200;
                break;
            case CardType.Header:
                height = 220;
                break;
            default:
                goto case CardType.NameOnlyLeft;
        }
        return height;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        HomeScrollCallView cellView = scroller.GetCellView(CellViewPrefab) as HomeScrollCallView;
            cellView.OnClick.RemoveAllListeners();
            cellView.OnClick.AddListener(() => {
                NextPage(currentPage.cards[dataIndex].nextPage, cellView.Photo);
            });
        return cellView;
    }
    private void CellViewVisibilityChanged(EnhancedScrollerCellView cellView)
    {
        // cast the cell view to our custom view
        HomeScrollCallView view = cellView as HomeScrollCallView;

        // if the cell is active, we set its data, 
        // otherwise we will clear the image back to 
        // its default state

        if (cellView.active)
            view.SetData(currentPage.cards[cellView.dataIndex]);
        else
            view.ClearImage();
    }
}
