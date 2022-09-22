namespace AspNetBlazor_RssFeeder.Types;

public class SettingsData : ICloneable
{
    private int _UpdateFrequency;

    public List<FeedSettingsData> FeedsSettings { get; set; }
    public int UpdateFrequency 
    { 
        get => _UpdateFrequency;
        set => _UpdateFrequency = value >= 10 ? value : 10; 
    }
    public bool StyleDescription { get; set; }

    public object Clone()
    {
        return new SettingsData
        {
            FeedsSettings = new(FeedsSettings.Select(f => new FeedSettingsData { IsActive = f.IsActive, URL = f.URL })),
            StyleDescription = StyleDescription,
            UpdateFrequency = UpdateFrequency
        };
    }

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
