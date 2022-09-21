using AspNetBlazor_RssFeeder.Services.Interfaces;
using AspNetBlazor_RssFeeder.Types;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace AspNetBlazor_RssFeeder.Services;

public class SettingsService : ISettingsService
{
    private static readonly string _settingsFileName = "settings.xml";
    public static readonly string _settingsSchemaFileName = "SettingsSchema.xsd";
    //public static readonly string _settingsSchemaNS = "RssFeeder.SettingsSchema";

    public static SettingsService CreateWithFile()
    {
        var settingsFile = File.Open(_settingsFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        return new SettingsService { SettingsFile = settingsFile };
    } 

    public static SettingsService CreateEmpty()
    {
        return new SettingsService();
    }



    private XmlSchemaSet _settingsSchemaSet;
    private SettingsData? _cached;

    public Stream? SettingsFile { get; set; } = null;


    public SettingsService() 
    {
        using var settingsSchemeReader = new StreamReader(_settingsSchemaFileName);

        _settingsSchemaSet = new XmlSchemaSet();
        var settingsSchema = XmlSchema.Read(settingsSchemeReader, null);
        _settingsSchemaSet.Add(settingsSchema!);
    }


    public async Task<Result<SettingsData>> Get()
    {
        // return cached if was changes
        if (_cached != null)
            return new Result<SettingsData> { Value = _cached };
            

        // check is stream valid
        if (!IsStreamValide())
            return new Result<SettingsData> { Error = new IOException("SettingsFile stream is not valid") };

        // read and validate xml from stream
        var settingsXmlData = LoadXmlDocument(SettingsFile!, out Exception? validationError);
        if (validationError != null)
            return new Result<SettingsData> { Error = validationError };

        // create SettingsData from the validated xml stream
        _cached = await Task.FromResult(new SettingsData
        {
            FeedsSettings = settingsXmlData!.Descendants("feed")!
                .Select(f => new FeedSettingsData
                {
                    IsActive = (bool)f.Attribute("is_active")!,
                    URL = f.Element("url")!.Value
                }),
            UpdateFrequency = (float)settingsXmlData.Descendants("update_frequency").First(),
            StyleDescription = (bool)settingsXmlData.Descendants("style_description").First()
        });

        return new Result<SettingsData> { Value = _cached };
    }

    public async Task<Result<bool>> Set(SettingsData settingsData)
    {
        // return false if was no changes 
        if (settingsData.Equals(_cached))
            return new Result<bool> { Value = false };
        else
            _cached = settingsData;

        // check is stream valid
        if (!IsStreamValide())
            return new Result<bool> { Error = new IOException("SettingsFile stream is not valid") };

        // set resultint xml 
        var result = await Task.FromResult( 
            new XElement("settings",
                new XElement("feeds",
                    settingsData.FeedsSettings.Select(f =>
                        new XElement("feed",
                            new XElement("url", f.URL),
                            new XAttribute("is_active", f.IsActive)
                        )
                    )
                ),
                new XElement("update_frequency", settingsData.UpdateFrequency),
                new XElement("style_description", settingsData.StyleDescription)
            ));

        // clear stream
        await ClearStream(SettingsFile!);

        // save to file
        SettingsFile!.Position = 0;
        await Task.Run(() => result.Save(SettingsFile!));
        return new Result<bool> { Value = true };
    }

    public async ValueTask DisposeAsync()
    {
        if (IsStreamValide())
            await SettingsFile!.DisposeAsync();
    }


    private bool IsStreamValide() =>
        !(SettingsFile == null || !SettingsFile.CanSeek || !SettingsFile.CanWrite || !SettingsFile.CanRead);

    private SettingsData GetDefaultSettingsData() =>
        new SettingsData
        {
            FeedsSettings = new[] { new FeedSettingsData { URL = "https://habr.com/ru/rss/interesting/", IsActive = true } },
            StyleDescription = false,
            UpdateFrequency = 100f
        };

    private XDocument? LoadXmlDocument(Stream xml, out Exception? e)
    {
        try
        {
            e = null;
            SettingsFile!.Position = 0;
            var doc = XDocument.Load(SettingsFile!);
            doc.Validate(_settingsSchemaSet, (s, e) =>
            {
                if (e.Severity == XmlSeverityType.Error)
                    throw e.Exception;
            });
            return doc;
        }
        catch(Exception _e)
        {
            e = _e;
            return null;
        }
    }

    private async Task ClearStream(Stream stream)
    {
        await stream.WriteAsync(new byte[stream.Length], 0, (int)stream.Length);
        await stream.FlushAsync();
        stream.Position = 0;
        stream.SetLength(0);
    }
}
