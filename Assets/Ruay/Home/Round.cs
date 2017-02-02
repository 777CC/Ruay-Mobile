using System;
[Serializable]
public struct Round
{
    public string id;
    public string Name;
    public string Photo;
    public string Ratio;
    public string Description;
    public int Price;
    public string DiscountPercent;
    public string DiscountPrice;
    public int ExpireDate;
    public int StartDate;
    public Choice[] Choices;
}
[Serializable]
public struct Choice
{
    public string Name;
    public int Value;
}
