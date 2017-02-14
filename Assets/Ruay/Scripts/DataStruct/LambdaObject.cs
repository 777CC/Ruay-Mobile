using System.Collections;
using System;
[Serializable]
public struct LambdaBuyTicket
{
    public string roundId;
    public int reserveNumber;
    public int amount;
}
[Serializable]
public struct LambdaBuyReward
{
    public string itemId;
    public int choice;
    public int amount;
}

