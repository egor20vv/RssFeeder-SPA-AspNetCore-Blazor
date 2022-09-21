using AspNetBlazor_RssFeeder.Types;
using System.Diagnostics.CodeAnalysis;

namespace AspNetBlazor_RssFeeder.Services.Interfaces;

public class FeedSettingsData
{
    public string URL { get; set; }
    public bool IsActive { get; set; }
}

public class SettingsData
{
    public IEnumerable<FeedSettingsData> FeedsSettings { get; set; }
    public float UpdateFrequency { get; set; }
    public bool StyleDescription { get; set; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (obj is not SettingsData) return false;

        var _obj = obj as SettingsData;

        return
            UpdateFrequency == _obj!.UpdateFrequency &&
            StyleDescription == _obj!.StyleDescription &&
            FeedsSettings.SequenceEqual(_obj!.FeedsSettings, new FeedSettingsDataComparer());
    }

    private class FeedSettingsDataComparer : IEqualityComparer<FeedSettingsData>
    {
        public bool Equals(FeedSettingsData? x, FeedSettingsData? y) =>
            x != null && y != null &&
            x.IsActive == y.IsActive &&
            x.URL == y.URL;

        public int GetHashCode([DisallowNull] FeedSettingsData obj) =>
            obj.GetHashCode();
    }
}


public interface ISettingsService : IAsyncDisposable
{
    Stream? SettingsFile { get; set; }

    Task<Result<SettingsData>> Get();
    Task<Result<bool>> Set(SettingsData settingsData);
}
