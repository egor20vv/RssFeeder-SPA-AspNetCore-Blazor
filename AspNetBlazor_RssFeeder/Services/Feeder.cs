using AspNetBlazor_RssFeeder.Services.Interfaces;
using AspNetBlazor_RssFeeder.Types;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;

namespace AspNetBlazor_RssFeeder.Services;

public class Feeder : IFeeder
{
    private IEnumerable<FeedItemData>? _cachedFeedItems;


    public SettingsData? Settings { get; set; }


    public async Task<Result<IEnumerable<FeedItemData>>> FeedNews()
    {
        if (Settings == null)
            return new Result<IEnumerable<FeedItemData>>() { Error = new NullReferenceException("Settings field can not be null") };

        _cachedFeedItems = await Task.FromResult( 
            Settings.FeedsSettings.Where(f => f.IsActive).SelectMany(f =>
            {
                using var reader = CreateXmlReaderFromUrl(f.URL);
                if (reader == null)
                    return Array.Empty<FeedItemData>();
                var feed = SyndicationFeed.Load(reader);
                return feed.Items.Select(i =>
                    new FeedItemData
                    {
                        Id = i.Id,

                        Title = i.Title.Text,
                        Description = i.Summary.Text,
                        PubDate = i.PublishDate.LocalDateTime,

                        PostUrl = i.Links.First().Uri.ToString(),
                        ResourceName = feed.Title.Text
                    }
                );
            }).OrderByDescending(f => f.PubDate)
        );

        var feedSettings = _cachedFeedItems.ToList();

        if (Settings.StyleDescription == false)
            foreach (var feed in feedSettings)
                feed.Description = WebUtility.HtmlDecode(Regex.Replace(feed.Description, @"<.*?>", string.Empty)).Trim();

        return new Result<IEnumerable<FeedItemData>> { Value = feedSettings };
    }

    public async Task<Result<bool>> WasUpdated()
    {
        if (_cachedFeedItems == null)
            return new Result<bool> { Value = true };

        var cachedFirstId = _cachedFeedItems.First().Id;

        var wasUpdated = await Task.FromResult(Settings?.FeedsSettings.All(f =>
        {
            using var reader = CreateXmlReaderFromUrl(f.URL);
            if (reader == null)
                return true;
            return cachedFirstId != SyndicationFeed.Load(reader).Items.MaxBy(s => s.PublishDate.LocalDateTime)?.Id;
        }));

        return new Result<bool> { Value = wasUpdated ?? false };
    }

    private XmlReader? CreateXmlReaderFromUrl(string url)
    {
        try
        {
            return XmlReader.Create(url);
        }
        catch
        {
            return null;
        }
    }
}
