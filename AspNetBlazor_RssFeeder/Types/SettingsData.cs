namespace AspNetBlazor_RssFeeder.Types;

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
}
