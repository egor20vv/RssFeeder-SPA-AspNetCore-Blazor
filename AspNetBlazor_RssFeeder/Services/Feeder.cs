using AspNetBlazor_RssFeeder.Services.Interfaces;
using AspNetBlazor_RssFeeder.Types;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace AspNetBlazor_RssFeeder.Services;

public class Feeder : IFeeder
{
    public SettingsData? Settings { get; set; }


    public async Task<Result<IEnumerable<FeedItemData>>> FeedNews()
    {
        if (Settings == null)
            return new Result<IEnumerable<FeedItemData>>() { Error = new NullReferenceException("Settings field can not be null") };

        
        var feededNews = await Task.FromResult( 
            Settings.FeedsSettings.Where(f => f.IsActive).SelectMany(f =>
            {
                using var reader = XmlReader.Create(f.URL);
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

        return new Result<IEnumerable<FeedItemData>> { Value = feededNews };
    }

    public async Task<Result<bool>> WasUpdated()
    {
        throw new NotImplementedException();
    }
}
