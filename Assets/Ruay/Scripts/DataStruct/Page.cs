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
    NameOnlyLeft = 5,
    NameOnlyCenter = 6,
    NameOnlyRight = 7,
    NameWithBG = 8,
    NameWithLine = 9,
    Header = 10,
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
}
