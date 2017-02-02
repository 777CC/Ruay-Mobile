using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class RoundChoice : RoundPage
{
    private Choice currentChoice;
    [SerializeField]
    GameObject TwoChoice;
    [SerializeField]
    Text Choice1;
    [SerializeField]
    Text Choice2;
    [SerializeField]
    GameObject MultiChoice;
    [SerializeField]
    Text HeadText;
    [SerializeField]
    Text RatioText;
    [SerializeField]
    Text Desc;
    [SerializeField]
    Text ConfirmHeader;
    public override void SetRound(Round r)
    {
        base.SetRound(r);
        if (r.Choices != null)
        {
            if (r.Choices.Length == 2)
            {
                Choice1.name = r.Choices[0].Name;
                Choice2.name = r.Choices[1].Name;
            }
            else if(r.Choices.Length > 2)
            {
                for(int i = 0; i<r.Choices.Length;i++)
                {
                    GameObject go;
                    if(i == 0)
                    {
                        MultiChoice.SetActive(true);
                        go = MultiChoice;
                        MultiChoice.GetComponentInChildren<Text>().text = r.Choices[0].Name;
                        MultiChoice.GetComponent<Button>().onClick.AddListener(() => {
                            Choose(r.Choices[0]);
                        });
                    }
                    else
                    {
                        go = Instantiate(MultiChoice,MultiChoice.transform.parent,false);
                    }
                    go.transform.SetSiblingIndex(MultiChoice.transform.GetSiblingIndex() + i);
                    go.GetComponentInChildren<Text>().text = r.Choices[i].Name;
                    int index = i;
                    go.GetComponent<Button>().onClick.AddListener(() => {
                        Choose(r.Choices[index]);
                    });
                }
            }
        }
        if(!string.IsNullOrEmpty(r.Ratio))
        {
            RatioText.text = r.Ratio;
        }
        else
        {
            RatioText.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(r.Name))
        {
            HeadText.text = r.Name;
        }
        Desc.text = r.Description;
    }
    void Choose(Choice c)
    {
        currentChoice = c;
        ConfirmHeader.text = currentChoice.Name;
        Confirmation();
    }
    public void Buy()
    {
        Manager.Instance.BuyRound(round.id, currentChoice.Value.ToString(), amount);
    }
}
