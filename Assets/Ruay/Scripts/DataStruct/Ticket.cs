using System;
[Serializable]
public struct Ticket
{
    public int createdOn;
    public string roundId;
    public int reserveNumber;
    public int limit;
    public int amount;
    public bool announced;
}
