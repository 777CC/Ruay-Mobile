using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public enum CardType
{
    PhotoWithName = 1,
    PhotoOnly = 2,
    IconWithName = 3,
    IconOnly = 4,
    NameOnly = 5,
    NameWithoutBG = 6,
    NameWithLine = 7,
}//706
[Serializable]
public class Page{
    public string Name;
    public List<Card> Cards;
}
[Serializable]
public struct Card
{
    public string Name;
    public string NextPage;
    public CardType ViewType;
    public string Photo;
    public string PhotoBlur;
    public string Description;
}
