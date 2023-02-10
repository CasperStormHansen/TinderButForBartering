using Newtonsoft.Json;

namespace TinderButForBartering;

#nullable enable
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
#nullable disable

public class Message
{
    public int MatchId { get; set; }
    public bool Own { get; set; }
    public string Content { get; set; }
    public DateTime? DateTime { get; set; }

    [JsonConstructor]
    public Message(int matchId, bool own, string content, DateTime? dateTime)
    {
        MatchId = matchId;
        Own = own;
        Content = content;
        DateTime = dateTime;
    }
}