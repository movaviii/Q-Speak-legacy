namespace Q_speak.Models;

internal partial class Config
{
    public required string PreferredVoice { get; set; }
    public required string PreferredOutput { get; set; }
    public required List<string> SelectedVoices { get; set; }
    public required int Pitch { get; set; }
    public required int Rate { get; set; }
    public required int Volume { get; set; }
    public required Keys KeyBind { get; set; }
}