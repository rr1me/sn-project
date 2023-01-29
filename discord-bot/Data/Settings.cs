using Newtonsoft.Json;

namespace discordBot.Data;

public class Settings
{
    public SettingsEntity _settingsEntity;
    
    private readonly string JsonPath = Path.Combine(Environment.CurrentDirectory, "Data", "Settings.json");
    
    public dynamic GetSettings()
    {
        string jsonAsString;
        using (StreamReader streamReader = new StreamReader(JsonPath)) 
            jsonAsString = streamReader.ReadToEnd();
        
        _settingsEntity = JsonConvert.DeserializeObject<SettingsEntity>(jsonAsString)!;
        return _settingsEntity;
    }

    public void SaveSettings(SettingsEntity settings)
    {
        using (StreamWriter streamWriter = File.CreateText(JsonPath))
        {
            var jsonSerializer = JsonSerializer.Create();
            jsonSerializer.Formatting = Formatting.Indented;
            
            jsonSerializer.Serialize(streamWriter, settings);
        }
    }
}

public class SettingsEntity
{
    public string Token { get; set; }
    public ulong MessageId { get; set; }
    public string MessageText { get; set;}
    public ulong MentionChannelId { get; set; }
    public ulong NewsChannelId { get; set; }
    public Dictionary<string, ulong> EmoteAndRole { get; set; }
}
