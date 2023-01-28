namespace TinderButForBartering;

public class Match
{
    public int MatchId { get; set; }
    public string Name { get; set; }
    public string? PictureUrl { get; set; }
    public int[] OwnProductIds { get; set; }
    public Product[] ForeignProducts { get; set; }
    public Message[] Messages { get; set; }
    public Match(int matchId, string name, string? pictureUrl, int[] ownProductIds, Product[] foreignProducts, Message[] messages)
    {
        MatchId = matchId;
        Name = name;
        PictureUrl = pictureUrl;
        OwnProductIds = ownProductIds;
        ForeignProducts = foreignProducts;
        Messages = messages;
    }
}

public class Message
{
    public bool Own { get; set; }
    public string Content { get; set; }
    public DateTime DateTime { get; set; }
    public Message(bool own, string content, DateTime dateTime)
    {
        Own = own;
        Content = content;
        DateTime = dateTime;
    }
}