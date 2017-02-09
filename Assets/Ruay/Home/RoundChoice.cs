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
    RawImage Photo;
    [SerializeField]
    Text RatioText;
    [SerializeField]
    Text Desc;
    [SerializeField]
    Text ConfirmHeader;
    public override void SetItem(Item r,bool isItem,Texture tex)
    {
        base.SetItem(r, isItem,tex);
        if(tex)
        {
            Photo.gameObject.SetActive(true);
            Photo.texture = tex;
        }
        else
        {
            Photo.gameObject.SetActive(false);
        }
        if (r.choices != null)
        {
            if (r.choices.Length == 2)
            {
                Choice1.name = r.choices[0].name;
                Choice2.name = r.choices[1].name;
            }
            else if(r.choices.Length > 2)
            {
                for(int i = 0; i<r.choices.Length;i++)
                {
                    GameObject go;
                    if(i == 0)
                    {
                        MultiChoice.SetActive(true);
                        go = MultiChoice;
                        MultiChoice.GetComponentInChildren<Text>().text = r.choices[0].name;
                        MultiChoice.GetComponent<Button>().onClick.AddListener(() => {
                            Choose(r.choices[0]);
                        });
                    }
                    else
                    {
                        go = Instantiate(MultiChoice,MultiChoice.transform.parent,false);
                    }
                    go.transform.SetSiblingIndex(MultiChoice.transform.GetSiblingIndex() + i);
                    go.GetComponentInChildren<Text>().text = r.choices[i].name;
                    int index = i;
                    go.GetComponent<Button>().onClick.AddListener(() => {
                        Choose(r.choices[index]);
                    });
                }
            }
        }
        if(!string.IsNullOrEmpty(r.ratio))
        {
            RatioText.text = r.ratio;
        }
        else
        {
            RatioText.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(r.name))
        {
            HeadText.text = r.name;
        }
        Desc.text = r.desc;
    }
    void Choose(Choice c)
    {
        currentChoice = c;
        ConfirmHeader.text = currentChoice.name;
        Confirmation();
    }
    public void Buy()
    {
        if (isItem)
        {
            Manager.Instance.BuyItem(round.id, currentChoice.value.ToString(), amount);
        }
        else
        {
            Manager.Instance.BuyRound(round.id, currentChoice.value, amount);
        }
    }
}
