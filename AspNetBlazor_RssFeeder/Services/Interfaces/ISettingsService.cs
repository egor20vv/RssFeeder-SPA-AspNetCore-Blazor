using AspNetBlazor_RssFeeder.Types;

namespace AspNetBlazor_RssFeeder.Services.Interfaces;

public interface ISettingsService : IAsyncDisposable
{
    Stream? SettingsFile { get; set; }

    Task<Result<SettingsData>> Get();
    Task<Result<bool>> Set(SettingsData settingsData);
}
