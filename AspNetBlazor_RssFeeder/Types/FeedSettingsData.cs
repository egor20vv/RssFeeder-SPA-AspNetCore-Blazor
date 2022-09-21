using System.Diagnostics.CodeAnalysis;

namespace AspNetBlazor_RssFeeder.Types;

public class FeedSettingsData
{
    public string URL { get; set; }
    public bool IsActive { get; set; }
}

public class FeedSettingsDataComparer : IEqualityComparer<FeedSettingsData>
{
    public bool Equals(FeedSettingsData? x, FeedSettingsData? y) =>
        x != null && y != null &&
        x.IsActive == y.IsActive &&
        x.URL == y.URL;

    public int GetHashCode([DisallowNull] FeedSettingsData obj) =>
        obj.GetHashCode();
}
