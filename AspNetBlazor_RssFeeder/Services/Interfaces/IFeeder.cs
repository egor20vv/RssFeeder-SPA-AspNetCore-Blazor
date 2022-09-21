using AspNetBlazor_RssFeeder.Types;

namespace AspNetBlazor_RssFeeder.Services.Interfaces;

public interface IFeeder
{
    SettingsData? Settings { get; set; }

    Task<Result<bool>> WasUpdated();
    Task<Result<IEnumerable<FeedItemData>>> FeedNews();
}
