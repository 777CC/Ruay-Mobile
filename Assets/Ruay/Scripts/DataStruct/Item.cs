using System;
[Serializable]
public struct Item
{
    public string id;
    public string name;
    public string photo;
    public string ratio;
    public string desc;
    public int price;
    public int limit;
    public string discountPercent;
    public string discountPrice;
    public int startDate;
    public int expireDate;
    public Choice[] choices;

}
[Serializable]
public struct Choice
{
    public string name;
    public int value;
}
