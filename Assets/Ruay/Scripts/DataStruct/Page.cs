using System;
using System.Collections.Generic;
[Serializable]
public enum CardType
{
    PhotoWithNameCenter = 1,
    PhotoWithNameLeft = 2,
    PhotoWithNameRight = 3,
    PhotoOnly = 4,
    IconWithName = 5,
    IconOnly = 6,
    NameOnlyLeft = 7,
    NameOnlyCenter = 8,
    NameOnlyRight = 9,
    NameWithBGLeft = 10,
    NameWithLine = 11,
    Header = 12,
    Ad = 13,
    NameWithBGCenter = 14,
    NameWithBGRight = 15,
    Name2Side = 16
}//706
[Serializable]
public class Page{
    public string id;
    public string title;
    public List<Card> cards;
}
[Serializable]
public struct Card
{
    public string title;
    public string nextPage;
    public CardType viewType;
    public string photo;
    public double startTime;
    public double endTime;
}
