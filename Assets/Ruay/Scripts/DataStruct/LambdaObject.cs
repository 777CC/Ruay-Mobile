using System;
[Serializable]
public struct LambdaSetInviteName
{
    public string oldInviteName;
    public string newInviteName;
}
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

