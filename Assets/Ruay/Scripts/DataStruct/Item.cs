using System;
[Serializable]
public struct Item
{
    public string id;
    public string title;
    public string photo;
    public string ratio;
    public string desc;
    public int price;
    public int amountLimit;
    public string discountPercent;
    public string discountPrice;
    public double startDate;
    public double expireDate;
    public Choice[] choices;

}
[Serializable]
public struct Choice
{
    public string choiceName;
    public int choiceValue;
}
