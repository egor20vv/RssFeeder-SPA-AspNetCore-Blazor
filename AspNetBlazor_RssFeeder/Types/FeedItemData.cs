namespace AspNetBlazor_RssFeeder.Types;

public class FeedItemData
{
    public string Id { get; set; }

    public string Title { get; set; }
    public DateTime PubDate { get; set; }
    public string Description { get; set; }

    public string ResourceName { get; set; }
    public string PostUrl { get; set; }
}