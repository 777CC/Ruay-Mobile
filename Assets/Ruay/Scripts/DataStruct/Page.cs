using System;
using System.Collections.Generic;
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
    NameWithBGLeft = 8,
    NameWithLine = 9,
    Header = 10,
    Ad = 11,
    NameWithBGCenter = 12,
    NameWithBGRight = 13,
}//706
[Serializable]
public class Page{
    public string id;
    public string name;
    public List<Card> cards;
}
[Serializable]
public struct Card
{
    public string name;
    public string nextPage;
    public CardType viewType;
    public string photo;
}
