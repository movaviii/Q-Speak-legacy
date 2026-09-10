namespace Q_speak;

using Q_speak.Models;
using System.Text.Json;
using SpVoice;

partial class QForm
{
    private static readonly string _configPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Q-Speak",
        "config.json"
    );
    private static readonly SpVoice _qSpeak = new();
    private static readonly List<string> _allVoices = [.. _qSpeak
        .GetVoices()
        .Cast<SpObjectToken>()
        .Select(v => v.GetDescription())];
    private static readonly List<string> _allOutputs = [.. _qSpeak
        .GetAudioOutputs()
        .Cast<SpObjectToken>()
        .Select(o => o.GetDescription())];
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };
    private static Config _config = GetConfig();

    private static Config GetConfig()
    {
        if (!File.Exists(_configPath))
        {
            var cfg = DefaultConfig();
            PostConfig(cfg);
            return cfg;
        }

        try
        {
            return JsonSerializer.Deserialize<Config>(File.ReadAllText(_configPath)) ?? DefaultConfig();
        }
        catch
        {
            var cfg = DefaultConfig();
            PostConfig(cfg);
            return cfg;
        }
    }

    private static void PostConfig(Config config)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);
        File.WriteAllText(_configPath, JsonSerializer.Serialize(config, _options));
    }

    private static Config DefaultConfig()
    {
        return new()
        {
            SelectedVoices = [.. _allVoices],
            PreferredVoice = _allVoices[0],
            PreferredOutput = _allOutputs[0],
            Pitch = 0,
            Rate = 0,
            Volume = 100,
            KeyBind = Keys.Control | Keys.Alt | Keys.X
        };
    }
}