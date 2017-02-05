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
        NextPage("Home");
    }

    void LoadPage(Page nextPage)
    {
        if (nextPage != null)
        {
            Debug.Log("Load page : " + nextPage.Name);
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
    void NextPage(string pageName)
    {
        string round = string.Empty;
        string item = string.Empty;
        if (pageName.Length >= 6)
        {
            round = pageName.Substring(0, 6);
            item = pageName.Substring(0, 6);
        }
        if (pageName == "Back")
        {
            BackPage();
        }
        else if (round == "RoundA")
        {
            string id = pageName.Substring(5, pageName.Length - 5);
            Manager.Instance.GetRoundById(id, (r) => { ShowRound("LottoPopup", r); });
        }
        else if (round == "RoundB")
        {
            string id = pageName.Substring(5, pageName.Length - 5);
            Manager.Instance.GetRoundById(id, (r) => { ShowRound("RoundChoicePopup", r); });
        }
        else if (round == "RoundC")
        {
            string id = pageName.Substring(5, pageName.Length - 5);
            Manager.Instance.GetRoundById(id, (r) => { ShowRound("RoundChoicePopup", r); });
        }
        else if (item == "Ticket")
        {
            int index;
            if (int.TryParse(pageName.Substring(6, pageName.Length - 6), out index))
            {
                Manager.Instance.GetTicketById(index, (it,r) => { ShowTicketDesc("TicketDescPopup", it,r); });
            }
        }
        else if (!string.IsNullOrEmpty(pageName))
        {
            Manager.Instance.GetPageByName(pageName, NextPage);
            ChangePageEffect();
        }
    }
    void ShowRound(string prefabName,Round r)
    {
        RoundPage round = Instantiate(Resources.Load<GameObject>(prefabName)).GetComponent<RoundPage>();
        round.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        round.transform.SetSiblingIndex(homeCanvasGroup.transform.GetSiblingIndex() + 1);
        round.SetRound(r);
    }
    void ShowTicketDesc(string prefabName, Ticket ticket,Round round)
    {
        TicketDescPopup popup = Instantiate(Resources.Load<GameObject>(prefabName)).GetComponent<TicketDescPopup>();
        popup.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        popup.transform.SetSiblingIndex(homeCanvasGroup.transform.GetSiblingIndex() + 1);
        popup.SetTicket(round ,ticket);
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
                height = 533.2f;
                break;
            case CardType.NameOnly:
                height = 200;
                break;
            default:
                goto case CardType.NameOnly;
        }
        return height;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        HomeScrollCallView cellView = scroller.GetCellView(CellViewPrefab) as HomeScrollCallView;
            //cellView.SetData(currentPage.Cards[dataIndex]);
            cellView.button.onClick.RemoveAllListeners();
            cellView.button.onClick.AddListener(() => {
                Debug.Log("Nextpage : " + currentPage.Cards[dataIndex].NextPage);
                NextPage(currentPage.Cards[dataIndex].NextPage);
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
            view.SetData(currentPage.Cards[cellView.dataIndex]);
        else
            view.ClearImage();
    }
}
