using UnityEngine;
[System.Serializable]
public class Page{
    public string Name;
    public Page()
    {
        Cards = new Card[2];
        Cards[0] = new Card("111","p1");
        Cards[1] = new Card("222","p2");
    }
    [System.Serializable]
    public struct Card
    {
        public Card(string name, string photo)
        {
            Name = name;
            Photo = photo;
            Description = "";
        }
        public string Name;
        public string Photo;
        public string Description;
    }
    public Card[] Cards;
}
